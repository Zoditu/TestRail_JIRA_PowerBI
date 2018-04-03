using System;

namespace ConsumerTestRail.Application.Models.TestRailModels
{
	public class Milestone
	{
		public Milestone()
		{
			SprintName = "NO_MILESTONE";
		}
		
		public String SprintName { get; set; }
		
	}
}
