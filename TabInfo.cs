using System;
using System.Windows.Forms;

namespace BafflerStandalone
{
	public class TabInfo
	{
		public ToolStripButton button;
		public FlashPlayer swf;
		public string swfUrl = string.Empty;
		public TabStatus status;
		
		public TabInfo(ToolStripButton Button, FlashPlayer flashObject)
		{
			this.button = Button;
			this.swf = flashObject;
		}

		public void LoadMovie(string url)
		{
			this.swfUrl = url;
			this.swf.Movie = url;
		}
	}
	
	public enum TabStatus
	{
		Loading,
		Ready
	}
}
