using System;
using AxShockwaveFlashObjects;

namespace BafflerStandalone
{
	public class FlashPlayer : AxShockwaveFlash
	{
		//private const int WM_RBUTTONDOWN = 516;
		//private const int WM_RBUTTONUP = 517;
		
		public FlashPlayer()
		{
			base.HandleCreated += this.FlashPlayer_HandleCreated;
		}

		private void FlashPlayer_HandleCreated(object sender, EventArgs e)
		{
			this.AllowFullScreen = "true";
			this.AllowNetworking = "all";
			this.AllowScriptAccess = "always";
		}
	}
}
