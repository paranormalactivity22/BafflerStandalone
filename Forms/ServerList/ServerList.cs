using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using BafflerStandalone.Database;
using BafflerStandalone.Forms.ServerDialog;

namespace BafflerStandalone.Forms.ServerList
{
	public partial class ServerList : Form
	{
		private ServerDialog.ServerDialog srvDialog;

		public ServerList()
		{
			this.InitializeComponent();
		}

		public void LoadList()
		{
			this.lvServerList.Items.Clear();
			foreach (string text in DbServers.Instance.List.Keys)
			{
				ListViewItem listViewItem = new ListViewItem(text);
				listViewItem.SubItems.Add(DbServers.Instance.List[text]);
				this.lvServerList.Items.Add(listViewItem);
			}
		}

		// Menu Buttons
		private void ConnectMenuItem_Click(object sender, EventArgs e)
		{
			if (this.lvServerList.SelectedItems.Count == 1)
			{
				string text = this.lvServerList.SelectedItems[0].Text;
				string currentServerURL = string.Empty;
				if (this.lvServerList.SelectedItems[0].SubItems.Count > 1)
				{
					currentServerURL = this.lvServerList.SelectedItems[0].SubItems[1].Text;
				}
				DbServers.Instance.currentServerLabel = text;
				DbServers.Instance.currentServerURL = currentServerURL;
				base.DialogResult = DialogResult.OK;
			}
		}

		private void CheckMenuItem_Click(object sender, EventArgs e)
		{
			if (this.lvServerList.SelectedItems.Count == 1)
			{
				if (this.lvServerList.SelectedItems[0].SubItems.Count > 1)
				{
					string currentServerURL = this.lvServerList.SelectedItems[0].SubItems[1].Text;
					this.CheckURL(currentServerURL, true);
				}
			}
		}

		private void EditMenuItem_Click(object sender, EventArgs e)
		{
			if (this.lvServerList.SelectedItems.Count == 1)
			{
				string text = this.lvServerList.SelectedItems[0].Text;
				this.srvDialog = new ServerDialog.ServerDialog();
				this.srvDialog.disableServNameTextBox(text);
				if (this.srvDialog.ShowDialog() == DialogResult.OK)
				{
					this.LoadList();
				}
			}
		}

		private void DeleteMenuItem_Click(object sender, EventArgs e)
		{
			if (this.lvServerList.SelectedItems.Count == 1)
			{
				string text = this.lvServerList.SelectedItems[0].Text;
				if (MessageBox.Show(this, "Are you sure you want to delete '" + text + "'?", "Baffler's Standalone", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					DbServers.Instance.RemoveServer(text);
					this.lvServerList.SelectedItems[0].Remove();
				}
			}
		}

		private void NewMenuItem_Click(object sender, EventArgs e)
		{
			this.srvDialog = new ServerDialog.ServerDialog();
			if (this.srvDialog.ShowDialog() == DialogResult.OK)
			{
				this.LoadList();
			}
		}

		// Form Buttons
		private void ReloadBtn_Click(object sender, EventArgs e)
		{
			this.LoadList();
		}

		private void CheckAllBtn_Click(object sender, EventArgs e)
        {
			int counter = 0;
			for(int i = 0; i < this.lvServerList.Items.Count; i++)
            {
				string url = this.lvServerList.Items[i].SubItems[1].Text;
				if (this.CheckURL(url, false)) counter++;
			}
			MessageBox.Show(this, "Total Working Websites: " + counter, "Baffler's Standalone", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

		private void DeleteAllBtn_Click(object sender, EventArgs e)
        {
			if (MessageBox.Show(this, "Are you sure you want to delete all connections?", "Baffler's Standalone", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				if(DbServers.Instance.List.Count == 0)
				{
					MessageBox.Show(this, "There are no servers to delete.", "Baffler's Standalone", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				DbServers.Instance.RemoveAllServers();
				DbServers.Instance.List.Clear();
				this.LoadList();
			}
        }

		private void ListView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ContextMenu m = new ContextMenu();
				if (this.lvServerList.FocusedItem != null && this.lvServerList.FocusedItem.Bounds.Contains(e.Location) == true)
				{
					/// Connect Button (Edits the information).
					MenuItem connectMenuItem = new MenuItem("Connect");
					connectMenuItem.Click += new global::System.EventHandler(this.ConnectMenuItem_Click);
					m.MenuItems.Add(connectMenuItem);

					/// Check Button (Checks the connection).
					MenuItem checkMenuItem = new MenuItem("Check");
					checkMenuItem.Click += new global::System.EventHandler(this.CheckMenuItem_Click);
					m.MenuItems.Add(checkMenuItem);
					m.MenuItems.Add("-"); // poor way for separator.

					/// Edit Button (Edits the information).
					MenuItem editMenuItem = new MenuItem("Edit");
					editMenuItem.Click += new global::System.EventHandler(this.EditMenuItem_Click);
					m.MenuItems.Add(editMenuItem);

					/// Delete Button (Deletes the information).
					MenuItem deleteMenuItem = new MenuItem("Delete");
					deleteMenuItem.Click += new global::System.EventHandler(this.DeleteMenuItem_Click);
					m.MenuItems.Add(deleteMenuItem);
				}
				else
                {
					/// New Button (Creates the information).
					MenuItem newMenuItem = new MenuItem("New");
					newMenuItem.Click += new global::System.EventHandler(this.NewMenuItem_Click);
					m.MenuItems.Add(newMenuItem);
				}
				m.Show(this.lvServerList, new Point(e.X, e.Y));
			}
		}

		// Utils

		private bool CheckURL(string url, bool showBoxes)
		{
			try
            {
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Timeout = 10000; // 10 seconds
				request.Method = "HEAD";
				try
				{
					using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
					{
						if (response.StatusCode == HttpStatusCode.OK)
						{
							if (showBoxes) MessageBox.Show(this, "The Website is working.", "Baffler's Standalone", MessageBoxButtons.OK, MessageBoxIcon.Information);
							return true;
						}
					}
					return false;
				}
				catch (WebException ex)
				{
					if (showBoxes) MessageBox.Show(this, ex.Message, "Baffler's Standalone", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}
			catch (System.UriFormatException)
			{
				if (showBoxes) MessageBox.Show(this, "The url is invalid.", "Baffler's Standalone", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
	}
}
