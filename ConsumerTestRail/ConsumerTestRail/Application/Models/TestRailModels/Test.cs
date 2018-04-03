using System;

namespace ConsumerTestRail.Application.Models.TestRailModels
{
	public class Test
	{
		public Test()
		{
			ID = -1;
			Title = "N/A";
			Reference = "N/A";
			Case = new Case();
			Type = new Type();
			Priority = new Priority();
			Area = new Area();
			Feature = new Feature();
			Status = new Status();
		}
		
		public Int32 ID { get; set; }
		public String Title { get; set; }
		public String Reference { get; set; }
		public Case Case { get; set; }
		public Type Type { get; set; }
		public Status Status { get; set; }
		public Priority Priority { get; set; }
		public Area Area { get; set; }
		public Feature Feature { get; set; }
	}
}
