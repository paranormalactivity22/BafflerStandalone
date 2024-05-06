namespace BafflerStandalone.Forms.ServerDialog
{
	public partial class ServerDialog : global::System.Windows.Forms.Form
	{
#pragma warning disable 0649
		private global::System.ComponentModel.IContainer components = null;
		private global::System.Windows.Forms.Button button2;
		private global::System.Windows.Forms.Button button1;
		private global::System.Windows.Forms.TextBox textBox2;
		private global::System.Windows.Forms.TextBox textBox1;
		private global::System.Windows.Forms.Label label2;
		private global::System.Windows.Forms.Label label1;
		
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
			this.button2 = new global::System.Windows.Forms.Button();
			this.button1 = new global::System.Windows.Forms.Button();
			this.textBox2 = new global::System.Windows.Forms.TextBox();
			this.textBox1 = new global::System.Windows.Forms.TextBox();
			this.label2 = new global::System.Windows.Forms.Label();
			this.label1 = new global::System.Windows.Forms.Label();
			base.SuspendLayout();
			this.button2.Location = new global::System.Drawing.Point(155, 92);
			this.button2.Name = "button2";
			this.button2.Size = new global::System.Drawing.Size(75, 23);
			this.button2.TabIndex = 11;
			this.button2.Text = "OK";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new global::System.EventHandler(this.button2_Click);
			this.button1.Location = new global::System.Drawing.Point(236, 92);
			this.button1.Name = "button1";
			this.button1.Size = new global::System.Drawing.Size(75, 23);
			this.button1.TabIndex = 10;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new global::System.EventHandler(this.button1_Click);
			this.textBox2.Location = new global::System.Drawing.Point(12, 66);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new global::System.Drawing.Size(299, 20);
			this.textBox2.TabIndex = 9;
			this.textBox1.Location = new global::System.Drawing.Point(12, 22);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new global::System.Drawing.Size(135, 20);
			this.textBox1.TabIndex = 8;
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(9, 50);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(54, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Server Url";
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(9, 6);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(69, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Server Name";
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(325, 122);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.textBox2);
			base.Controls.Add(this.textBox1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "ServerDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configure Server - Baffler's Standalone";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
