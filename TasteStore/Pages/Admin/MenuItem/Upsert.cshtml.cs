using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models.ViewModels;

namespace TasteStore.Pages.Admin.MenuItem
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UpsertModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty(SupportsGet = true)]
        public MenuItemViewModel ViewModel { get; set; }

        public IActionResult OnGet(int? id)
        {
            ViewModel = new MenuItemViewModel
            {
                MenuItem = new Models.MenuItem(),
                CategoryItems = _unitOfWork.CategoryRepository.GetCategoryListForDropDown(),
                FoodTypeItems = _unitOfWork.FoodTypeRepository.GetFoodTypeListForDropDown()
            };

            if (id.HasValue)
            {
                ViewModel.MenuItem = _unitOfWork.MenuItemRepository.GetFirstOrDefault(c => c.Id == id);
                if (ViewModel.MenuItem == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            string webrootPath = _hostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ViewModel.MenuItem.Id == 0)
            {
                var fileName = UploadFile(webrootPath, files);

                ViewModel.MenuItem.Image = fileName;

                _unitOfWork.MenuItemRepository.Add(ViewModel.MenuItem);
            }
            else
            {
                var menuItem = _unitOfWork.MenuItemRepository.Get(ViewModel.MenuItem.Id);
                if (files.Count > 0)
                {
                    string imagePath = Path.Combine(webrootPath, "images", "menuItems", menuItem.Image);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    var fileName = UploadFile(webrootPath, files);

                    ViewModel.MenuItem.Image = fileName;
                }
                else
                {
                    ViewModel.MenuItem.Image = menuItem.Image;
                }

                _unitOfWork.MenuItemRepository.Update(ViewModel.MenuItem);
            }

            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }

        private string UploadFile(string webrootPath, IFormFileCollection files)
        {
            string uploadPath = Path.Combine(webrootPath, "images", "menuItems");
            string extension = Path.GetExtension(files[0].FileName);
            string fileName = Guid.NewGuid() + extension;

            using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
            {
                files[0].CopyTo(fileStream);
            }

            return fileName;
        }
    }
}
