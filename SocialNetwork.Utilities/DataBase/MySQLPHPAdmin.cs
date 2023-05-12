using MySql.Data.MySqlClient;
using SocialNetwork.Utilities.Logging;
using System.Data;
using System.Diagnostics;

namespace SocialNetwork.Utilities.DataBase
{
	public class Connect
	{
		public MySqlConnection SqlConnection;
		public Connect(string connect)
		{
			SqlConnection = new MySqlConnection(connect);
			SqlConnection.Open();
		}
	}

	public class MySQLPHPAdmin : IDataBaseProvider
	{
		private string _connection = "";

		public void SetConnectString(string connectString)
		{
			_connection = connectString;
		}

		public DataTable SqlLoginUser(string login, string pass)
		{
			MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE `login` = @login AND `password` = @pass;");
			command.Parameters.AddWithValue("@login", login);
			command.Parameters.AddWithValue("@pass", Cryptography.Checksum.Compute(pass));
			return SqlExecute(command);
		}
		private DataTable ConvertReaderToDataTable(MySqlDataReader reader)
		{
			DataTable dataTable = new DataTable();

			// Добавляем колонки с названиями и типами данных из MySqlDataReader
			for (int i = 0; i < reader.FieldCount; i++)
			{
				dataTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
			}

			// Заполняем таблицу данными из MySqlDataReader
			while (reader.Read())
			{
				DataRow dataRow = dataTable.NewRow();
				for (int i = 0; i < reader.FieldCount; i++)
				{
					dataRow[i] = reader.GetValue(i);
				}
				dataTable.Rows.Add(dataRow);
			}

			return dataTable;
		}
		private DataTable SqlExecute(MySqlCommand sqlCommand)
		{
			try
			{

				DataTable dtable = new DataTable();
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				using (var connection = new MySqlConnection(_connection))
				{
					connection.Open();
					sqlCommand.Connection = connection;

					using (MySqlDataReader reader = sqlCommand.ExecuteReader())
					{
						dtable = ConvertReaderToDataTable(reader);
					}
				}

				
				Log.WriteLine($"SQL query: {stopwatch.ElapsedMilliseconds} [{sqlCommand.CommandText}] [{dtable.Rows.Count}]");
				return dtable;
			}
			catch (Exception ex) { Log.WriteLine(ex, LogLevel.Error); return null; }
		}
		public void CreateChat(string id_user, string type)
		{
			MySqlCommand command = new MySqlCommand($"INSERT INTO `chats` (`id`, `id_admin_user`, `chat_type`) VALUES (NULL, @id, @type);");
			command.Parameters.AddWithValue($"@id", id_user);
			command.Parameters.AddWithValue($"@type", type);
			SqlExecute(command);
		}
		public DataTable SqlNewUser(string login, string pass, string username_1, string username_2, string username_3)
		{
			MySqlCommand command = new MySqlCommand($"INSERT INTO `users` (`id`, `login`, `password`, `username_1`, `username_2`, `username_3`) VALUES (NULL, @login, @pass, @name_1, @name_2, @name_3);");
			command.Parameters.AddWithValue($"@login", login);
			command.Parameters.AddWithValue($"@pass", Cryptography.Checksum.Compute(pass));
			command.Parameters.AddWithValue($"@name_1", username_1);
			command.Parameters.AddWithValue($"@name_2", username_2);
			command.Parameters.AddWithValue($"@name_3", username_3);
			return SqlExecute(command);
		}

		public DataTable SqlGetAllUsers()
		{
			MySqlCommand command = new MySqlCommand($"SELECT * FROM `users`;");
			return SqlExecute(command);
		}
		public DataTable SqlQuery(string sql)
		{
			return SqlExecute(new MySqlCommand(sql));
		}

		public DataTable SqlQuery(MySqlCommand command)
		{
			return SqlExecute(command);
		}

		public DataTable SqlGetUser(string id)
		{
			MySqlCommand command = new MySqlCommand($"SELECT * FROM `users` WHERE `id` = @id;");
			command.Parameters.AddWithValue("@id", id);
			return SqlExecute(command);
		}
		public DataTable SqlSaveImage(string id, MemoryStream stream)
		{
			MySqlCommand command = new MySqlCommand($"UPDATE `users` SET `image_avatar` = @image WHERE `id` = @id;");
			command.Parameters.AddWithValue("@id", id);
			command.Parameters.AddWithValue("@image", stream.ToArray());
			return SqlExecute(command);
		}
	}
}
