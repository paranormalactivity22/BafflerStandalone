using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BafflerStandalone
{
	public class ServerInfo
	{
		public Button button;
		public string label;
		public string url;
		public BackgroundWorker threadedStatusCheck;
		public bool isOnline;
		
		public ServerInfo(Button sButton, string sLabel, string sUrl)
		{
			this.button = sButton;
			this.label = sLabel;
			this.url = sUrl;
			this.isOnline = false;
		}
	}
}
