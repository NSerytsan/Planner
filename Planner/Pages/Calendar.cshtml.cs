using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Planner.Pages
{
    [Authorize]
    public class CalendarModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
