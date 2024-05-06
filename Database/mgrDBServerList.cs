using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace BafflerStandalone.Database
{
	public class MgrDBServerList
	{
		private Dictionary<string, string> m_serverlist;
		public string currentServerLabel = string.Empty;
		public string currentServerURL = string.Empty;
		
		public Dictionary<string, string> List
		{
			get
			{
				return this.m_serverlist;
			}
			set
			{
				this.m_serverlist = value;
			}
		}

		public MgrDBServerList()
		{
			this.m_serverlist = new Dictionary<string, string>();
			SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT * FROM serverlist", Database.Instance.Conn);
			using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
			{
				while (sqliteDataReader.Read())
				{
					this.m_serverlist.Add(sqliteDataReader.GetString(0), sqliteDataReader.GetString(1));
				}
			}
		}

		public void AddServer(string server, string url)
		{
			Database.Instance.ExecuteNonQuery(string.Concat(new string[] {"INSERT INTO serverlist (server, url) VALUES ('",server,"', '",url,"')"}));
			if (!this.m_serverlist.ContainsKey(server))
			{
				this.m_serverlist.Add(server, url);
			}
		}

		public void RemoveServer(string server)
		{
			Database.Instance.ExecuteNonQuery("DELETE FROM serverlist WHERE server = '" + server + "'");
			if (this.m_serverlist.ContainsKey(server))
			{
				this.m_serverlist.Remove(server);
			}
		}

		public void EditServer(string server, string url)
		{
			Database.Instance.ExecuteNonQuery(string.Concat(new string[] {"UPDATE serverlist SET url = '",url,"' WHERE server = '",server,"'"}));
			if (this.m_serverlist.ContainsKey(server))
			{
				this.m_serverlist[server] = url;
			}
		}
		
		public void RemoveAllServers()
		{
			Database.Instance.ExecuteNonQuery("DELETE FROM serverlist");
		}
	}
}
