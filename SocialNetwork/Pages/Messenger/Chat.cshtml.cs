using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Controllers;
using SocialNetwork.Models;
using SocialNetwork.Service.DataBase;
using SocialNetwork.Service.RedirectManager;
using System.Data;

namespace SocialNetwork.Pages.Messenger
{
	[Authorize(Roles = "authorization")]
	public class ChatModel : PageModel
	{
		public User _User { get; set; } = new User();

		public string IdChat { get; set; } = "";
		public bool IsSelf { get; set; } = false;

		public IActionResult OnGet(string id)
		{
			if (string.IsNullOrEmpty(id))
				return null;
			try
			{
				_User = DataBaseProvider.CreateProvider(DataBaseProvider.DataBase.SqlGetUser(User.FindFirst("id").Value.ToString())).GetUser();
				if (id == "-1")
				{
					IdChat = DataBaseProvider.GetIdSelfChat(_User.Id);
					IsSelf = true;
				}
				else
				{
					if (DataBaseProvider.UserInChat(id, _User.Id))
					{
						IdChat = id;
						
					}
					else
					{
						return Redirect(Manager.PathToMainPage);
					}
				}
			}
			catch { }
			return null;
		}
	}
}
