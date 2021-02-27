using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Stripe;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Models.ViewModels;
using TasteStore.Utility;

namespace TasteStore.Pages.Admin.Order
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeSettings _options;

        public DetailsModel(IUnitOfWork unitOfWork, IOptions<StripeSettings> options)
        {
            _unitOfWork = unitOfWork;
            _options = options.Value;

            StripeConfiguration.ApiKey = _options.SecretKey;
        }

        public OrderPickupViewModel OrderPickup { get; set; }

        public void OnGet(int orderId)
        {
            OrderPickup = new OrderPickupViewModel
            {
                OrderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.Id == orderId, "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetailRepository.GetAll(od => od.OrderId == orderId).ToList()
            };
        }

        public IActionResult OnPostConfirmPickup(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.Status = SD.OrderStatusCompleted;
                _unitOfWork.Save();
            }

            return RedirectToPage("Pickup");
        }

        public IActionResult OnPostCancelOrder(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.Status = SD.OrderStatusCancelled;
                _unitOfWork.Save();
            }

            return RedirectToPage("Pickup");
        }

        public IActionResult OnPostRefundOrder(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.Id == orderId);
            if (orderHeader != null)
            {
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = orderHeader.PaymentIntentId,
                    Reason = RefundReasons.RequestedByCustomer,
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100)
                };

                var refunds = new RefundService();
                var refund = refunds.Create(refundOptions);

                orderHeader.Status = SD.OrderStatusRefunded;
                _unitOfWork.Save();
            }

            return RedirectToPage("Pickup");
        }
    }
}
