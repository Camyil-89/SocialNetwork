using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using SocialNetwork.Utilities.Logging;

namespace SocialNetwork.Pages.Account
{
	public class ProfileSettingsModel : PageModel
	{
		public string ErrorMessageUserName { get; set; } = "";
		public string ErrorMessagePassword { get; set; } = "";
		public void OnGet()
		{
		}

		[HttpPost]
		public async Task<IActionResult> OnPostChangePassword(string inputPassword, string inputConfirmPassword)
		{
			try
			{
				MySqlCommand command = new MySqlCommand($"UPDATE `users` SET `password` = @pass WHERE `users`.`id` = @id;");
				command.Parameters.AddWithValue("@pass", Utilities.Cryptography.Checksum.Compute(inputPassword));
				command.Parameters.AddWithValue("@id", User.FindFirst("id").Value);
				Service.DataBase.DataBaseProvider.DataBase.SqlQuery(command);
				Pages.Login.LoginModel.UpdateClaim(User.FindFirst("id").Value, HttpContext);
			}
			catch (Exception ex) { Log.WriteLine(ex, Utilities.Logging.LogLevel.Error); ErrorMessagePassword = "Не удалось изменть пароль!"; return null; }
			return Redirect(Service.RedirectManager.Manager.PathToSettingsProfile);
		}
		[HttpPost]
		public async Task<IActionResult> OnPostChangeName(string username_1, string username_2, string username_3)
		{
			//UPDATE `users` SET `login` = 'asd1', `password` = 'asd1', `username_1` = 'asd1', `username_2` = 'asd1', `username_3` = 'asd1' WHERE `users`.`id` = 2;
			Console.WriteLine($"{username_1};{username_2};{username_3}|{string.Join(";", Request.Form)}");
			try
			{
				MySqlCommand command = new MySqlCommand($"UPDATE `users` SET `username_1` = @name_1, `username_2` = @name_2, `username_3` = @name_3 WHERE `users`.`id` = @id;");
				command.Parameters.AddWithValue("@name_1", username_1);
				command.Parameters.AddWithValue("@name_2", username_2);
				command.Parameters.AddWithValue("@name_3", username_3);
				command.Parameters.AddWithValue("@id", User.FindFirst("id").Value);
				Service.DataBase.DataBaseProvider.DataBase.SqlQuery(command);
				Pages.Login.LoginModel.UpdateClaim(User.FindFirst("id").Value, HttpContext);
			}
			catch (Exception ex) { Log.WriteLine(ex, Utilities.Logging.LogLevel.Error); ErrorMessageUserName = "Не удалось изменить!"; return null; }
			return Redirect(Service.RedirectManager.Manager.PathToSettingsProfile);
		}
	}
}
