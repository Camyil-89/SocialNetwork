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
			try
			{
				var user_admin = User.FindFirst("id").Value.ToString();

				var chats = DataBaseProvider.GetMyChats(user_admin);
				var ids_chat = DataBaseProvider.UserInChats(chats, id_user, "chat");
				if (ids_chat.Count > 0)
					return Redirect($"{Manager.PathToChat}?id={ids_chat[0]}");
				return Redirect($"{Manager.PathToChat}?id={DataBaseProvider.CreateChat(user_admin, id_user, "chat")}");
			}
			catch { }
			return Redirect(Manager.PathToMainPage);
		}
	}
}
