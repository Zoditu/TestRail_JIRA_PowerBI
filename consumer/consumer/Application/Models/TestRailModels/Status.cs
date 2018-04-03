using System;

namespace ConsumerTestRail.Application.Models.TestRailModels
{
	public class Status
	{
		public Status()
		{
			Name = "Unknown";
			ID = -1;
		}
		
		public String Name { get; set; }
		public Int32 ID { get; set; }
	}
}
