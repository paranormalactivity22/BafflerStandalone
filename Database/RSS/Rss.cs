using System;

namespace BafflerStandalone.Database.RSS
{
	public class Rss
	{
		[Serializable]
		public struct Items
		{
			public DateTime Date;
			public string Title;
			public string Description;
			public string Link;
			public string Comments;
		}
	}
}
