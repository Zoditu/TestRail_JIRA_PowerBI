using System;

namespace ConsumerTestRail.Application.Models.TestRailModels
{
	public class Type
	{
		public Type()
		{
			ID = -1;
			Name = "Unknown";
		}
		
		public Int32 ID { get; set; }
		public String Name { get; set; }
	}
}
