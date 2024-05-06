using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BafflerStandalone.Forms.QuickAlign
{
	public partial class QuickAlign : Form
	{
		public QuickAlign()
		{
			this.InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.Alignment = 5;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.Alignment = 4;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.Alignment = 6;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.Alignment = 1;
		}

		private void button5_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.Alignment = 0;
		}

		private void button6_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.Alignment = 2;
		}

		private void button7_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.Alignment = 9;
		}

		private void button8_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.Alignment = 8;
		}

		private void button9_Click(object sender, EventArgs e)
		{
			Database.Database.Instance.Alignment = 10;
		}
	}
}
