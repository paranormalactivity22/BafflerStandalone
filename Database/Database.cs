using System;

namespace BafflerStandalone.Database
{
	public sealed class Database
	{
		private static DatabaseManager instance = null;
		private static readonly object padlock = new object();
		private Database() {}

		public static DatabaseManager Instance
		{
			get
			{
				DatabaseManager result;
				lock (Database.padlock)
				{
					if (Database.instance == null)
					{
						Database.instance = new DatabaseManager();
					}
					result = Database.instance;
				}
				return result;
			}
		}
	}
}
