using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using TasteStore.DataAccess.Data.Repository.IRepository;
using TasteStore.Models;

namespace TasteStore.DataAccess.Data.Repository
{
    public class FoodTypeRepository : Repository<FoodType>, IFoodTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public FoodTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetFoodTypeListForDropDown()
        {
            return _context.FoodTypes.Select(ft => new SelectListItem
            {
                Text = ft.Name,
                Value = ft.Id.ToString()
            });
        }

        public void Update(FoodType foodType)
        {
            var currentFoodType = _context.FoodTypes.FirstOrDefault(ft => ft.Id == foodType.Id);
            if (currentFoodType != null)
            {
                currentFoodType.Name = foodType.Name;

                _context.SaveChanges();
            }
        }
    }
}
