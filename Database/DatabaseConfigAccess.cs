using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace BafflerStandalone.Database
{
	public static class DatabaseConfigAccess
	{
		private static Dictionary<string, string> configDefaults = new Dictionary<string, string>();
		
		static DatabaseConfigAccess()
		{
			DatabaseConfigAccess.configDefaults.Add("Version", 1.ToString());
			DatabaseConfigAccess.configDefaults.Add("AutoJoinRoom", "cheeseformice bootcamp");
			DatabaseConfigAccess.configDefaults.Add("Snap", "true");
		}

		public static Dictionary<string, string> ConfigDefaults
		{
			get
			{
				return DatabaseConfigAccess.configDefaults;
			}
		}

		public static bool GetConfigBool(string configName, SQLiteConnection connection)
		{
			return bool.Parse(DatabaseConfigAccess.GetConfigString(configName, connection));
		}

		public static double GetConfigDouble(string configName, SQLiteConnection connection)
		{
			return double.Parse(DatabaseConfigAccess.GetConfigString(configName, connection));
		}

		public static int GetConfigInt(string configName, SQLiteConnection connection)
		{
			return int.Parse(DatabaseConfigAccess.GetConfigString(configName, connection));
		}

		public static string GetConfigString(string configName, SQLiteConnection connection)
		{
			using (SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT value FROM settings WHERE name = '" + configName + "'", connection))
			{
				using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					if (sqliteDataReader.Read())
					{
						return sqliteDataReader.GetString(0);
					}
				}
			}
			if (DatabaseConfigAccess.configDefaults.ContainsKey(configName))
			{
				DatabaseConfigAccess.AddConfigValue(configName, DatabaseConfigAccess.configDefaults[configName], connection);
				return DatabaseConfigAccess.configDefaults[configName];
			}
			throw new ArgumentException("Config value not found: " + configName, "configName");
		}

		private static void AddConfigValue(string configName, string configValue, SQLiteConnection connection)
		{
			using (SQLiteCommand sqliteCommand = new SQLiteCommand(connection))
			{
				SQLiteParameter sqliteParameter = new SQLiteParameter();
				SQLiteParameter sqliteParameter2 = new SQLiteParameter();
				sqliteCommand.CommandText = "INSERT INTO settings (name, status, value) VALUES (?, 0, ?)";
				sqliteCommand.Parameters.Add(sqliteParameter);
				sqliteCommand.Parameters.Add(sqliteParameter2);
				sqliteParameter.Value = configName;
				sqliteParameter2.Value = configValue;
				sqliteCommand.ExecuteNonQuery();
			}
		}

		public static void SetConfigValue(string configName, bool value, SQLiteConnection connection)
		{
			DatabaseConfigAccess.SetConfigValue(configName, value.ToString(), connection);
		}

		public static void SetConfigValue(string configName, double value, SQLiteConnection connection)
		{
			DatabaseConfigAccess.SetConfigValue(configName, value.ToString(), connection);
		}

		public static void SetConfigValue(string configName, int value, SQLiteConnection connection)
		{
			DatabaseConfigAccess.SetConfigValue(configName, value.ToString(), connection);
		}

		public static void SetConfigValue(string configName, string configValue, SQLiteConnection connection)
		{
			using (SQLiteCommand sqliteCommand = new SQLiteCommand(string.Concat(new string[] {"UPDATE settings SET value = \"",configValue,"\", Status = 1 WHERE name = '",configName,"'"}), connection))
			{
				if (sqliteCommand.ExecuteNonQuery() == 0)
				{
					DatabaseConfigAccess.AddConfigValue(configName, configValue, connection);
				}
			}
		}
	}
}
