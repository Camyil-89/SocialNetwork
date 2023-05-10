using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using SocialNetwork.Utilities.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.Utilities.DataBase
{
	public class MySQLPHPAdmin : IDataBaseProvider
	{
		private string _connection = "";
		private MySqlConnection _sql_connection;
		public bool Connect()
		{
			if (_sql_connection == null) 
				_sql_connection = new MySqlConnection(_connection);

			if (Connecting() == false)
			{
				try
				{
					_sql_connection.Open();
					return true;
				} catch (Exception ex) { }
				return false;
			}
			return true;
		}

		public bool Connecting()
		{
			return _sql_connection.State != ConnectionState.Closed && _sql_connection.State != ConnectionState.Broken;
		}

		public void Disconnect()
		{
			_sql_connection.Close();
			_sql_connection.Dispose();
			_sql_connection = null;
		}

		public void SetConnectString(string connectString)
		{
			_connection = connectString;
		}

		public DataTable SqlLoginUser(string login, string pass)
		{
			MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE `login` = @login AND `password` = @pass;", _sql_connection);
			command.Parameters.AddWithValue("@login", Cryptography.Checksum.Compute(login));
			command.Parameters.AddWithValue("@pass", Cryptography.Checksum.Compute(pass));
			return SqlExecute(command);
		}
		private DataTable SqlExecute(MySqlCommand sqlCommand)
		{
			if (Connect() == false)
				return null;
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				MySqlDataAdapter adapter = new MySqlDataAdapter();
				adapter.SelectCommand = sqlCommand;
				DataTable dtable = new DataTable();
				adapter.Fill(dtable);
				Log.WriteLine($"SQL query: {stopwatch.ElapsedMilliseconds} [{adapter.SelectCommand.CommandText}] [{dtable.Rows.Count}]");
				return dtable;
			}
			catch (Exception ex) { Log.WriteLine(ex, LogLevel.Error); return null; }
		}
		public DataTable SqlNewUser(string login, string pass, string username_1, string username_2, string username_3)
		{
			MySqlCommand command = new MySqlCommand($"INSERT INTO `users` (`id`, `login`, `password`, `username_1`, `username_2`, `username_3`) VALUES (NULL, @login, @pass, @name_1, @name_2, @name_3);", _sql_connection);
			command.Parameters.AddWithValue($"@login", Cryptography.Checksum.Compute(login));
			command.Parameters.AddWithValue($"@pass", Cryptography.Checksum.Compute(pass));
			command.Parameters.AddWithValue($"@name_1", username_1);
			command.Parameters.AddWithValue($"@name_2", username_2);
			command.Parameters.AddWithValue($"@name_3", username_3);
			return SqlExecute(command);
		}

		public DataTable SqlQuery(string sql)
		{
			return SqlExecute(new MySqlCommand(sql, _sql_connection));
		}
	}
}
