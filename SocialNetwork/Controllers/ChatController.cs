using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Service.DataBase;
using SocialNetwork.Service.RedirectManager;
using System.Data;

namespace SocialNetwork.Controllers
{
	[Authorize(Roles = "authorization")]
	public class ChatController : Controller
	{
		[Route("messenger/chat/messages")]
		[HttpGet]
		public async Task<JsonResult> GetMessages(string id_chat)
		{
			try
			{
				var user = User.FindFirst("id").Value.ToString();
				if (DataBaseProvider.UserInChat(id_chat, user) == false)
					return null;

				return Json(DataBaseProvider.GetMessages(id_chat));
			}
			catch { }
			return null;
		}
	}
}
