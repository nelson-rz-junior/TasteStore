using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Utility;

namespace TasteStore.Pages.Admin.FoodType
{
    [Authorize(Roles = SD.ManageRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty(SupportsGet = true)]
        public Models.FoodType FoodType { get; set; }

        public IActionResult OnGet(int? id)
        {
            FoodType = new Models.FoodType();

            if (id.HasValue)
            {
                FoodType = _unitOfWork.FoodTypeRepository.GetFirstOrDefault(ft => ft.Id == id);
                if (FoodType == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (FoodType.Id != 0)
                {
                    _unitOfWork.FoodTypeRepository.Update(FoodType);
                }
                else
                {
                    _unitOfWork.FoodTypeRepository.Add(FoodType);
                }

                _unitOfWork.Save();

                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
