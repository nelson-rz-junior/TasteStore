using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe.Checkout;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Utility;

namespace TasteStore.Pages.Customer.Cart
{
    [Authorize]
    public class PaymentSuccessModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentSuccessModel> _logger;

        public PaymentSuccessModel(IUnitOfWork unitOfWork, ILogger<PaymentSuccessModel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult OnGet(string sessionId)
        {
            int orderId = 0;

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                var sessionService = new SessionService();
                Session session = sessionService.Get(sessionId);

                _logger.LogInformation($"PaymentSuccess: {JsonConvert.SerializeObject(session)}");

                if (int.TryParse(session.Metadata["OrderId"], out orderId))
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                    var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.UserId == claims.Value && oh.Id == orderId);
                    if (orderHeader != null)
                    {
                        orderHeader.CheckoutPaymentStatus = session.PaymentStatus;
                        if (orderHeader.CheckoutPaymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase))
                        {
                            orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                            orderHeader.Status = SD.OrderStatusSubmitted;
                        }
                        else
                        {
                            orderHeader.PaymentStatus = SD.PaymentRejected;
                        }

                        orderHeader.PaymentMethodTypes = string.Join(",", session.PaymentMethodTypes);
                        orderHeader.PaymentIntentId = session.PaymentIntentId;

                        _unitOfWork.Save();

                        HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
                    }
                }
            }

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = orderId });
        }
    }
}
