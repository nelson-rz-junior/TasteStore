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

        [HttpPost("save")]
        public async Task<JsonResult> Post([FromBody] SummaryOrder summaryOrder)
        {
            try
            {
                _logger.LogInformation($"[HttpPost(save)] summaryOrder: {JsonConvert.SerializeObject(summaryOrder)}");

                var user = await _userManager.FindByIdAsync(summaryOrder.UserId);
                if (user == null)
                {
                    return new JsonResult(new { Success = false, OrderId = 0, Message = "Invalid user" });
                }

                var shoppingCartItems = _unitOfWork.ShoppingCartRepository.GetAll(sc => sc.ApplicationUserId == user.Id, includeProperties: "MenuItem")
                    .ToList();

                if (shoppingCartItems.Count == 0)
                {
                    return new JsonResult(new { Success = false, OrderId = 0, Message = "Empty shopping cart" });
                }

                OrderHeader orderHeader = new OrderHeader
                {
                    UserId = user.Id,
                    PickupName = summaryOrder.PickupName,
                    PhoneNumber = summaryOrder.PickupPhoneNumber,
                    PickupDate = summaryOrder.PickupDate,
                    PickupTime = summaryOrder.PickupTime,
                    Comments = summaryOrder.PickupComments,
                    OrderDate = DateTime.Now,
                    OrderTotal = shoppingCartItems.Sum(sc => sc.MenuItem.Price * sc.Quantity),
                    PaymentStatus = SD.PaymentStatusPending,
                    Status = SD.OrderStatusPlaced
                };

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

                return new JsonResult(new { Success = true, OrderId = orderHeader.Id, Message = "Order created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[HttpPost(save)]: {ex}");
                return new JsonResult(new { Success = false, OrderId = 0, Message = "An error occurred while creating an order" });
            }
        }

        [HttpPost("stripe/createsession")]
        public async Task<JsonResult> Post([FromBody] StripeSession stripeSession)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(stripeSession.UserId);
                if (user == null)
                {
                    return new JsonResult(new { Success = false, Message = "Invalid user" });
                }

                var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.UserId == stripeSession.UserId && oh.Id == stripeSession.OrderId);
                if (orderHeader == null)
                {
                    return new JsonResult(new { Success = false, Message = "Invalid order" });
                }

                var lineItems = new List<SessionLineItemOptions>();

                var orderDetailItems = _unitOfWork.OrderDetailRepository.GetAll(filter: od => od.OrderId == orderHeader.Id, includeProperties: "MenuItem")
                    .ToList();

                if (orderDetailItems.Count == 0)
                {
                    return new JsonResult(new { Success = false, Message = "Empty order list" });
                }

                orderDetailItems.ForEach(o =>
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        Name = o.MenuItem.Name,
                        Description = o.MenuItem.Description,
                        Currency = "brl",
                        Quantity = o.Quantity,
                        Amount = (long)(o.MenuItem.Price * 100),
                        Images = new List<string> { $"{_options.ImagesBaseUrl}/{o.MenuItem.Image}" }
                    });
                });

                var options = new SessionCreateOptions
                {
                    CustomerEmail = user.Email,
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    Metadata = new Dictionary<string, string> { 
                        { "OrderId", orderHeader.Id.ToString() },
                        { "UserId", user.Id }
                    },
                    SuccessUrl = $"{_options.SuccessUrl}?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = _options.CancelUrl
                };

                var service = new SessionService();
                Session session = service.Create(options);

                _logger.LogInformation($"[HttpPost(stripe/createsession)] options: {JsonConvert.SerializeObject(options)}");

                return new JsonResult(new { Success = true, SessionId = session.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[HttpPost(stripe/createsession)]: {ex}");
                return new JsonResult(new { Success = false, Message = "An error occurred while creating a session" });
            }
        }
    }
}
