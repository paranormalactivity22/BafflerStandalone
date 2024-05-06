using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BafflerStandalone.Forms.HotKey;

namespace BafflerStandalone.Forms.Options
{
	public partial class Options : Form
	{
		private string tmpDir = "";
		
		public Options()
		{
			this.InitializeComponent();
		}

		public void loadSettings()
		{
			this.tmpDir = Database.Database.Instance.ScreenshotDir;
			this.textBox3.Text = this.tmpDir;
			this.cbOptAlign.SelectedIndex = Database.Database.Instance.Alignment;
			this.tbOptAutoJoin.Text = Database.Database.Instance.AutoJoinRoom;
			this.checkBox2.Checked = Database.Database.Instance.FsBorderless;
			this.checkBox1.Checked = Database.Database.Instance.FsExpand;
			this.checkBox4.Checked = Database.Database.Instance.SnapToEdges;
			this.textBox1.Text = Database.Database.Instance.FileNamePrefix;
			this.textBox2.Text = Database.Database.Instance.FileTimeFormat;
			this.RefreshHotKeyList();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				this.tmpDir = this.folderBrowserDialog1.SelectedPath;
				this.textBox3.Text = this.tmpDir;
			}
		}
		
		private void button1_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.AutoJoinRoom = this.tbOptAutoJoin.Text;
			Database.Database.Instance.ScreenshotDir = this.textBox3.Text;
			Database.Database.Instance.Alignment = this.cbOptAlign.SelectedIndex;
			Database.Database.Instance.FsBorderless = this.checkBox2.Checked;
			Database.Database.Instance.FsExpand = this.checkBox1.Checked;
			Database.Database.Instance.SnapToEdges = this.checkBox4.Checked;
			Database.Database.Instance.FileNamePrefix = this.textBox1.Text;
			Database.Database.Instance.FileTimeFormat = this.textBox2.Text;
			Database.Database.Instance.SaveSettings();
		}

		private void RefreshHotKeyList()
		{
			this.lvHotKeys.Items.Clear();
			foreach (HotKeysItem hotKeysItem in HotKeys.Instance.HotKeyList.Values)
			{
				string text = Utilities.KeyToString(hotKeysItem.HotKey);
				if (hotKeysItem.Modifier != Keys.None)
				{
					text = Utilities.KeyToString(hotKeysItem.Modifier) + "-" + text;
				}
				ListViewItem listViewItem = new ListViewItem(text);
				listViewItem.Tag = hotKeysItem.id;
				string text2 = string.Empty;
				foreach (HotKeyActionItem hotKeyActionItem in hotKeysItem.HotKeyActionList)
				{
					if (hotKeyActionItem.Params.Count > 0)
					{
						object obj = text2;
						text2 = string.Concat(new object[] {obj,hotKeyActionItem.Action,"(",string.Join(",", hotKeyActionItem.Params.ToArray()),"); "});
					}
					else
					{
						text2 = text2 + hotKeyActionItem.Action + "; ";
					}
				}
				listViewItem.SubItems.Add(text2);
				this.lvHotKeys.Items.Add(listViewItem);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			HotKeyForm hotKeyForm = new HotKeyForm();
			hotKeyForm.loadHotKey();
			if (hotKeyForm.ShowDialog() == DialogResult.OK)
			{
				this.RefreshHotKeyList();
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (this.lvHotKeys.SelectedItems.Count > 0 && HotKeys.Instance.HotKeyList.Count > this.lvHotKeys.SelectedItems[0].Index)
			{
				HotKeyForm hotKeyForm = new HotKeyForm();
				hotKeyForm.loadHotKey(HotKeys.Instance.HotKeyList[(int)this.lvHotKeys.SelectedItems[0].Tag]);
				if (hotKeyForm.ShowDialog() == DialogResult.OK)
				{
					this.RefreshHotKeyList();
				}
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (this.lvHotKeys.SelectedItems.Count > 0 && HotKeys.Instance.HotKeyList.Count > this.lvHotKeys.SelectedItems[0].Index)
			{
				HotKeys.Instance.RemoveHotKey((int)this.lvHotKeys.SelectedItems[0].Tag);
				this.RefreshHotKeyList();
			}
		}
	}
}
