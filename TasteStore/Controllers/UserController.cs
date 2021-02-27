using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Utility;

namespace TasteStore.Controllers
{
    [Authorize(Roles = SD.ManageRole)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(new 
            { 
                data = _unitOfWork.ApplicationUserRepository.GetAll() 
            });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var user = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return new JsonResult(new 
                { 
                    success = false, 
                    message = "Error while locking/unlocking" 
                });
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }
            
            _unitOfWork.Save();

            return new JsonResult(new { success = true, message = "Operation successful" });
        }
    }
}
