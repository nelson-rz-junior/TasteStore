using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Utility;

namespace TasteStore.Pages.Customer.Home
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public ShoppingCart ShoppingCart { get; set; }

        public void OnGet(int id)
        {
            ShoppingCart = new ShoppingCart
            {
                MenuItem = _unitOfWork.MenuItemRepository.GetFirstOrDefault(includeProperties: "Category,FoodType", filter: mi => mi.Id == id),
                MenuItemId = id
            };
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCart.ApplicationUserId = claim.Value;

                ShoppingCart currentShoppingCart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(sc => sc.ApplicationUserId == ShoppingCart.ApplicationUserId &&
                    sc.MenuItemId == ShoppingCart.MenuItemId);

                if (currentShoppingCart == null)
                {
                    _unitOfWork.ShoppingCartRepository.Add(ShoppingCart);
                }
                else
                {
                    _unitOfWork.ShoppingCartRepository.IncrementQuantity(currentShoppingCart, ShoppingCart.Quantity);
                }

                _unitOfWork.Save();

                var totalItems = _unitOfWork.ShoppingCartRepository.GetAll(sc => sc.ApplicationUserId == ShoppingCart.ApplicationUserId)
                    .Sum(s => s.Quantity);

                HttpContext.Session.SetInt32(SD.ShoppingCart, totalItems);

                return RedirectToPage("Index");
            }
            else
            {
                ShoppingCart.MenuItem = _unitOfWork.MenuItemRepository
                    .GetFirstOrDefault(includeProperties: "Category,FoodType", filter: mi => mi.Id == ShoppingCart.MenuItemId);

                return Page();
            }
        }
    }
}
