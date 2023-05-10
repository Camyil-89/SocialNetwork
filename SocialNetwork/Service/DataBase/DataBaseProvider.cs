using SocialNetwork.Utilities.Logging;
using System.Data;

namespace SocialNetwork.Service.DataBase
{
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
		public static bool IsNullOrEmpty(DataTable data)
		{
			return data == null || data.Rows.Count == 0;
		}
	}
}
