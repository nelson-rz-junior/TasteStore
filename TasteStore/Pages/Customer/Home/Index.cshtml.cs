using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Utility;

namespace TasteStore.Pages.Customer.Home
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<MenuItem> MenuItems { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public void OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var totalItems = _unitOfWork.ShoppingCartRepository.GetAll(sc => sc.ApplicationUserId == claim.Value)
                    .Sum(s => s.Quantity);

                HttpContext.Session.SetInt32(SD.ShoppingCart, totalItems);
            }

            MenuItems = _unitOfWork.MenuItemRepository.GetAll(null, null, "Category,FoodType");
            Categories = _unitOfWork.CategoryRepository.GetAll(null, q => q.OrderBy(c => c.DisplayOrder), null);
        }
    }
}
