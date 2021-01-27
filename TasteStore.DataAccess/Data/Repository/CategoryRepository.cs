using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using System.Linq;

namespace TasteStore.DataAccess.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetCategoryListForDropDown()
        {
            return _context.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }

        public void Update(Category category)
        {
            var currentCategory = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (currentCategory != null)
            {
                currentCategory.Name = category.Name;
                currentCategory.DisplayOrder = category.DisplayOrder;

                _context.SaveChanges();
            }
        }
    }
}
