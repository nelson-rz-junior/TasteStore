using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TasteStore.Pages.Customer.Cart
{
    [Authorize]
    public class CheckoutCancelModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
