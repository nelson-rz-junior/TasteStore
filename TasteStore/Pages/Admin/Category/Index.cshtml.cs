using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TasteStore.Utility;

namespace TasteStore.Pages.Admin.Category
{
    [Authorize(Roles = SD.ManageRole)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
