using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Models;
using SocialNetwork.Service.DataBase;
using System.Data;
using System.Diagnostics;

namespace SocialNetwork.Pages.Friends
{
	[Authorize(Roles = "authorization")]
	public class FindModel : PageModel
	{
		public List<User> Users { get; set; } = new List<User>();
		public void OnGet()
		{
			Users = DataBaseProvider.GetAllUsers();
		}
	}
}
