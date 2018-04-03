using System;

namespace ConsumerTestRail.Application.Models.JiraModels
{
	public class Issue
	{
		public Issue()
		{
			Key = "N/A";
			ProjectName = "N/A";
			Resolution = DateTime.MinValue;
			Created = DateTime.MinValue;
			Priority = "N/A";
			Status = "N/A";
			Creator = "N/A";
		}
		
		public String Key { get; set; }
		public String ProjectName { get; set; }
		public DateTime Resolution { get; set; }
		public DateTime Created { get; set; }
		public String Priority { get; set; }
		public String Status { get; set; }
		public String Creator { get; set; }
	}
}
