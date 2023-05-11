using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Models;
using SocialNetwork.Service.DataBase;
using SocialNetwork.Utilities.Logging;
using System.Data;

namespace SocialNetwork.Pages.Account
{
	[Authorize(Roles = "authorization")]
	public class ProfileModel : PageModel
	{
		public User _User = new User();
		public bool NotFound { get; set; } = false;
		public void OnGet(string id)
		{
			_User.Id = id;
			if (string.IsNullOrEmpty(_User.Id))
			{
				_User.Id = User.FindFirst("id").Value.ToString();
			}
			try
			{
				var user = Service.DataBase.DataBaseProvider.CreateProvider(Service.DataBase.DataBaseProvider.DataBase.SqlGetUser(_User.Id));
				if (user.IsNullOrEmpty() == false)
				{
					_User.Name_1 = user.GetValueFromColumn("username_1");
					_User.Name_2 = user.GetValueFromColumn("username_2");
					_User.Name_3 = user.GetValueFromColumn("username_3");
					_User.ImageUrl = user.GetBase64Image();
					return;
				}
				NotFound = true;
			}
			catch (Exception ex) { Log.WriteLine(ex); NotFound = true; }
			NotFound = true;
		}
	}
}
