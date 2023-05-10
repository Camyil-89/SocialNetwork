using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Service.RedirectManager;

namespace SocialNetwork.Pages
{
    public class IndexModel : PageModel
    {

		public async Task<IActionResult> OnGet()
		{
			return Redirect(Manager.PathToMainPage);
        }
    }
}
