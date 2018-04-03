using System;

namespace ConsumerTestRail.Application.Models.TestRailModels
{
	public class Area
	{
		public Area()
		{
			Name = "N/A";
			ID = -1;
		}
		
		public Int32 ID { get; set; }
		public String Name { get; set; }
	}
}
