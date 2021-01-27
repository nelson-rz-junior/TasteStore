using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using TasteStore.Models;

namespace TasteStore.DataAccess.Data.Repository.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        IEnumerable<SelectListItem> GetCategoryListForDropDown();

        void Update(Category category);
    }
}
