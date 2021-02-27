using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Models.ViewModels;
using TasteStore.Utility;

namespace TasteStore.Pages.Admin.Order
{
    [Authorize]
    public class ManageModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeSettings _options;

        public ManageModel(IUnitOfWork unitOfWork, IOptions<StripeSettings> options)
        {
            _unitOfWork = unitOfWork;
            _options = options.Value;

            StripeConfiguration.ApiKey = _options.SecretKey;
        }

        [BindProperty]
        public List<OrderPickupViewModel> OrderPickup { get; set; }

        public void OnGet()
        {
            OrderPickup = new List<OrderPickupViewModel>();

            List<OrderHeader> orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(oh => oh.Status == SD.OrderStatusSubmitted || oh.Status == SD.OrderStatusInProcess)
                .OrderBy(o => o.PickupDate)
                .ThenBy(o => o.PickupTime)
                .ToList();

            foreach (OrderHeader item in orderHeaders)
            {
                OrderPickup.Add(new OrderPickupViewModel
                {
                    OrderHeader = item,
                    OrderDetails = _unitOfWork.OrderDetailRepository.GetAll(od => od.OrderId == item.Id).ToList()
                });
            }
        }

        public IActionResult OnPostStartCooking(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.Status = SD.OrderStatusInProcess;
                _unitOfWork.Save();
            }

            return RedirectToPage("Manage");
        }

        public IActionResult OnPostOrderReady(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.Status = SD.OrderStatusReady;
                _unitOfWork.Save();
            }

            return RedirectToPage("Manage");
        }

        public IActionResult OnPostCancelOrder(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.Status = SD.OrderStatusCancelled;
                _unitOfWork.Save();
            }

            return RedirectToPage("Manage");
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

            return RedirectToPage("Manage");
        }
    }
}
