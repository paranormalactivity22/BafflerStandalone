using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;

namespace BafflerStandalone
{
	public class HotKeysManager
	{
		private Dictionary<int, HotKeysItem> m_hotKeys;
		public Dictionary<int, HotKeysItem> HotKeyList { get => this.m_hotKeys; set => this.m_hotKeys = value; }

		public HotKeysManager()
		{
			this.m_hotKeys = new Dictionary<int, HotKeysItem>();
			SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT * FROM hotkeys", Database.Database.Instance.Conn);
			using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
			{
				while (sqliteDataReader.Read())
				{
					int @int = sqliteDataReader.GetInt32(0);
					Keys int2 = (Keys)sqliteDataReader.GetInt32(1);
					Keys int3 = (Keys)sqliteDataReader.GetInt32(2);
					List<HotKeyActionItem> actionList = new List<HotKeyActionItem>();
					new List<string>();
					this.m_hotKeys.Add(@int, new HotKeysItem(@int, int2, int3, actionList));
				}
			}
			SQLiteCommand sqliteCommand2 = new SQLiteCommand("SELECT * FROM hotkeyActions", Database.Database.Instance.Conn);
			using (SQLiteDataReader sqliteDataReader2 = sqliteCommand2.ExecuteReader())
			{
				while (sqliteDataReader2.Read())
				{
					int int4 = sqliteDataReader2.GetInt32(0);
					HotKeyAction int5 = (HotKeyAction)sqliteDataReader2.GetInt32(1);
					string @string = sqliteDataReader2.GetString(2);
					string string2 = sqliteDataReader2.GetString(3);
					string string3 = sqliteDataReader2.GetString(4);
					string string4 = sqliteDataReader2.GetString(5);
					List<string> list = new List<string>();
					if (@string != string.Empty)
					{
						list.Add(@string);
						if (string2 != string.Empty)
						{
							list.Add(string2);
							if (string3 != string.Empty)
							{
								list.Add(string3);
								if (string4 != string.Empty)
								{
									list.Add(string4);
								}
							}
						}
					}
					this.m_hotKeys[int4].HotKeyActionList.Add(new HotKeyActionItem(int5, list));
				}
			}
		}

		public void AddHotKey(Keys keyMod, Keys key, List<HotKeyActionItem> actionList)
		{
			HotKeysAdder hotKeysAdder = new HotKeysAdder(Database.Database.Instance.Conn);
			int num = hotKeysAdder.AddMapping(keyMod, key, actionList);
			if (num >= 0)
			{
				this.m_hotKeys.Add(num, new HotKeysItem(num, keyMod, key, actionList));
			}
		}

		public void RemoveHotKey(int index)
		{
			this.m_hotKeys.Remove(index);
			Database.Database.Instance.ExecuteNonQuery("DELETE FROM hotkeys WHERE id = " + index.ToString(), Database.Database.Instance.Conn);
			Database.Database.Instance.ExecuteNonQuery("DELETE FROM hotkeyActions WHERE hotKeyId = " + index.ToString(), Database.Database.Instance.Conn);
		}

		public static void AddDefaultKeys(SQLiteConnection connection)
		{
			HotKeysAdder hotKeysAdder = new HotKeysAdder(connection);
			hotKeysAdder.AddMapping(Keys.None, Keys.F1, HotKeyAction.ShowHelp);
			hotKeysAdder.AddMapping(Keys.None, Keys.F2, HotKeyAction.ShowOptions);
			hotKeysAdder.AddMapping(Keys.None, Keys.F3, HotKeyAction.ShowQuickAlign);
			hotKeysAdder.AddMapping(Keys.None, Keys.F11, HotKeyAction.ToggleFullscreen);
			hotKeysAdder.AddMapping(Keys.None, Keys.Escape, HotKeyAction.ExitFullScreen);
			hotKeysAdder.AddMapping(Keys.None, Keys.F12, HotKeyAction.Screenshot);
		}

		public static bool IsBound(Keys inputModifier, Keys inputKey, SQLiteConnection connection)
		{
			SQLiteCommand sqliteCommand = new SQLiteCommand(string.Concat(new object[]
			{
				"SELECT COUNT(*) FROM hotkeys WHERE inputModifier = ",
				(int)inputModifier,
				" AND inputKey = ",
				(int)inputKey
			}), connection);
			int num = Convert.ToInt32(sqliteCommand.ExecuteScalar());
			return num > 0;
		}
	}
}
