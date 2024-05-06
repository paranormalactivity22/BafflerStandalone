namespace BafflerStandalone.Database
{
	public class DbServers
	{
		private static MgrDBServerList instance = null;
		private static readonly object padlock = new object();
		private DbServers() {}

		public static MgrDBServerList Instance
		{
			get
			{
				MgrDBServerList result;
				lock (DbServers.padlock)
				{
					if (DbServers.instance == null)
					{
						DbServers.instance = new MgrDBServerList();
					}
					result = DbServers.instance;
				}
				return result;
			}
		}
	}
}
