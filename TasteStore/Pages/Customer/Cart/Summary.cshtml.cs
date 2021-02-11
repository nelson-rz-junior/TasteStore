using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Stripe;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Models.ViewModels;
using TasteStore.Utility;

namespace TasteStore.Pages.Customer.Cart
{
    [Authorize]
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork, IOptions<StripeSettings> options)
        {
            _unitOfWork = unitOfWork;
            StripeConfiguration.ApiKey = options.Value.SecretKey;
        }

        [BindProperty]
        public OrderDetailCartViewModel DetailCart { get; set; }

        public IActionResult OnGet()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            DetailCart = new OrderDetailCartViewModel
            {
                OrderHeader = new OrderHeader()
            };

            DetailCart.ShoppingCartItems = _unitOfWork.ShoppingCartRepository
                .GetAll(filter: sc => sc.ApplicationUserId == claims.Value, includeProperties: "MenuItem")
                .ToList();

            if (DetailCart.ShoppingCartItems != null)
            {
                DetailCart.OrderHeader.OrderTotal = DetailCart.ShoppingCartItems
                    .Sum(sc => sc.MenuItem.Price * sc.Quantity);

                ApplicationUser applicationUser = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(u => u.Id == claims.Value);
                DetailCart.OrderHeader.PickupName = applicationUser.FullName;
                DetailCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            }

            return Page();
        }

        public IActionResult OnPost(string stripeToken)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            
            var nameClaim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = claimIdentity.FindFirst(ClaimTypes.Name);

            DetailCart.ShoppingCartItems = _unitOfWork.ShoppingCartRepository.GetAll(sc => sc.ApplicationUserId == nameClaim.Value, includeProperties: "MenuItem")
                .ToList();

            DetailCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            DetailCart.OrderHeader.OrderDate = DateTime.Now;
            DetailCart.OrderHeader.UserId = nameClaim.Value;
            DetailCart.OrderHeader.Status = SD.PaymentStatusPending;
            DetailCart.OrderHeader.PickupTime = DetailCart.OrderHeader.PickupDate.Add(DetailCart.OrderHeader.PickupTime.TimeOfDay);
            DetailCart.OrderHeader.OrderTotal = DetailCart.ShoppingCartItems.Sum(sc => sc.MenuItem.Price * sc.Quantity);

            _unitOfWork.OrderHeaderRepository.Add(DetailCart.OrderHeader);

            // Save order header
            _unitOfWork.Save();

            foreach (var item in DetailCart.ShoppingCartItems)
            {
                _unitOfWork.OrderDetailRepository.Add(new OrderDetail
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = DetailCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Quantity = item.Quantity
                });
            }

            _unitOfWork.ShoppingCartRepository.RemoveRange(DetailCart.ShoppingCartItems);

            // Save order details
            _unitOfWork.Save();

            // Reset session
            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);

            if (stripeToken != null)
            {
                // Strip charge
                var customerOptions = new CustomerCreateOptions
                {
                    Email = emailClaim.Value,
                    Source = stripeToken,
                };

                var customerService = new CustomerService();
                Stripe.Customer customer = customerService.Create(customerOptions);

                var chargeOptions = new ChargeCreateOptions
                {
                    Customer = customer.Id,
                    Description = $"Order Id: {DetailCart.OrderHeader.Id}",
                    Amount = Convert.ToInt32(DetailCart.OrderHeader.OrderTotal * 100), // cents
                    Currency = "usd",
                };

                var chargeService = new ChargeService();
                Charge charge = chargeService.Create(chargeOptions);

                DetailCart.OrderHeader.TransactionId = charge.Id;

                if (charge.Status.Equals("succeeded", StringComparison.OrdinalIgnoreCase))
                {
                    // Send e-mail
                    DetailCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    DetailCart.OrderHeader.Status = SD.StatusSubmitted;
                }
                else
                {
                    DetailCart.OrderHeader.PaymentStatus = SD.PaymentRejected;
                }
            }
            else
            {
                DetailCart.OrderHeader.PaymentStatus = SD.PaymentRejected;
            }

            // Update order header status
            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = DetailCart.OrderHeader.Id });
        }
    }
}
