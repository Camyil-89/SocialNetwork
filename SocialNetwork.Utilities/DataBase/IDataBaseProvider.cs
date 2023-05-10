using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Utilities.DataBase
{
	public interface IDataBaseProvider
	{
		public void SetConnectString(string connectString);
		public bool Connect();
		public void Disconnect();
		public DataTable SqlQuery(string sql);
		public DataTable SqlNewUser(string login, string pass, string username_1, string username_2, string username_3);
		public DataTable SqlLoginUser(string login, string pass);
		public bool Connecting();
	}
}
