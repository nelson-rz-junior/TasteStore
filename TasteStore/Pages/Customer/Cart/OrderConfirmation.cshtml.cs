using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;

namespace TasteStore.Pages.Customer.Cart
{
    [Authorize]
    public class OrderConfirmationModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderConfirmationModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderHeader OrderHeader { get; set; }

        public IActionResult OnGet(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(oh => oh.UserId == claims.Value && oh.Id == id);
            if (OrderHeader == null)
            {
                return RedirectToPage("/Customer/Cart/Index");
            }

            return Page();
        }
    }
}
