using MySql.Data.MySqlClient;
using SocialNetwork.Models;
using SocialNetwork.Utilities.Logging;
using System.Data;

namespace SocialNetwork.Service.DataBase
{
	public class DataProvider
	{
		public DataTable Data { get; set; }
		public DataProvider(DataTable data)
		{
			Data = data;
		}
		public bool IsNullOrEmpty()
		{
			return DataBaseProvider.IsNullOrEmpty(Data);
		}
		public string GetValueFromColumn(string columnName)
		{
			foreach (DataColumn i in Data.Columns)
			{
				if (i.ColumnName == columnName)
				{
					return Data.Rows[0][i].ToString();
				}
			}
			return null;
		}
		public User GetUser()
		{
			User user = new User();

			user.Id = GetValueFromColumn("id");
			user.Name_1 = GetValueFromColumn("username_1");
			user.Name_2 = GetValueFromColumn("username_2");
			user.Name_3 = GetValueFromColumn("username_3");
			user.ImageUrl = GetBase64Image();

			return user;
		}
		public string GetBase64Image()
		{
			foreach (DataColumn i in Data.Columns)
			{
				if (i.ColumnName == "image_avatar")
				{
					try
					{
						byte[] bytes = (byte[])Data.Rows[0][i];
						return string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bytes, 0, bytes.Length));
					}
					catch { }
					return "/images/no_image.png";
				}
			}
			return "/images/no_image.png";
		}
	}
	public static class DataBaseProvider
	{
		public static Utilities.DataBase.IDataBaseProvider DataBase = new Utilities.DataBase.MySQLPHPAdmin();
		public static bool IsConnecting { get => DataBase.Connecting(); }

		public static void Init(string connect)
		{
			DataBase.SetConnectString(connect);
			DataBase.Connect();
			Log.WriteLine($"DataBaseProvider.Init: {IsConnecting}");
		}
		public static List<User> GetAllUsers()
		{
			List<User> users = new List<User>();
			var response = DataBase.SqlGetAllUsers();
			if (response == null)
				return users;
			foreach (DataRow i in response.Rows)
			{
				var user = new User();
				user.Id = i[0].ToString();
				user.Name_1 = i[3].ToString();
				user.Name_2 = i[4].ToString();
				user.Name_3 = i[5].ToString();
				if (string.IsNullOrEmpty(i[6].ToString()) == false)
				{
					byte[] bytes = (byte[])i[6];
					user.ImageUrl = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bytes, 0, bytes.Length));
				}

				users.Add(user);
			}
			return users;
		}
		public static List<User> ChatUsers(string id_chat)
		{
			List<User> users = new List<User>();

			MySqlCommand command = new MySqlCommand("SELECT * FROM `members_chat` WHERE id_chat = @chat;");
			command.Parameters.AddWithValue($"@chat", id_chat);
			var answer_1 = DataBase.SqlQuery(command);
			Console.WriteLine($">>>{id_chat}<<< | {answer_1 == null}");
			foreach (DataRow i in answer_1.Rows)
			{
				users.Add(CreateProvider(DataBase.SqlGetUser(i[2].ToString())).GetUser());
			}


			return users;
		}
		public static List<Message> GetMessages(string id_chat)
		{
			List<Message> messages = new List<Message>();
			MySqlCommand command = new MySqlCommand("SELECT * FROM messages WHERE id_chat = @chat");
			command.Parameters.AddWithValue($"@chat", id_chat);
			var response = DataBase.SqlQuery(command);

			foreach (DataRow i in response.Rows)
			{
				Message message = new Message();
				message.Id = i[0].ToString();
				message.IdChat = i[1].ToString();
				message.IdUser = i[2].ToString();
				message.Text = i[3].ToString();
				message.File = "NOTTEST";
				messages.Add(message);
			}

			return messages;
		}
		public static string GetIdSelfChat(string id_user)
		{
			MySqlCommand command = new MySqlCommand("SELECT * FROM `chats` WHERE `id_admin_user` = @id and `chat_type` = 'self';");
			command.Parameters.AddWithValue($"@id", id_user);
			var response = DataBase.SqlQuery(command);
			if (IsNullOrEmpty(response) == false)
			{
				return response.Rows[0][0].ToString();
			}
			return null;
		}
		public static Chat GetChat(string id_chat)
		{
			Chat chat = new Chat();
			chat.Id = id_chat;

			chat.Users = ChatUsers(id_chat);
			chat.Messages = GetMessages(id_chat);

			return chat;
		}
		public static DataProvider CreateProvider(DataTable dataTable)
		{
			return new DataProvider(dataTable);
		}
		public static bool IsNullOrEmpty(DataTable data)
		{
			return data == null || data.Rows.Count == 0;
		}
	}
}
