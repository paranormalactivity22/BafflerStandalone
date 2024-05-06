using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace BafflerStandalone
{
	public class HotKeysAdder
	{
		private SQLiteCommand insertMapping;
		private SQLiteParameter inputCodeParam;
		private SQLiteParameter inputTypeParam;
		private SQLiteCommand insertAction;
		private SQLiteParameter actionHotKeyIdParam;
		private SQLiteParameter actionCodeParam;
		private SQLiteParameter actionParam1;
		private SQLiteParameter actionParam2;
		private SQLiteParameter actionParam3;
		private SQLiteParameter actionParam4;
		
		public HotKeysAdder(SQLiteConnection connection)
		{
			this.insertMapping = new SQLiteCommand("INSERT INTO hotkeys (inputModifier, inputKey) VALUES (?, ?); SELECT last_insert_rowid() AS RecordID", connection);
			this.inputCodeParam = this.insertMapping.Parameters.Add("inputModifier", DbType.Int32);
			this.inputTypeParam = this.insertMapping.Parameters.Add("inputKey", DbType.Int32);
			this.insertAction = new SQLiteCommand("INSERT INTO hotkeyActions (hotKeyId, actionCode, param1, param2, param3, param4) VALUES (?, ?, ?, ?, ?, ?)", connection);
			this.actionHotKeyIdParam = this.insertAction.Parameters.Add("hotKeyId", DbType.Int32);
			this.actionCodeParam = this.insertAction.Parameters.Add("actionCode", DbType.Int32);
			this.actionParam1 = this.insertAction.Parameters.Add("param1", DbType.String);
			this.actionParam2 = this.insertAction.Parameters.Add("param2", DbType.String);
			this.actionParam3 = this.insertAction.Parameters.Add("param3", DbType.String);
			this.actionParam4 = this.insertAction.Parameters.Add("param4", DbType.String);
		}

		public int AddMapping(Keys modkey, Keys key, HotKeyAction action)
		{
			List<HotKeyActionItem> list = new List<HotKeyActionItem>();
			List<string> parameters = new List<string>();
			list.Add(new HotKeyActionItem(action, parameters));
			return this.AddMapping((int)modkey, (int)key, list);
		}

		public int AddMapping(Keys modkey, Keys key, List<HotKeyActionItem> actions)
		{
			return this.AddMapping((int)modkey, (int)key, actions);
		}

		public int AddMapping(int inputMod, int inputKey, List<HotKeyActionItem> actions)
		{
			int num = -1;
			if (actions.Count > 0)
			{
				this.inputCodeParam.Value = inputMod;
				this.inputTypeParam.Value = inputKey;
				num = Convert.ToInt32(this.insertMapping.ExecuteScalar());
				foreach (HotKeyActionItem hotKeyActionItem in actions)
				{
					this.actionHotKeyIdParam.Value = num;
					this.actionCodeParam.Value = hotKeyActionItem.Action;
					for (int i = 0; i <= 3; i++)
					{
						if (i == 0)
						{
							if (hotKeyActionItem.Params.Count > 0)
							{
								this.actionParam1.Value = hotKeyActionItem.Params[0];
							}
							else
							{
								this.actionParam1.Value = string.Empty;
							}
						}
						else if (i == 1)
						{
							if (hotKeyActionItem.Params.Count > 1)
							{
								this.actionParam2.Value = hotKeyActionItem.Params[1];
							}
							else
							{
								this.actionParam2.Value = string.Empty;
							}
						}
						else if (i == 2)
						{
							if (hotKeyActionItem.Params.Count > 2)
							{
								this.actionParam3.Value = hotKeyActionItem.Params[2];
							}
							else
							{
								this.actionParam3.Value = string.Empty;
							}
						}
						else if (i == 3)
						{
							if (hotKeyActionItem.Params.Count > 3)
							{
								this.actionParam4.Value = hotKeyActionItem.Params[3];
							}
							else
							{
								this.actionParam4.Value = string.Empty;
							}
						}
					}
					this.insertAction.ExecuteNonQuery();
				}
			}
			return num;
		}
	}
}
