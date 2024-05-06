using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BafflerStandalone
{
	public class HotKeysItem
	{
		private int m_id;
		private Keys m_modifier;
		private Keys m_hotkey;
		private List<HotKeyActionItem> m_hotkeyActionList = new List<HotKeyActionItem>();
		public int id => this.m_id;
		public Keys Modifier => this.m_modifier;
		public Keys HotKey => this.m_hotkey;
		public List<HotKeyActionItem> HotKeyActionList => this.m_hotkeyActionList;

		public HotKeysItem(int RowID, Keys modifier, Keys hotkey, List<HotKeyActionItem> actionList)
		{
			this.m_id = RowID;
			this.m_modifier = modifier;
			this.m_hotkey = hotkey;
			this.m_hotkeyActionList = actionList;
		}
	}
}
