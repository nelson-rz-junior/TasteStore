using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Models.ViewModels;
using TasteStore.Utility;

namespace TasteStore.Pages.Customer.Cart
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public OrderDetailCartViewModel OrderDetailCartViewModel { get; set; }

        public void OnGet()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailCartViewModel = new OrderDetailCartViewModel
            {
                OrderHeader = new OrderHeader()
            };

            OrderDetailCartViewModel.ShoppingCartItems = _unitOfWork.ShoppingCartRepository
                .GetAll(filter: sc => sc.ApplicationUserId == claims.Value, includeProperties: "MenuItem")
                .ToList();

            if (OrderDetailCartViewModel.ShoppingCartItems != null)
            {
                OrderDetailCartViewModel.OrderHeader.OrderTotal = OrderDetailCartViewModel.ShoppingCartItems
                    .Sum(sc => sc.MenuItem.Price * sc.Quantity);

                int totalItems = OrderDetailCartViewModel.ShoppingCartItems.Sum(sc => sc.Quantity);
                if (totalItems == 0)
                {
                    HttpContext.Session.Remove(SD.ShoppingCart);
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.ShoppingCart, totalItems);
                }
            }
        }

        public IActionResult OnPostPlus(int cartId)
        {
            var item = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(sc => sc.Id == cartId);

            _unitOfWork.ShoppingCartRepository.IncrementQuantity(item, 1);
            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostMinus(int cartId)
        {
            var item = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(sc => sc.Id == cartId);
            if (item.Quantity == 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(item);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.DecrementQuantity(item, 1);
                _unitOfWork.Save();
            }

            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostRemove(int cartId)
        {
            _unitOfWork.ShoppingCartRepository.Remove(cartId);
            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/Index");
        }
    }
}
