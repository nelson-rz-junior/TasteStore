using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TasteStore.DataAccess.Data.Repository.IRepository;
using TasteStore.Models;

namespace TasteStore.DataAccess.Data.Repository
{
    public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(MenuItem menuItem)
        {
            var currentMenuItem = _context.MenuItems.FirstOrDefault(mi => mi.Id == menuItem.Id);
            if (currentMenuItem != null)
            {
                currentMenuItem.Name = menuItem.Name;
                currentMenuItem.Description = menuItem.Description;
                currentMenuItem.Price = menuItem.Price;
                currentMenuItem.CategoryId = menuItem.CategoryId;
                currentMenuItem.FoodTypeId = menuItem.FoodTypeId;

                if (menuItem.Image != "")
                {
                    currentMenuItem.Image = menuItem.Image;
                }

                _context.SaveChanges();
            }
        }
    }
}
