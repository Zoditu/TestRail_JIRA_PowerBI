using System;

namespace ConsumerTestRail.Application.Models.TestRailModels
{
	public class Result
	{
		public Result()
		{
		}
		
		public Int32 ID { get; set; }
		public Test Test { get; set; }
		public Status Status { get; set; }
		public User TestedBy { get; set; }
		public DateTime TestedOn { get; set; }
		public String Defects { get; set; }
	}
}
