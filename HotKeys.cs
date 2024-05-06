using System;

namespace BafflerStandalone
{
	public class HotKeys
	{
		private static HotKeysManager instance = null;
		private static readonly object padlock = new object();
		
		private HotKeys() {}
        public static HotKeysManager Instance
        {
            get
            {
                HotKeysManager result;
                lock (HotKeys.padlock)
                {
                    if (HotKeys.instance == null)
                    {
                        HotKeys.instance = new HotKeysManager();
                    }
                    result = HotKeys.instance;
                }
                return result;
            }
        }
    }
}
