using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BafflerStandalone.Forms.HotKey
{
	public partial class HotKeyForm : Form
	{
		private Keys tmpModKey;
		private Keys tmpKey;
		private List<HotKeyActionItem> actions = new List<HotKeyActionItem>();
		private bool isEditing;
		private HotKeysItem editingHotkey;
		
		public HotKeyForm()
		{
			this.InitializeComponent();
		}

		public void loadHotKey()
		{
			this.isEditing = false;
			this.tmpModKey = Keys.None;
			this.tmpKey = Keys.None;
			this.actions.Clear();
			this.comboBox3.SelectedIndex = 0;
		}

		public void loadHotKey(HotKeysItem hotkey)
		{
			this.isEditing = true;
			this.editingHotkey = hotkey;
			this.tmpModKey = hotkey.Modifier;
			Keys keys = this.tmpModKey;
			if (keys != Keys.Shift)
			{
				if (keys != Keys.Control)
				{
					if (keys != Keys.Alt)
					{
						this.comboBox3.SelectedIndex = 0;
					}
					else
					{
						this.comboBox3.SelectedIndex = 2;
					}
				}
				else
				{
					this.comboBox3.SelectedIndex = 1;
				}
			}
			else
			{
				this.comboBox3.SelectedIndex = 3;
			}
			this.tmpKey = hotkey.HotKey;
			this.textBox1.Text = Utilities.KeyToString(this.tmpKey);
			this.actions.AddRange(hotkey.HotKeyActionList);
			foreach (HotKeyActionItem hotKeyActionItem in hotkey.HotKeyActionList)
			{
				this.listBox1.Items.Add(this.listBox1.Items.Count.ToString() + ": " + hotKeyActionItem.toString());
			}
		}

		private void textBox1_KeyUp(object sender, KeyEventArgs e)
		{
			this.textBox1.Clear();
			this.tmpKey = e.KeyCode;
			this.textBox1.Text = Utilities.KeyToString(this.tmpKey);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			HotKeyActionItem hotKeyActionItem = null;
			if (this.comboBox1.SelectedIndex >= 0)
			{
				List<string> list = new List<string>();
				if (this.tbParm1.Text.Length > 0)
				{
					if (this.tbParm2.Text.Length < 1)
					{
						list.Add(this.tbParm1.Text);
						hotKeyActionItem = new HotKeyActionItem(this.comboBox1.SelectedIndex + HotKeyAction.None, list);
					}
					else
					{
						list.Add(this.tbParm1.Text);
						list.Add(this.tbParm2.Text);
						hotKeyActionItem = new HotKeyActionItem(this.comboBox1.SelectedIndex + HotKeyAction.None, list);
					}
				}
				else
				{
					hotKeyActionItem = new HotKeyActionItem(this.comboBox1.SelectedIndex + HotKeyAction.None, list);
				}
			}
			if (hotKeyActionItem != null)
			{
				this.actions.Add(hotKeyActionItem);
				this.listBox1.Items.Add(this.listBox1.Items.Count.ToString() + ": " + hotKeyActionItem.toString());
				return;
			}
			MessageBox.Show(this, "Some fields were invalid!", "Invalid Parameters");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (this.tmpKey == Keys.None)
			{
				MessageBox.Show(this, "You need to choose a key first!", "Invalid Hot Key");
				return;
			}
			if (this.actions.Count < 1)
			{
				MessageBox.Show(this, "You need to add at least one action to the list!", "Invalid action list");
				return;
			}
			if (!this.isEditing)
			{
				HotKeys.Instance.AddHotKey(this.tmpModKey, this.tmpKey, this.actions);
				base.DialogResult = DialogResult.OK;
				return;
			}
			int id = this.editingHotkey.id;
			if (id >= 0)
			{
				HotKeys.Instance.RemoveHotKey(id);
				HotKeys.Instance.AddHotKey(this.tmpModKey, this.tmpKey, this.actions);
				base.DialogResult = DialogResult.OK;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.tbParm1.Visible = true;
			this.comboBox2.Visible = false;
			this.comboBox4.Visible = false;
			switch (this.comboBox1.SelectedIndex)
			{
			case 6:
				this.tbParm1.Enabled = true;
				this.tbParm2.Enabled = true;
				this.tbParm1.Text = string.Empty;
				this.tbParm2.Text = string.Empty;
				this.label4.Text = "Width (px)";
				this.label5.Text = "Height (px)";
				return;
			case 7:
				this.tbParm1.Enabled = true;
				this.tbParm2.Enabled = true;
				this.tbParm1.Text = string.Empty;
				this.tbParm2.Text = string.Empty;
				this.label4.Text = "Width (%)";
				this.label5.Text = "Height (%)";
				return;
			case 9:
				this.tbParm1.Enabled = false;
				this.tbParm2.Enabled = false;
				this.tbParm1.Text = string.Empty;
				this.tbParm2.Text = string.Empty;
				this.label4.Text = "-";
				this.label5.Text = "-";
				return;
			case 10:
				this.tbParm1.Enabled = true;
				this.tbParm2.Enabled = false;
				this.tbParm1.Text = string.Empty;
				this.tbParm2.Text = string.Empty;
				this.label4.Text = "Chat Cmnd";
				this.label5.Text = "-";
				return;
			case 13:
				this.tbParm1.Enabled = true;
				this.tbParm2.Enabled = false;
				this.tbParm1.Text = "Center";
				this.tbParm2.Text = string.Empty;
				this.label4.Text = "Align To";
				this.tbParm1.Visible = false;
				this.comboBox2.Visible = true;
				this.label5.Text = "-";
				return;
			case 14:
				this.tbParm1.Enabled = true;
				this.tbParm2.Enabled = false;
				this.tbParm1.Text = "NoScale";
				this.tbParm2.Text = string.Empty;
				this.label4.Text = "Scale";
				this.tbParm1.Visible = false;
				this.comboBox4.Visible = true;
				this.label5.Text = "-";
				return;
			}
			this.tbParm1.Enabled = false;
			this.tbParm2.Enabled = false;
			this.tbParm1.Text = string.Empty;
			this.tbParm2.Text = string.Empty;
			this.label4.Text = "-";
			this.label5.Text = "-";
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.listBox1.SelectedIndex >= 0)
			{
				this.button7.Text = "Replace (" + this.listBox1.SelectedIndex.ToString() + ")";
				this.button7.Enabled = true;
				HotKeyActionItem hotKeyActionItem = this.actions[this.listBox1.SelectedIndex];
				this.comboBox1.SelectedIndex = (int)(hotKeyActionItem.Action + 1);
				if (hotKeyActionItem.Params.Count > 1)
				{
					this.tbParm1.Text = hotKeyActionItem.Params[0];
					this.tbParm2.Text = hotKeyActionItem.Params[1];
					return;
				}
				if (hotKeyActionItem.Params.Count == 1)
				{
					this.tbParm1.Text = hotKeyActionItem.Params[0];
					return;
				}
			}
			else
			{
				this.button7.Enabled = false;
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			HotKeyActionItem hotKeyActionItem = null;
			if (this.comboBox1.SelectedIndex >= 0)
			{
				List<string> list = new List<string>();
				if (this.tbParm1.Text.Length > 0)
				{
					if (this.tbParm2.Text.Length < 1)
					{
						list.Add(this.tbParm1.Text);
						hotKeyActionItem = new HotKeyActionItem(this.comboBox1.SelectedIndex + HotKeyAction.None, list);
					}
					else
					{
						list.Add(this.tbParm1.Text);
						list.Add(this.tbParm2.Text);
						hotKeyActionItem = new HotKeyActionItem(this.comboBox1.SelectedIndex + HotKeyAction.None, list);
					}
				}
				else
				{
					hotKeyActionItem = new HotKeyActionItem(this.comboBox1.SelectedIndex + HotKeyAction.None, list);
				}
			}
			if (hotKeyActionItem != null)
			{
				if (this.listBox1.SelectedIndex >= 0)
				{
					int selectedIndex = this.listBox1.SelectedIndex;
					this.actions.RemoveAt(selectedIndex);
					this.actions.Insert(selectedIndex, hotKeyActionItem);
					this.listBox1.Items.RemoveAt(selectedIndex);
					this.listBox1.Items.Insert(selectedIndex, selectedIndex.ToString() + ": " + hotKeyActionItem.toString());
					return;
				}
			}
			else
			{
				MessageBox.Show(this, "Some fields were invalid!", "Invalid Parameters");
			}
		}

		private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (this.comboBox3.SelectedIndex)
			{
			case 0:
				this.tmpModKey = Keys.None;
				return;
			case 1:
				this.tmpModKey = Keys.Control;
				return;
			case 2:
				this.tmpModKey = Keys.Alt;
				return;
			case 3:
				this.tmpModKey = Keys.Shift;
				return;
			default:
				return;
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			if (this.listBox1.SelectedIndex >= 0)
			{
				int selectedIndex = this.listBox1.SelectedIndex;
				this.actions.RemoveAt(selectedIndex);
				this.listBox1.Items.Clear();
				foreach (HotKeyActionItem hotKeyActionItem in this.actions)
				{
					this.listBox1.Items.Add(this.listBox1.Items.Count.ToString() + ": " + hotKeyActionItem.toString());
				}
			}
		}

		private void btMoveUp_Click(object sender, EventArgs e)
		{
			int selectedIndex = this.listBox1.SelectedIndex;
			if (selectedIndex > 0)
			{
				HotKeyActionItem item = this.actions[selectedIndex];
				this.actions.RemoveAt(selectedIndex);
				this.actions.Insert(selectedIndex - 1, item);
				this.listBox1.Items.Clear();
				foreach (HotKeyActionItem hotKeyActionItem in this.actions)
				{
					this.listBox1.Items.Add(this.listBox1.Items.Count.ToString() + ": " + hotKeyActionItem.toString());
				}
				this.listBox1.SelectedIndex = selectedIndex - 1;
			}
		}

		private void btMoveDown_Click(object sender, EventArgs e)
		{
			int selectedIndex = this.listBox1.SelectedIndex;
			if (selectedIndex >= 0 && selectedIndex < this.listBox1.Items.Count - 1)
			{
				HotKeyActionItem item = this.actions[selectedIndex];
				this.actions.RemoveAt(selectedIndex);
				this.actions.Insert(selectedIndex + 1, item);
				this.listBox1.Items.Clear();
				foreach (HotKeyActionItem hotKeyActionItem in this.actions)
				{
					this.listBox1.Items.Add(this.listBox1.Items.Count.ToString() + ": " + hotKeyActionItem.toString());
				}
				this.listBox1.SelectedIndex = selectedIndex + 1;
			}
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.tbParm1.Text = this.comboBox2.SelectedItem.ToString();
		}

		private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.comboBox4.SelectedIndex == 1)
			{
				this.tbParm1.Text = "ShowAll";
				return;
			}
			this.tbParm1.Text = "NoScale";
		}

		private void HotKeyForm_Load(object sender, EventArgs e) {}
	}
}
