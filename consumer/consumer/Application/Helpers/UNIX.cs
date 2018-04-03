using System;

namespace ConsumerTestRail.Application.Helpers
{
	public class UNIX
	{
		public UNIX()
		{
		}
		
		public static DateTime FromUnixTime(long unixTime)
		{
		    return epoch.AddSeconds(unixTime);
		}
		private static readonly DateTime epoch = new DateTime(1969, 12, 31, 16, 0, 0, DateTimeKind.Utc);
	}
}
