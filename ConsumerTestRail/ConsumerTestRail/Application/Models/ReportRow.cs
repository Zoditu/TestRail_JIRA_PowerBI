using System;

namespace ConsumerTestRail.Application.Models
{	
	public class ReportRow
	{		
		public ReportRow()
		{
			TestID = "-1";
			Defects = "N/A";
			Status = "N/A";
			TestedBy = "N/A";
			TestedOn = "N/A";
			CaseID = "C-1";
			Reference = "N/A";
			Priority = "N/A";
			Area = "N/A";
			Feature = "N/A";
			Type = "N/A";
			Run = "N/A";
			SuiteID = -1;
			IsCompleted = false;
			ProjectID = -1;
			ProjectName = "N/A";
		}
		
		public String TestID { get; set; }
		public String Defects { get; set; }
		public String Status { get; set; }
		public String TestedBy { get; set; }
		public String TestedOn { get; set; }
		public String CaseID { get; set; }
		public String Case { get; set; }
		public String Reference { get; set; }
		public String Priority { get; set; }
		public String Area { get; set; }
		public String Feature { get; set; }
		public String Type { get; set; }
		public String Run { get; set; }
		public String RunID { get; set; }
		public String Milestone { get; set; }
		public Int32 SuiteID { get; set; }
		public Boolean IsCompleted { get; set; }
		public Int32 ProjectID { get; set; }
		public String ProjectName { get; set; }
		
		public String Bug_Key { get; set; }
		public String Bug_ProjectName { get; set; }
		public String Bug_Resolution { get; set; }
		public String Bug_CreatedBy { get; set; }
		public String Bug_Priority { get; set; }
		public String Bug_Status { get; set; }
		public String Bug_CreatedOn { get; set; }
		public String FileCreationDate { get; set; }
	}
}
