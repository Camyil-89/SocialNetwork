using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Models;
using SocialNetwork.Service.DataBase;

namespace SocialNetwork.Pages.Messenger
{
	public class LongPoolHub : Hub
	{
		public async Task JoinChat(string chatId)
		{
			Console.WriteLine($"JoinChat: {chatId}");
			if (DataBaseProvider.UserInChat(chatId, Context.User.FindFirst("id").Value.ToString()))
				await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
		}

		public async Task LeaveChat(string chatId)
		{
			Console.WriteLine($"LeaveChat: {chatId}");
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
		}

		public async Task SendMessage(string chatId, string message)
		{
			Console.WriteLine($"SendMessage: {chatId};{message}");
			if (string.IsNullOrEmpty(message))
				return;
			if (DataBaseProvider.UserInChat(chatId, Context.User.FindFirst("id").Value.ToString()))
			{
				var sql_message = DataBaseProvider.SendNewMessage(Context.User.FindFirst("id").Value.ToString(), chatId, message, "msg");
				await Clients.Group(chatId).SendAsync("ReceiveMessage", sql_message);
			}
		}
	}
}
