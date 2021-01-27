using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;

namespace TasteStore.DataAccess.Data.Repository.IRepository
{
    public interface IFoodTypeRepository : IRepository<FoodType>
    {
        IEnumerable<SelectListItem> GetFoodTypeListForDropDown();

        void Update(FoodType foodType);
    }
}
