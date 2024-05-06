using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace BafflerStandalone.Database
{
	public class DatabaseManager
	{
		private const string dbName = "BafflerStandalone.sqlite";
		private SQLiteConnection m_conn;
		private bool m_SettingsChange = true;
		private string m_AutoJoinRoom = string.Empty;
		private string m_ScreenshotDir = string.Empty;
		private int m_Alignment = 8;
		private bool m_FsBorderless = true;
		private bool m_FsExpand = true;
		private bool m_SnapToEdges = true;
		private string m_FileNamePrefix = "Transformice ";
		private string m_FileTimeFormat = "MM-dd-yyyy HHmmss_fff";

		public SQLiteConnection Conn
		{
			get
			{
				if (this.m_conn.State == ConnectionState.Closed)
				{
					this.m_conn.Open();
				}
				return this.m_conn;
			}
			set
			{
				this.m_conn = value;
			}
		}

		public string AutoJoinRoom
		{
			get
			{
				return this.m_AutoJoinRoom;
			}
			set
			{
				this.m_AutoJoinRoom = value;
				this.m_SettingsChange = true;
			}
		}

		public string ScreenshotDir
		{
			get
			{
				return this.m_ScreenshotDir;
			}
			set
			{
				this.m_ScreenshotDir = value;
				this.m_SettingsChange = true;
			}
		}

		public int Alignment
		{
			get
			{
				return this.m_Alignment;
			}
			set
			{
				this.m_Alignment = value;
				this.m_SettingsChange = true;
			}
		}

		public bool FsBorderless
		{
			get
			{
				return this.m_FsBorderless;
			}
			set
			{
				this.m_FsBorderless = value;
				this.m_SettingsChange = true;
			}
		}

		public bool FsExpand
		{
			get
			{
				return this.m_FsExpand;
			}
			set
			{
				this.m_FsExpand = value;
				this.m_SettingsChange = true;
			}
		}

		public bool SnapToEdges
		{
			get
			{
				return this.m_SnapToEdges;
			}
			set
			{
				this.m_SnapToEdges = value;
				this.m_SettingsChange = true;
			}
		}

		public string FileNamePrefix
		{
			get
			{
				return this.m_FileNamePrefix;
			}
			set
			{
				this.m_FileNamePrefix = value;
				this.m_SettingsChange = true;
			}
		}

		public string FileTimeFormat
		{
			get
			{
				return this.m_FileTimeFormat;
			}
			set
			{
				this.m_FileTimeFormat = value;
				this.m_SettingsChange = true;
			}
		}

		public DatabaseManager()
		{
			if (this.m_conn == null)
			{
				if (!File.Exists(dbName))
				{
					SQLiteConnection.CreateFile(dbName);
					this.m_conn = new SQLiteConnection("Data Source=|DataDirectory|"+dbName);
					this.m_conn.Open();
					this.CreateTables(this.m_conn);
					return;
				}
				this.m_conn = new SQLiteConnection("Data Source=|DataDirectory|"+dbName);
				this.m_conn.Open();
			}
		}

		private void CreateTables(SQLiteConnection connection)
		{
			this.ExecuteNonQuery("CREATE TABLE settings (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT UNIQUE, status INTEGER, value TEXT)", connection);
			this.InsertDefaultSettings();
			this.ExecuteNonQuery("CREATE TABLE serverlist (server TEXT UNIQUE, url TEXT)", connection);
			this.ExecuteNonQuery("CREATE TABLE hotkeys (id INTEGER PRIMARY KEY AUTOINCREMENT, inputModifier INTEGER, inputKey INTEGER)", connection);
			this.ExecuteNonQuery("CREATE TABLE hotkeyActions (hotKeyId INTEGER, actionCode INTEGER, param1 TEXT DEFAULT '', param2 TEXT DEFAULT '', param3 TEXT DEFAULT '', param4 TEXT DEFAULT '')", connection);
			HotKeysManager.AddDefaultKeys(connection);
		}

		public void LoadSettings()
		{
			this.m_AutoJoinRoom = this.GetStringSetting("AutoJoinRoom");
			this.m_ScreenshotDir = this.GetStringSetting("ScreenshotDir");
			this.m_SnapToEdges = DatabaseConfigAccess.GetConfigBool("Snap", this.Conn);
			this.m_Alignment = DatabaseConfigAccess.GetConfigInt("Alignment", this.Conn);
		}

		public void SaveSettings()
		{
			if (this.m_SettingsChange)
			{
				this.SaveStringSetting("AutoJoinRoom", this.m_AutoJoinRoom);
				this.SaveStringSetting("ScreenshotDir", this.m_ScreenshotDir);
				DatabaseConfigAccess.SetConfigValue("Snap", this.m_SnapToEdges, this.Conn);
				DatabaseConfigAccess.SetConfigValue("Alignment", this.m_Alignment, this.Conn);
				this.m_SettingsChange = false;
			}
		}

		private void InsertDefaultSettings()
		{
			this.ExecuteNonQuery("INSERT INTO settings (name, status, value) VALUES ('AutoJoinRoom', 0, 'cheeseformice bootcamp')", this.Conn);
			this.ExecuteNonQuery("INSERT INTO settings (name, status, value) VALUES ('ScreenshotDir', 0, '')", this.Conn);
			this.ExecuteNonQuery("INSERT INTO settings (name, status, value) VALUES ('Alignment', 0, '8')", this.Conn);
			this.ExecuteNonQuery("INSERT INTO settings (name, status, value) VALUES ('Snap', 0, 'true')", this.Conn);
		}

		public string GetStringSetting(string Settingname)
		{
			string commandText = "SELECT * FROM settings WHERE name = '" + Settingname + "' LIMIT 1";
			string result;
			try
			{
				using (SQLiteCommand sqliteCommand = new SQLiteCommand(commandText, this.Conn))
				{
					using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
					{
						if (sqliteDataReader.HasRows)
						{
							if (sqliteDataReader.Read())
							{
								result = sqliteDataReader.GetString(3);
							}
							else
							{
								result = "";
							}
						}
						else
						{
							result = "";
						}
					}
				}
			}
			catch (SQLiteException ex)
			{
				MessageBox.Show(ex.Message);
				result = "";
			}
			catch (Exception ex2)
			{
				MessageBox.Show(ex2.Message);
				result = "";
			}
			return result;
		}

		public void SaveStringSetting(string SettingName, string Value)
		{
			string text = string.Concat(new string[] {"UPDATE settings SET Value = '",Value,"', Status = 1 WHERE Name = '",SettingName,"'"});
			try
			{
				using (SQLiteCommand sqliteCommand = new SQLiteCommand(text, this.Conn))
				{
					try
					{
						int num = sqliteCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						string.Concat(new object[] {"No rows were inserted for query:\n",ex.Message,"\n",ex.InnerException,"\n",text,"\n"});
					}
				}
			}
			catch (SQLiteException ex2)
			{
				MessageBox.Show(ex2.Message);
			}
			catch (Exception ex3)
			{
				MessageBox.Show(ex3.Message);
			}
		}

		public void ExecuteNonQuery(string query, SQLiteConnection connection)
		{
			try
			{
				using (SQLiteCommand sqliteCommand = new SQLiteCommand(connection))
				{
					sqliteCommand.CommandText = query;
					sqliteCommand.ExecuteNonQuery();
				}
			}
			catch (SQLiteException ex)
			{
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex2)
			{
				MessageBox.Show(ex2.Message);
			}
		}

		public void ExecuteNonQuery(string query)
		{
			try
			{
				using (SQLiteCommand sqliteCommand = new SQLiteCommand(this.Conn))
				{
					sqliteCommand.CommandText = query;
					sqliteCommand.ExecuteNonQuery();
				}
			}
			catch (SQLiteException ex)
			{
				MessageBox.Show(ex.Message);
			}
			catch (Exception ex2)
			{
				MessageBox.Show(ex2.Message);
			}
		}
	}
}
