using System;

namespace ConsumerTestRail.Application.Models.TestRailModels
{
	public class Feature
	{
		public Feature()
		{
			ID = -1;
			Name = "N/A";
		}
		
		public Int32 ID { get; set; }
		public String Name { get; set; }
	}
}
