using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Security.Certificates;
using SocialNetwork.Models;
using SocialNetwork.Utilities.Logging;
using System.Data;
using System.Text;

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
			if (Data == null)
				return null;
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
			if (Data == null)
				return "/images/no_image.png";
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
			foreach (DataRow i in answer_1.Rows)
			{
				users.Add(CreateProvider(DataBase.SqlGetUser(i[2].ToString())).GetUser());
			}


			return users;
		}
		public static bool UserInChat(string id_chat, string id_user)
		{
			MySqlCommand command = new MySqlCommand("SELECT * FROM `members_chat` WHERE id_chat = @chat and id_user = @user;");
			command.Parameters.AddWithValue($"@chat", id_chat);
			command.Parameters.AddWithValue($"@user", id_user);
			var answer_1 = DataBase.SqlQuery(command);
			return IsNullOrEmpty(answer_1) == false;
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
		public static Message SendNewMessage(string id_user, string id_chat, string text, string type)
		{
			//INSERT INTO `messages` (`id`, `id_chat`, `id_user`, `message`, `file`, `time`, `type`) VALUES (NULL, '3', '5', 'test', NULL, '3123125414124124124', 'msg');

			var time = DateTime.Now.Ticks;
			MySqlCommand command = new MySqlCommand($"INSERT INTO `messages` (`id`, `id_chat`, `id_user`, `message`, `file`, `time`, `type`) VALUES (NULL, @chat, @user, @msg, NULL, {time}, '{type}');SELECT LAST_INSERT_ID();");
			command.Parameters.AddWithValue($"@chat", id_chat);
			command.Parameters.AddWithValue($"@user", id_user);
			command.Parameters.AddWithValue($"@msg", text);
			var answer = DataBase.SqlQuery(command);
			var provider = CreateProvider(DataBase.SqlQuery($"SELECT * FROM `messages` WHERE `id` = {answer.Rows[0][0]};"));

			var message = new Message();
			message.Text = provider.GetValueFromColumn("message");
			message.IdChat = provider.GetValueFromColumn("id_chat");
			message.Id = provider.GetValueFromColumn("id");
			message.IdUser = provider.GetValueFromColumn("id_user");

			return message;
		}
		public static Chat GetChat(string id_chat)
		{
			Chat chat = new Chat();
			chat.Id = id_chat;

			MySqlCommand command = new MySqlCommand("SELECT * FROM `chats` WHERE `id` = @id;");
			command.Parameters.AddWithValue($"@id", id_chat);
			var answer = DataBase.SqlQuery(command);
			if (IsNullOrEmpty(answer) == false)
			{
				chat.IdUserAdmin = answer.Rows[0][1].ToString();
				chat.Type = answer.Rows[0][2].ToString();
			}

			chat.Users = ChatUsers(id_chat);
			chat.Messages = GetMessages(id_chat);

			return chat;
		}
		public static List<string> GetMyChats(string id)
		{
			MySqlCommand command = new MySqlCommand("SELECT `id` FROM `members_chat` WHERE `members_chat`.`id_user` = @id;");
			command.Parameters.AddWithValue($"@id", id);
			var answer = DataBase.SqlQuery(command);
			if (IsNullOrEmpty(answer) == false)
			{
				List<string> list = new List<string>();

				foreach (DataRow i in answer.Rows)
				{
					list.Add(i[0].ToString());
				}
				return list;
			}
			return new List<string>();
		}
		public static List<string> UserInChats(List<string> chats, string id_user, string type)
		{
			MySqlCommand command = new MySqlCommand($"SELECT * FROM `members_chat` INNER JOIN `chats` ON `chats`.`id` = `members_chat`.`id` WHERE `members_chat`.`id_user` = @id and `chats`.`chat_type` = @type and `members_chat`.`id_chat` IN ({string.Join(", ",chats)});");
			command.Parameters.AddWithValue($"@id", id_user);
			command.Parameters.AddWithValue($"@type", type);
			var answer = DataBase.SqlQuery(command);
			return null;
		}
		public static string GetIdChatFromIdUser(string id1, string id2, string type)
		{
			//SELECT `chats`.`id`, `members_chat`.`id_user`, `chats`.`chat_type` FROM `members_chat` INNER JOIN `chats` ON `chats`.`id` = `members_chat`.`id_chat` WHERE `chats`.`chat_type` = 'self' and `chats`.`id_admin_user` = '5' OR `members_chat`.id_user = '5';
			MySqlCommand command = new MySqlCommand("SELECT `chats`.`id` FROM `members_chat` INNER JOIN `chats` ON `chats`.`id` = `members_chat`.`id_chat` WHERE `chats`.`chat_type` = @type and (`chats`.`id_admin_user` = @id1 OR `members_chat`.id_user = @id2);");
			command.Parameters.AddWithValue($"@id1", id1);
			command.Parameters.AddWithValue($"@id2", id2);
			command.Parameters.AddWithValue($"@type", type);
			var answer = DataBase.SqlQuery(command);
			if (IsNullOrEmpty(answer) == false)
			{
				return answer.Rows[0][0].ToString();
			}
			return "";
		}
		public static void CreateChat(string id_user1, string id_user2)
		{

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
