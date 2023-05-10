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

			foreach (DataRow i in DataBase.SqlGetAllUsers().Rows)
			{
				var user = new User();
				user.Id = i[0].ToString();
				user.Name_1 = i[3].ToString();
				user.Name_2 = i[4].ToString();
				user.Name_3 = i[5].ToString();
				users.Add(user);
			}
			return users;
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
