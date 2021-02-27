using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasteStore.DataAccess.Data.Repository.Interfaces;

namespace TasteStore.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FoodTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FoodTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(new
            {
                data = _unitOfWork.FoodTypeRepository.GetAll()
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var foodType = _unitOfWork.FoodTypeRepository.GetFirstOrDefault(ft => ft.Id == id);
            if (foodType == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Error while deleting"
                });
            }

            _unitOfWork.FoodTypeRepository.Remove(foodType);
            _unitOfWork.Save();

            return new JsonResult(new
            {
                success = true,
                message = "Delete successful"
            });
        }
    }
}
