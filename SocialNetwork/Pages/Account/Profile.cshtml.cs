using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialNetwork.Utilities.Logging;
using System.Data;

namespace SocialNetwork.Pages.Account
{
	[Authorize(Roles = "authorization")]
	public class ProfileModel : PageModel
	{
		public string Name_1 { get; set; }
		public string Name_2 { get; set; }
		public string Name_3 { get; set; }
		public string ID { get; set; } = "";
		public bool NotFound { get; set; } = false;
		public void OnGet(string id)
		{
			ID = id;
			if (string.IsNullOrEmpty(ID))
			{
				ID = User.FindFirst("id").Value.ToString();
			}
			try
			{
				var user = Service.DataBase.DataBaseProvider.CreateProvider(Service.DataBase.DataBaseProvider.DataBase.SqlGetUser(ID));
				if (user.IsNullOrEmpty() == false)
				{
					Name_1 = user.GetValueFromColumn("username_1");
					Name_2 = user.GetValueFromColumn("username_2");
					Name_3 = user.GetValueFromColumn("username_3");
					return;
				}
				NotFound = true;
			}
			catch (Exception ex) { Log.WriteLine(ex); NotFound = true; }
			NotFound = true;
		}
	}
}
