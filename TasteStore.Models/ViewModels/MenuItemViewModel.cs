using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace TasteStore.Models.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }

        public IEnumerable<SelectListItem> CategoryItems { get; set; }

        public IEnumerable<SelectListItem> FoodTypeItems { get; set; }
    }
}
