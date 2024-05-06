namespace BafflerStandalone.Forms.QuickAlign
{
	public partial class QuickAlign : global::System.Windows.Forms.Form
	{
#pragma warning disable 0649
		private global::System.ComponentModel.IContainer components = null;
		private global::System.Windows.Forms.Button button1;
		private global::System.Windows.Forms.Button button2;
		private global::System.Windows.Forms.Button button3;
		private global::System.Windows.Forms.Button button4;
		private global::System.Windows.Forms.Button button5;
		private global::System.Windows.Forms.Button button6;
		private global::System.Windows.Forms.Button button7;
		private global::System.Windows.Forms.Button button8;
		private global::System.Windows.Forms.Button button9;
		
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
			this.button1 = new global::System.Windows.Forms.Button();
			this.button2 = new global::System.Windows.Forms.Button();
			this.button3 = new global::System.Windows.Forms.Button();
			this.button4 = new global::System.Windows.Forms.Button();
			this.button5 = new global::System.Windows.Forms.Button();
			this.button6 = new global::System.Windows.Forms.Button();
			this.button7 = new global::System.Windows.Forms.Button();
			this.button8 = new global::System.Windows.Forms.Button();
			this.button9 = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.button1.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new global::System.Drawing.Point(12, 12);
			this.button1.Name = "button1";
			this.button1.Size = new global::System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "TopLeft";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new global::System.EventHandler(this.button1_Click);
			this.button2.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button2.Location = new global::System.Drawing.Point(93, 12);
			this.button2.Name = "button2";
			this.button2.Size = new global::System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "Top";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new global::System.EventHandler(this.button2_Click);
			this.button3.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button3.Location = new global::System.Drawing.Point(174, 12);
			this.button3.Name = "button3";
			this.button3.Size = new global::System.Drawing.Size(75, 23);
			this.button3.TabIndex = 2;
			this.button3.Text = "TopRight";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new global::System.EventHandler(this.button3_Click);
			this.button4.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button4.Location = new global::System.Drawing.Point(12, 41);
			this.button4.Name = "button4";
			this.button4.Size = new global::System.Drawing.Size(75, 23);
			this.button4.TabIndex = 3;
			this.button4.Text = "Left";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new global::System.EventHandler(this.button4_Click);
			this.button5.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button5.Location = new global::System.Drawing.Point(93, 41);
			this.button5.Name = "button5";
			this.button5.Size = new global::System.Drawing.Size(75, 23);
			this.button5.TabIndex = 4;
			this.button5.Text = "Center";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new global::System.EventHandler(this.button5_Click);
			this.button6.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button6.Location = new global::System.Drawing.Point(174, 41);
			this.button6.Name = "button6";
			this.button6.Size = new global::System.Drawing.Size(75, 23);
			this.button6.TabIndex = 5;
			this.button6.Text = "Right";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new global::System.EventHandler(this.button6_Click);
			this.button7.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button7.Location = new global::System.Drawing.Point(12, 70);
			this.button7.Name = "button7";
			this.button7.Size = new global::System.Drawing.Size(75, 23);
			this.button7.TabIndex = 6;
			this.button7.Text = "BottomLeft";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new global::System.EventHandler(this.button7_Click);
			this.button8.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button8.Location = new global::System.Drawing.Point(93, 70);
			this.button8.Name = "button8";
			this.button8.Size = new global::System.Drawing.Size(75, 23);
			this.button8.TabIndex = 7;
			this.button8.Text = "Bottom";
			this.button8.UseVisualStyleBackColor = true;
			this.button8.Click += new global::System.EventHandler(this.button8_Click);
			this.button9.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button9.Location = new global::System.Drawing.Point(174, 70);
			this.button9.Name = "button9";
			this.button9.Size = new global::System.Drawing.Size(75, 23);
			this.button9.TabIndex = 8;
			this.button9.Text = "BottomRight";
			this.button9.UseVisualStyleBackColor = true;
			this.button9.Click += new global::System.EventHandler(this.button9_Click);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(262, 105);
			base.Controls.Add(this.button9);
			base.Controls.Add(this.button8);
			base.Controls.Add(this.button7);
			base.Controls.Add(this.button6);
			base.Controls.Add(this.button5);
			base.Controls.Add(this.button4);
			base.Controls.Add(this.button3);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.button1);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "QuickAlign";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set Alignment - Baffler's Standalone";
			base.ResumeLayout(false);
		}
	}
}
