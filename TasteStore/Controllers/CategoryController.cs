using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Utility;

namespace TasteStore.Controllers
{
    [Authorize(Roles = SD.ManageRole)]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(new 
            { 
                data = _unitOfWork.DapperRepository.GetAll<Category>("GetAllCategories")
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _unitOfWork.CategoryRepository.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return new JsonResult(new 
                { 
                    success = false, 
                    message = "Error while deleting" 
                });
            }

            _unitOfWork.CategoryRepository.Remove(category);
            _unitOfWork.Save();

            return new JsonResult(new 
            { 
                success = true, 
                message = "Delete successful" 
            });
        }
    }
}
