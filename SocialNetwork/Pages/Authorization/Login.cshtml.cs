using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using SocialNetwork.Utilities.Logging;

namespace SocialNetwork.Pages.Login
{
	public class LoginModel : PageModel
	{
		public bool IsLogin { get; set; } = true;
		public string ErrorMessage { get; set; } = "";
		public async Task<IActionResult> OnGet(string type)
		{
			if (type == "logout")
			{
				IsLogin = true;
				HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				return Redirect(Service.RedirectManager.Manager.PathToLogin);
			}

			IsLogin = string.IsNullOrEmpty(type);
			return null;
		}

		[HttpPost]
		public async Task<IActionResult> OnPostRegistration(string login, string password, string username_1, string username_2, string username_3)
		{
			Console.WriteLine($"{login};{password};{username_1};{username_2};{username_3}|{string.Join(";", Request.Form)}");
			return Redirect(Service.RedirectManager.Manager.PathToMainPage);
		}

		[HttpPost]
		public async Task<IActionResult> OnPostLogin(string login, string password)
		{
			try
			{
				var result = Service.DataBase.DataBaseProvider.DataBase.SqlLoginUser(login, password);
				if (Service.DataBase.DataBaseProvider.IsNullOrEmpty(result) == false)
				{
					var claims = new List<Claim>()
			{
				//new Claim(ClaimsIdentity.DefaultNameClaimType, ""),
				new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"),
				new Claim(ClaimsIdentity.DefaultRoleClaimType, "authorization"),
				new Claim("login", login),
				new Claim("username_1", result.Rows[0][3].ToString()),
				new Claim("username_2", result.Rows[0][4].ToString()),
				new Claim("username_3", result.Rows[0][5].ToString()),
			};


					ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
					ClaimsIdentity.DefaultRoleClaimType);
					// установка аутентификационных куки
					HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
					return Redirect(Service.RedirectManager.Manager.PathToMainPage);
				}
			}
			catch (Exception ex) { Log.WriteLine(ex, Utilities.Logging.LogLevel.Error); }
			ErrorMessage = "Не верный пароль или логин!";
			return null;
			return Redirect(Service.RedirectManager.Manager.PathToLogin);
		}
	}
}
