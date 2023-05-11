using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Models;
using SocialNetwork.Service.DataBase;
using System.Data;

namespace SocialNetwork.Pages.Messenger
{
	[Authorize(Roles = "authorization")]
	public class ChatModel : PageModel
    {
		public User _User { get; set; } = new User();

		public Chat Chat = new Chat();
        public void OnGet(string id)
        {
			_User = DataBaseProvider.CreateProvider(DataBaseProvider.DataBase.SqlGetUser(User.FindFirst("id").Value.ToString())).GetUser();
			if (id == "-1")
			{
				Console.WriteLine("ASDASD");
				Chat = DataBaseProvider.GetChat(DataBaseProvider.GetIdSelfChat(_User.Id));
			}
        }
    }
}
