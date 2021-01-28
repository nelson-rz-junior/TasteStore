using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using TasteStore.DataAccess.Data.Repository.Interfaces;

namespace TasteStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MenuItemController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(new
            {
                data = _unitOfWork.MenuItemRepository.GetAll(null, null, "Category,FoodType")
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var menuItem = _unitOfWork.MenuItemRepository.GetFirstOrDefault(ft => ft.Id == id);
                if (menuItem == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Error while deleting"
                    });
                }

                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", "menuItems", menuItem.Image);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _unitOfWork.MenuItemRepository.Remove(menuItem);
                _unitOfWork.Save();

                return new JsonResult(new
                {
                    success = true,
                    message = "Delete successful"
                });
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "An exception occurred while deleting"
                });
            }
        }
    }
}
