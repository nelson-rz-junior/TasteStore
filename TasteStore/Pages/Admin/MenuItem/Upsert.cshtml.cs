using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models.ViewModels;
using TasteStore.Utility;

namespace TasteStore.Pages.Admin.MenuItem
{
    [Authorize(Roles = SD.ManageRole)]
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

        [BindProperty]
        public IFormFileCollection UploadImage { get; set; }

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

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ViewModel.MenuItem.Id == 0)
            {
                var fileName = UploadFile(webrootPath, UploadImage);

                ViewModel.MenuItem.Image = fileName;

                _unitOfWork.MenuItemRepository.Add(ViewModel.MenuItem);
            }
            else
            {
                var menuItem = _unitOfWork.MenuItemRepository.Get(ViewModel.MenuItem.Id);
                if (UploadImage.Count > 0)
                {
                    string imagePath = Path.Combine(webrootPath, "images", "menuItems", menuItem.Image);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    var fileName = UploadFile(webrootPath, UploadImage);

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
