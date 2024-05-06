using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BafflerStandalone.Database;

namespace BafflerStandalone.Forms.ServerDialog
{
	public partial class ServerDialog : Form
	{
		public ServerDialog()
		{
			this.InitializeComponent();
		}

		public void disableServNameTextBox(string txtKey)
		{
			this.textBox1.Text = txtKey;
			this.textBox1.Enabled = false;
			if (DbServers.Instance.List.ContainsKey(txtKey))
			{
				this.textBox2.Text = DbServers.Instance.List[txtKey];
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (this.textBox1.Text.Length < 1 && this.textBox2.Text.Length < 1)
			{
				MessageBox.Show("Some fields are empty.");
				return;
			}
			if (!this.textBox1.Enabled)
			{
				if (!DbServers.Instance.List.ContainsKey(this.textBox1.Text))
				{
					MessageBox.Show("Cannot edit server. Invalid key.");
					return;
				}
				DbServers.Instance.EditServer(this.textBox1.Text, this.textBox2.Text);
			}
			else
			{
				if (DbServers.Instance.List.ContainsKey(this.textBox1.Text))
				{
					MessageBox.Show("A server with that name already exists.");
					return;
				}
				DbServers.Instance.AddServer(this.textBox1.Text, this.textBox2.Text);
			}
			base.DialogResult = DialogResult.OK;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}
	}
}
