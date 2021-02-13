using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe.Checkout;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Utility;

namespace TasteStore.Pages.Customer.Cart
{
    public class PaymentSuccessModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentSuccessModel(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet(string sessionId)
        {
            int orderId = 0;

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                var sessionService = new SessionService();
                Session session = sessionService.Get(sessionId);

                if (int.TryParse(session.Metadata["OrderId"], out orderId))
                {
                    var user = await _userManager.FindByIdAsync(session.Metadata["UserId"]);
                    if (user != null)
                    {
                        var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.UserId == user.Id && oh.Id == orderId);
                        if (orderHeader != null)
                        {
                            orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                            orderHeader.Status = SD.OrderStatusSubmitted;
                            orderHeader.TransactionId = session.PaymentIntentId;
                        }
                        else
                        {
                            orderHeader.PaymentStatus = SD.PaymentRejected;
                        }

                        _unitOfWork.Save();

                        HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
                    }
                }
            }

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = orderId });
        }
    }
}
