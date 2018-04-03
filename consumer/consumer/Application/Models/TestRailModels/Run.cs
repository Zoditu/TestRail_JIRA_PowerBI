using System;

namespace ConsumerTestRail.Application.Models.TestRailModels
{
	public class Run
	{
		public Run()
		{
		}
		
		public Int32 ID { get; set; }
		public Suite Suite { get; set; }
		public String Name { get; set; }
		public Milestone Milestone { get; set; }
		public Boolean IsCompleted { get; set; }
	}
}
