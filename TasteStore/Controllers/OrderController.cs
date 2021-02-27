using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Utility;

namespace TasteStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeSettings _options;
        private readonly ILogger<OrderController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IUnitOfWork unitOfWork, IOptions<StripeSettings> options, ILogger<OrderController> logger, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _options = options.Value;
            _logger = logger;
            _userManager = userManager;

            StripeConfiguration.ApiKey = _options.SecretKey;
        }

        [HttpPost("stripe/createsession")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Post(SummaryOrder summaryOrder)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(summaryOrder.UserId);
                if (user == null)
                {
                    return new JsonResult(new { Success = false, Message = "Invalid user" });
                }

                var shoppingCartItems = _unitOfWork.ShoppingCartRepository.GetAll(filter: sc => sc.ApplicationUserId == summaryOrder.UserId, includeProperties: "MenuItem")
                    .ToList();

                if (shoppingCartItems.Count == 0)
                {
                    return new JsonResult(new { Success = false, Message = "Empty shopping cart" });
                }

                var lineItems = new List<SessionLineItemOptions>();

                shoppingCartItems.ForEach(sc =>
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = Convert.ToInt32(sc.MenuItem.Price * 100),
                            Currency = "brl",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = sc.MenuItem.Name,
                                Description = sc.MenuItem.Description,
                                Images = new List<string> { $"{_options.ImagesBaseUrl}/{sc.MenuItem.Image}" }
                            }
                        },
                        Quantity = sc.Quantity
                    });
                });

                var options = new SessionCreateOptions
                {
                    CustomerEmail = user.Email,
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    Metadata = new Dictionary<string, string> 
                    { 
                        { "UserId", summaryOrder.UserId },
                        { "PickupName", summaryOrder.PickupName },
                        { "PickupDate", summaryOrder.PickupDate.ToString() },
                        { "PickupTime", summaryOrder.PickupTime.ToString() },
                        { "PickupPhoneNumber", summaryOrder.PickupPhoneNumber },
                        { "PickupComments", summaryOrder.PickupComments }
                    },
                    SuccessUrl = $"{_options.SuccessUrl}/{{CHECKOUT_SESSION_ID}}",
                    CancelUrl = _options.CancelUrl
                };

                var service = new SessionService();
                Session session = service.Create(options);

                _logger.LogInformation($"[HttpPost(stripe/createsession)] session: {JsonConvert.SerializeObject(session)}");

                return new JsonResult(new { Success = true, SessionId = session.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[HttpPost(stripe/createsession)]: {ex}");
                return new JsonResult(new { Success = false, Message = "Internal server error" });
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> Post(CreateOrderApiRequest createOrderApiRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createOrderApiRequest.SessionId))
                {
                    return BadRequest("Empty session id");
                }

                var user = await _userManager.FindByIdAsync(createOrderApiRequest.UserId);
                if (user == null)
                {
                    return BadRequest("Invalid user");
                }

                var sessionService = new SessionService();
                Session session = sessionService.Get(createOrderApiRequest.SessionId);

                if (session.Metadata["UserId"] != user.Id)
                {
                    return BadRequest("Invalid session id");
                }

                var shoppingCartItems = _unitOfWork.ShoppingCartRepository.GetAll(sc => sc.ApplicationUserId == createOrderApiRequest.UserId, includeProperties: "MenuItem")
                    .ToList();

                if (shoppingCartItems.Count == 0)
                {
                    return BadRequest("Empty shopping cart");
                }

                OrderHeader orderHeader = new OrderHeader
                {
                    UserId = createOrderApiRequest.UserId,
                    PickupName = session.Metadata["PickupName"],
                    PhoneNumber = session.Metadata["PickupPhoneNumber"],
                    PickupDate = DateTime.Parse(session.Metadata["PickupDate"]),
                    PickupTime = DateTime.Parse(session.Metadata["PickupTime"]),
                    Comments = session.Metadata["PickupComments"],
                    OrderDate = DateTime.Now,
                    OrderTotal = shoppingCartItems.Sum(sc => sc.MenuItem.Price * sc.Quantity),
                    PaymentStatus = SD.PaymentStatusPending,
                    Status = SD.OrderStatusPlaced,
                    CheckoutPaymentStatus = session.PaymentStatus,
                    PaymentMethodTypes = string.Join(",", session.PaymentMethodTypes),
                    PaymentIntentId = session.PaymentIntentId
                };

                if (session.PaymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase))
                {
                    orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    orderHeader.Status = SD.OrderStatusSubmitted;
                }

                _unitOfWork.OrderHeaderRepository.Add(orderHeader);

                // Save order header
                _unitOfWork.Save();

                foreach (var item in shoppingCartItems)
                {
                    _unitOfWork.OrderDetailRepository.Add(new OrderDetail
                    {
                        MenuItemId = item.MenuItemId,
                        OrderId = orderHeader.Id,
                        Description = item.MenuItem.Description,
                        Name = item.MenuItem.Name,
                        Price = item.MenuItem.Price,
                        Quantity = item.Quantity
                    });
                }

                // Remove shopping cart items
                _unitOfWork.ShoppingCartRepository.RemoveRange(shoppingCartItems);

                // Save changes
                _unitOfWork.Save();

                return StatusCode(StatusCodes.Status201Created, new { OrderId = orderHeader.Id, Message = "Order created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[HttpPost(save)]: {ex}");

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
