using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TasteStore.Pages.Admin.Order
{
    [Authorize]
    public class PickupModel : PageModel
    {
        public string Status { get; set; }

        public void OnGet(string status)
        {
            Status = status;
        }
    }
}
