using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Models.ViewModels;

namespace TasteStore.Pages.Customer.Cart
{
    [Authorize]
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                DetailCart.OrderHeader.PickupDate = DateTime.Now;
                DetailCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            }

            return Page();
        }
    }
}