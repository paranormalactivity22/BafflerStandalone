namespace BafflerStandalone.Forms.ServerList
{
	public partial class ServerList : global::System.Windows.Forms.Form
	{
#pragma warning disable 0649
        private global::System.ComponentModel.IContainer components = null;
        private global::System.Windows.Forms.ListView lvServerList;
		private global::System.Windows.Forms.ColumnHeader columnHeader3;
		private global::System.Windows.Forms.ColumnHeader columnHeader4;
		private global::System.Windows.Forms.Panel panel1;
		private global::System.Windows.Forms.Button btnReload;
		private global::System.Windows.Forms.Button btnCheckAll;
		private global::System.Windows.Forms.Button btnDeleteAll;
		
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.lvServerList = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvServerList
            // 
            this.lvServerList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvServerList.FullRowSelect = true;
            this.lvServerList.GridLines = true;
            this.lvServerList.HideSelection = false;
            this.lvServerList.Location = new System.Drawing.Point(20, 16);
            this.lvServerList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lvServerList.MultiSelect = false;
            this.lvServerList.Name = "lvServerList";
            this.lvServerList.ShowGroups = false;
            this.lvServerList.Size = new System.Drawing.Size(597, 226);
            this.lvServerList.TabIndex = 3;
            this.lvServerList.UseCompatibleStateImageBehavior = false;
            this.lvServerList.View = System.Windows.Forms.View.Details;
            this.lvServerList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ListView1_MouseClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Server";
            this.columnHeader3.Width = 99;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "URL";
            this.columnHeader4.Width = 317;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lvServerList);
            this.panel1.Controls.Add(this.btnReload);
            this.panel1.Controls.Add(this.btnCheckAll);
            this.panel1.Controls.Add(this.btnDeleteAll);
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(639, 288);
            this.panel1.TabIndex = 12;
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(20, 250);
            this.btnReload.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(100, 28);
            this.btnReload.TabIndex = 13;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.ReloadBtn_Click);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(138, 250);
            this.btnCheckAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(100, 28);
            this.btnCheckAll.TabIndex = 13;
            this.btnCheckAll.Text = "Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.CheckAllBtn_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Location = new System.Drawing.Point(517, 250);
            this.btnDeleteAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(100, 28);
            this.btnDeleteAll.TabIndex = 13;
            this.btnDeleteAll.Text = "Delete All";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.DeleteAllBtn_Click);
            // 
            // ServerList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 300);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ServerList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server List - Baffler\'s Standalone";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
	}
}
