using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteStore.DataAccess.Data.Repository.Interfaces;

namespace TasteStore.Pages.Admin.Category
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty(SupportsGet = true)]
        public Models.Category Category { get; set; }

        public IActionResult OnGet(int? id)
        {
            Category = new Models.Category();

            if (id.HasValue)
            {
                Category = _unitOfWork.CategoryRepository.GetFirstOrDefault(c => c.Id == id);
                if (Category == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Category.Id == 0)
            {
                _unitOfWork.CategoryRepository.Add(Category);
            }
            else
            {
                _unitOfWork.CategoryRepository.Update(Category);
            }

            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }
    }
}
