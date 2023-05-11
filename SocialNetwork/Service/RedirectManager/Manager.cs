using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Service.RedirectManager
{
	public static class Manager
	{
		public readonly static string PathToMainPage = "/account/profile";
		public readonly static string PathToLogin = "/Authorization/Login";
		public readonly static string PathToSettingsProfile = "/account/ProfileSettings";
		public readonly static string PathToChat = "/messenger/chat";
	}
}
