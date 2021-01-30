using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;

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
            MenuItems = _unitOfWork.MenuItemRepository.GetAll(null, null, "Category,FoodType");
            Categories = _unitOfWork.CategoryRepository.GetAll(null, q => q.OrderBy(c => c.DisplayOrder), null);
        }
    }
}
