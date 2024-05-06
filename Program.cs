using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace BafflerStandalone
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
