using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Service.DataBase;
using SocialNetwork.Service.RedirectManager;
using System.Data;

namespace SocialNetwork.Controllers
{
	[Authorize(Roles = "authorization")]
	public class NewChatController : Controller
	{
		[Route("messenger/chat/new")]
		[HttpGet]
		public async Task<IActionResult> OnCreateNewChat(string id_user, string type)
		{
			var chats = DataBaseProvider.GetMyChats(User.FindFirst("id").Value.ToString());
			DataBaseProvider.UserInChats(chats, User.FindFirst("id").Value.ToString(), "self");
			return Redirect($"{Manager.PathToChat}?id={id_user}");
		}
	}
}
