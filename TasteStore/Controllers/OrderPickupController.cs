using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Models.ViewModels;
using TasteStore.Utility;

namespace TasteStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderPickupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderPickupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get(string status)
        {
            status ??= "";

            List<OrderPickupViewModel> orderDetails = new List<OrderPickupViewModel>();

            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.CustomerRole))
            {
                // Retrieve all orders for that customer
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(oh => oh.UserId == claim.Value, null, "ApplicationUser");
            }
            else
            {
                orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(null, null, "ApplicationUser");
            }

            if (status.Equals("cancelled", System.StringComparison.OrdinalIgnoreCase))
            {
                orderHeaders = orderHeaders.Where(oh => oh.Status == SD.OrderStatusCancelled || oh.Status == SD.OrderStatusRefunded ||
                    oh.Status == SD.PaymentRejected);
            }
            else if (status.Equals("completed", System.StringComparison.OrdinalIgnoreCase))
            {
                orderHeaders = orderHeaders.Where(oh => oh.Status == SD.OrderStatusCompleted);
            }
            else
            {
                orderHeaders = orderHeaders.Where(oh => oh.Status == SD.OrderStatusReady || oh.Status == SD.OrderStatusInProcess ||
                    oh.Status == SD.OrderStatusSubmitted || oh.Status == SD.PaymentStatusPending);
            }

            foreach (OrderHeader item in orderHeaders)
            {
                orderDetails.Add(new OrderPickupViewModel
                {
                    OrderHeader = item,
                    OrderDetails = _unitOfWork.OrderDetailRepository.GetAll(od => od.OrderId == item.Id).ToList()
                });
            }

            return new JsonResult(new 
            { 
                data = orderDetails 
            });
        }
    }
}
