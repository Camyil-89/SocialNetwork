using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Service.DataBase;
using SocialNetwork.Service.RedirectManager;
using System.Data;

namespace SocialNetwork.Controllers
{
	[Authorize(Roles = "authorization")]
	public class UserController : Controller
	{
		[Route("account/profile/user")]
		[HttpGet]
		public async Task<JsonResult> GetUser(string id_user)
		{
			try
			{
				return Json(DataBaseProvider.CreateProvider(DataBaseProvider.DataBase.SqlGetUser(id_user)).GetUser());
			}
			catch { }
			return null;
		}
	}
}
