﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ConsumerTestRail.Application.Helpers;
using ConsumerTestRail.Application.Models;
using ConsumerTestRail.Application.Models.TestRailModels;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop;

namespace ConsumerTestRail.Application.Controllers
{
	public class APIController : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		
		public void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
		}
		#endregion

		private readonly API api;
		private APICollector collector;
		private List<Thread> MultiThreadingProjects;
		protected internal readonly Boolean EnableMultitask;
		protected internal Int32 CurrentCallBack, MaxCallBack;
		protected internal Views.ConsoleLog ConsoleLog;
		protected internal DateTime _start, _end;
		protected Boolean IsSaving;
		public Boolean IsSilent { get; set; }
		public Boolean IsArgumentRunning { get; set; }
		public Boolean EnableConsoleInteraction { get; set; }
		
		public APIController()
		{
			ConsoleColor = Brushes.White;
			EnableConsoleInteraction = true;
			ConsoleBackground = new SolidColorBrush( Color.FromRgb(37, 37, 37) );			
			ConsoleLog = new Views.ConsoleLog()
			{
				DataContext = this
			};
			ConsoleLog.Show();
			
			api = new API();
			EnableMultitask = api.IsMultiTask;
			
			QueriedItems = new ObservableCollection<ReportRow>();
			BindingOperations.EnableCollectionSynchronization( QueriedItems, new Object() );
			
			Log = "**** Output Operation Log ****" + Environment.NewLine + Environment.NewLine;
			SetInfo( "Application started" );
			if( api.HasDefaultConfiguration )
				SetInfo( "WARNING! Configuration is set to default. Please configure the application before executing a fetch" );
			
			if( api.IsMultiTask )
			{
				MultiThreadingProjects = new List<Thread>();
				SetInfo( "INFO:: MultiTask is enabled in this application!" + Environment.NewLine );			
			}
			
			SaveLog( api.LogLocation );
 		}
		
		public void RunSilent()
		{
			IsSilent = true;
			FetchData();
			EnableConsoleInteraction = false;
			NotifyPropertyChanged( "EnableConsoleInteraction" );
		}
		
		protected void ClearConsoleInput()
		{
			ConsoleInput = String.Empty;
			NotifyPropertyChanged( "ConsoleInput" );
		}
		
		private Application.ActionCommand<CancelEventArgs> _closing;
		public Application.ActionCommand<CancelEventArgs> Closing
		{
			get
			{
				if( _closing == null )
				{
					_closing = new ActionCommand<CancelEventArgs>
						(
						 e =>
						 {
						 	if( System.Windows.MessageBox.Show( "Are you sure you want to close this application?", "Confirm Exit", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question )
						 	   == System.Windows.MessageBoxResult.Yes )
						 	{
						 		SaveLog( api.LogLocation );
						 		System.Windows.Application.Current.Shutdown( 0 );
						 	}
						 	else
						 		e.Cancel = true;
						 }
						);
				}
				return _closing;
			}
		}
		
		protected void SaveLog( String location )
		{
			try
			{
				if( IsSaving )
					return;
				IsSaving = true;
				System.IO.File.WriteAllText( location, Log );
				IsSaving = false;
			}
			catch
			{
				if( IsSaving )
					return;
				IsSaving = true;
				System.IO.File.WriteAllText( "default_log.txt", Log );
				IsSaving = false;
			}
		}
		
		public SolidColorBrush ConsoleColor
		{
			get;
			set;
		}
		public SolidColorBrush ConsoleBackground
		{
			get;
			set;
		}
		
		private void CommandError( String error, SolidColorBrush messageColor )
		{
			ConsoleColor = messageColor;
			NotifyPropertyChanged( "ConsoleColor" );
			ConsoleInput = error;
			NotifyPropertyChanged( "ConsoleInput" );
		}
		private Application.ActionCommand<KeyEventArgs> _commandWorker;
		public Application.ActionCommand<KeyEventArgs> CommandWorker
		{
			get
			{
				if( _commandWorker == null )
					_commandWorker = new ActionCommand<KeyEventArgs>
						( 
						 X =>
	                     {
						 	if( ConsoleColor != Brushes.White )
						 		ClearConsoleInput();
						 	
						 	ConsoleColor = Brushes.White;
						 	NotifyPropertyChanged( "ConsoleColor" );
						 	
						 	
						 	if( X.Key == Key.Enter || X.Key == Key.Return )
							 {
						 		var command = ConsoleInput;
						 		//exit
						 		if( Regex.IsMatch( command, "\\s*" + "exit".ToRegex() + "\\s*" ) )
						 		{
						 			SetInfo( "_> CMD:EXECUTE[ " + command + " ]" + Environment.NewLine );
						 			ConsoleLog.Close();
						 			return;
						 		}
						 		
							 	ClearConsoleInput();
						 		SetInfo( "_> CMD:EXECUTE[ " + command + " ]" + Environment.NewLine );
						 		#region Commands
						 		
						 		//Save last status log
						 		SaveLog( api.LogLocation );
						 		
						 		//fetch results
						 		if( Regex.IsMatch( command, "\\s*" + "fetch".ToRegex() + "\\s*" + "results".ToRegex() + "\\s*" ) )
						 		{
						 			if( IsProcessing )
						 				CommandError( "ERROR! There is already a TestRail+JIRA fetch process running", Brushes.OrangeRed );
						 			else
						 			{
						 				try
						 				{
							 				QueriedItems.Clear();
							 				FetchData();
						 				}
						 				catch( Exception ex )
						 				{
						 					SetInfo( "An Exception occured! Information, stack trace and details will be printed below: " + 
						 					        Environment.NewLine + Environment.NewLine + ex.Source + Environment.NewLine + 
						 					        ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace );
						 				}
						 			}
						 		}
						 		
						 		//run silent
						 		else if( Regex.IsMatch( command, "\\s*" + "run".ToRegex() + "\\s*" + "silent".ToRegex() + "\\s*" ) )
						 		{
						 			if( IsProcessing )
						 				CommandError( "ERROR! There is already a TestRail+JIRA fetch process running", Brushes.OrangeRed );
						 			else
						 			{
						 				try
						 				{
							 				QueriedItems.Clear();
							 				RunSilent();
						 				}
						 				catch( Exception ex )
						 				{
						 					SetInfo( "An Exception occured! Information, stack trace and details will be printed below: " + 
						 					        Environment.NewLine + Environment.NewLine + ex.Source + Environment.NewLine + 
						 					        ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace );
						 				}
						 			}
						 		}
						 		
						 		//export filename
						 		else if( Regex.IsMatch( command, "\\s*" + "export".ToRegex() + "\\s*" + "result".ToRegex() + "\\s*" ) )
						 		{
						 			if( IsProcessing )
						 				CommandError( "WARNING! Cannot export a result while there is a TestRail+JIRA fetch process running!", Brushes.Goldenrod );
						 			else
						 			{
						 				if( IsExporting )
						 					CommandError( "WARNING! Cannot export a result while there is a TestRail+JIRA result export running!", Brushes.OrangeRed );
					 					else
					 					{
					 						IsExporting = true;
					 						Task.Run( async () => 
					 						         {
					 						         	if( !(await ExportExcel( api.ExportLocation )) )
					 						         	{
					 						         		SetInfo( Logger.Log );
					 						         	}
					 						         	else 
					 						         		IsExporting = false;
					 						         });
					 					}
						 			}
															 			                                  
						 		}
						 		
						 		//stop
						 		else if( Regex.IsMatch( command, "\\s*" + "stop".ToRegex() + "\\s*" ) )
						 		{
						 			SetInfo( "Stopping all threads for projects run" + Environment.NewLine );
						 			MultiThreadingProjects.ForEach
						 				( 
						 				 x => 
						 				 {
						 				 	try
						 				 	{
						 				 		SetInfo( "Stopping Process:: " + x.ManagedThreadId.ToString( "X" ) );
						 				 		x.Interrupt();
						 				 	}
						 				 	catch
						 				 	{
						 				 		SetInfo( "Could not stop Process:: " + x.ManagedThreadId.ToString( "X" ) );
						 				 	}
						 				 });
						 			MultiThreadingProjects.Clear();
						 		}
						 		
						 		//reload config
						 		else if( Regex.IsMatch( command, "\\s*" + "reload".ToRegex() + "\\s+" + "config".ToRegex() + "\\s*" ) )
						 		{
						 			api.ReloadConfig();
						 			if( api.IsMultiTask )
									{
										MultiThreadingProjects = new List<Thread>();
										SetInfo( "INFO:: MultiTask is enabled in this application!" + Environment.NewLine );			
									}
						 			
						 			if( api.HasDefaultConfiguration )
										SetInfo( "WARNING! Configuration is set to default. Please configure the application before executing a fetch" );
						 			
						 			NotifyPropertyChanged( "Server" );
						 		}
						 		
						 		//show config
						 		else if( Regex.IsMatch( command, "\\s*" + "show".ToRegex() + "\\s+" + "config".ToRegex() + "\\s*" ) )
						 		{
						 			SetInfo( "Configuration: " + Environment.NewLine + api.ShowConfig() );
						 			if( api.IsMultiTask )
									{
										MultiThreadingProjects = new List<Thread>();
										SetInfo( "INFO:: MultiTask is enabled in this application!" + Environment.NewLine );			
									}
						 		}
						 		
						 		//config PROPERTY VALUE
						 		else if ( Regex.IsMatch( command, "\\s*" + "config".ToRegex() + "\\s+" + "\\w+" + "\\s+" + ".+" + "\\s*" ) )
						 		{
						 			var property_value = Regex.Replace( command, "\\s*" + "config".ToRegex() + "\\s+", String.Empty );
						 			var property = Regex.Split( property_value.Trim(), "\\s+" )[0];
						 			var value = property_value.Trim().Remove(0, property.Length );
						 			value = value.Trim();
						 			
						 			api.SetProperty( property, value );
						 			if( api.IsMultiTask )
									{
										MultiThreadingProjects = new List<Thread>();
										SetInfo( "INFO:: MultiTask is enabled in this application!" + Environment.NewLine );			
									}
						 		}
						 		
						 		else if( Regex.IsMatch( command, "\\s*" + "about".ToRegex() + "\\s*" ) )
						 		{
						 			Log += Environment.NewLine + Environment.NewLine + "" +
						 				"*****************************************************************" +
						 				Environment.NewLine + 
						 				"|            Created By: Kevin Martin del Campo Fdz             |" +
						 				Environment.NewLine +
						 				"*****************************************************************" +
						 				Environment.NewLine + Environment.NewLine;
						 			
						 			CurrentOperation = "Here comes ur god";
						 			NotifyPropertyChanged( "CurrentOperation" );
						 			NotifyPropertyChanged( "Log" );
						 		}
						 		
						 		//not a valid command
						 		else
						 		{
						 			CommandError( "ERROR! Command ['" + command + "'] is not a valid command" , Brushes.OrangeRed );
						 		}
						 		
						 		#endregion						 		
						 	}
	                     });
	
				return _commandWorker;
			}
		}
		
		private Task<Boolean> ExportExcel( String filelocation )
		{
			return Task.Run<Boolean>
			( 
				async () => 
				{
					SetInfo( "Preparing data to export file " + filelocation );
					if( QueriedItems.Count < 1 )
					{
						SetInfo( "There are not items to export" + Environment.NewLine );
						return false;
					}
					var data = QueriedItems.ToDataTable();
					SetInfo( "Exporting file..." );
					var result = await WorkbookCreation.WriteDataTableToExcel( data, filelocation );
					SetInfo( "Exported file " + filelocation );
					return result;
				}
			);
		}
		
		private void CallBackForProjectRun()
		{
			if( CurrentCallBack == MaxCallBack )
			{
				Log += Environment.NewLine + Environment.NewLine;
				SetInfo( "Operation finished" );
				SetInfo( "Total items fetched: " + QueriedItems.Count );
				SetInfo( "TIMEWATCH::_FINISH_TIME_ " + DateTime.Now.ToLongTimeString() );
				_end = DateTime.Now;
				SetInfo( "TIMEWATCH::TOTAL_TIME_RESULT_ " + (_end - _start) );
				IsProcessing = false;
				
				SetInfo( "Global Log: " + Environment.NewLine  + Environment.NewLine + Logger.Log );
				
				if( QueriedItems.Count >  0 )
					QueriedItems[ QueriedItems.Count - 1 ].FileCreationDate = DateTime.Now.ToString();
				
				if( IsSilent )
				{
					SaveLog( api.LogLocation );
					var result = ExportExcel( api.ExportLocation ).Result;
					EnableConsoleInteraction = true;
					NotifyPropertyChanged( "EnableConsoleInteraction" );
					IsSilent = false;
				}
			}
			
			SaveLog( api.LogLocation );
		}
		
		private void ProcessProject( Project p )
		{
			try
			{
				SetInfo( "Requesting runs for project [" + p.Name + "]" );
				var runs = collector.GetRuns( p.ID );
				SetInfo( "Parsing runs for project [" + p.Name + "]" );
				foreach( var r in runs )
					ProcessRun( p, r );
			}
			catch( Exception ex )
			{
				SetInfo( "An error occured for project [" + p.Name + "]. StackTrace will be printed below: " + Environment.NewLine + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace );
			}
			finally
			{
				CurrentCallBack++;
				CallBackForProjectRun();
			}
		}
		
		private Task ProcessProjectTask( Project p )
		{
			return Task.Run
			( 
				 async () =>
				 {
					try
					{
						SetInfo( "Requesting runs for project [" + p.Name + "]" );
						var runs = await collector.GetRunsAsync( p.ID );
						SetInfo( "Parsing runs for project [" + p.Name + "]" );
						foreach( var r in runs )
							await ProcessRunTask( p, r );
					}
					catch( Exception ex )
					{
						SetInfo( "An error occured for project [" + p.Name + "]. StackTrace will be printed below: " + Environment.NewLine + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace );
					}
					finally
					{
						CurrentCallBack++;
						CallBackForProjectRun();
					}
				 }
			);
		}
		
		private void ProcessRun( Project p, Run r )
		{
			SetInfo( "Fetch run [" + r.Name + "] ::ID-{" + r.ID + "}" );
			var results = collector.GetResultsForRun( r.ID );
			if( results == null ) return;
			foreach( var result in results )
			{
				if( result == null ) continue;
				SetInfo( "Processing result ID::{" + result.ID+ "}" );
				
				if( result.Defects != "No defects" )
					FetchIssue( p, r, result );
				else
					ProcessRow( p, r, result, new Models.JiraModels.Issue() );
				
				NotifyPropertyChanged( "QueriedItems" );
			}
		}
		
		private Task ProcessRunTask( Project p, Run r )
		{
			return Task.Run
			(
				async () =>
				{
					SetInfo( "Fetch run [" + r.Name + "] ::ID-{" + r.ID + "}" );
					var results = await collector.GetResultsForRunAsync( r.ID );
					if( results == null ) return;
					foreach( var result in results )
					{
						if( result == null ) continue;
						SetInfo( "Processing result ID::{" + result.ID+ "}" );
						if( result.Defects != "No defects" )
							await FetchIssueTask( p, r, result );
						else
							await ProcessRowTask( p, r, result, new Models.JiraModels.Issue() );
						
						NotifyPropertyChanged( "QueriedItems" );
					}
				}
					
			);
		}
		
		public void FetchIssue( Project p, Run r, Result result )
		{
			var issues = result.Defects.Split( ',' );
			foreach( var issue in issues )
			{
				var bug = collector.GetIssue( issue.Trim() );
				ProcessRow( p, r, result, bug );
			}
		}
		
		public Task FetchIssueTask( Project p, Run r, Result result )
		{
			return Task.Run
			(
				async () =>
				{
					var issues = result.Defects.Split( ',' );
					foreach( var issue in issues )
					{
						var bug = await collector.GetIssueAsync( issue.Trim() );
						await ProcessRowTask( p, r, result, bug );
					}
				}
			);
			
		}
		
		private void ProcessRow( Project p, Run r, Result result, Models.JiraModels.Issue issue)
		{
			QueriedItems.Add( new ReportRow()
			{
				ProjectID = p.ID,
				ProjectName = p.Name,
				SuiteID = r.Suite.ID,
				Milestone = r.Milestone.SprintName,
				Run = r.Name,
				IsCompleted = r.IsCompleted,
				TestID = "T" + result.Test.ID,
				CaseID = "C" + result.Test.Case.ID,
				Case = result.Test.Case.Title,
				Defects = result.Defects,
				TestedBy = result.TestedBy.Name,
				TestedOn = result.TestedOn.ToString(),
				Status = result.Status.Name,
				Priority = result.Test.Priority.Name,
				Area = result.Test.Area.Name,
				Feature = result.Test.Feature.Name,
				Reference = result.Test.Reference,
				Type = result.Test.Type.Name,
				RunID = "R" + r.ID,
				
				Bug_Key = issue.Key,
				Bug_ProjectName = issue.ProjectName,
				Bug_CreatedBy = issue.Creator,
				Bug_CreatedOn = issue.Created.ToString(),
				Bug_Resolution = issue.Resolution.ToString(),
				Bug_Status = issue.Status,
				Bug_Priority = issue.Priority
		    });
		}
		
		private Task ProcessRowTask( Project p, Run r, Result result, Models.JiraModels.Issue issue)
		{
			return Task.Run
			(
				() =>
					QueriedItems.Add( new ReportRow()
					{
						ProjectID = p.ID,
						ProjectName = p.Name,
						SuiteID = r.Suite.ID,
						Milestone = r.Milestone.SprintName,
						Run = r.Name,
						IsCompleted = r.IsCompleted,
						TestID = "T" + result.Test.ID,
						CaseID = "C" + result.Test.Case.ID,
						Case = result.Test.Case.Title,
						Defects = result.Defects,
						TestedBy = result.TestedBy.Name,
						TestedOn = result.TestedOn.ToString(),
						Status = result.Status.Name,
						Priority = result.Test.Priority.Name,
						Area = result.Test.Area.Name,
						Feature = result.Test.Feature.Name,
						Reference = result.Test.Reference,
						Type = result.Test.Type.Name,
						RunID = "R" + r.ID,
							
						Bug_Key = issue.Key,
						Bug_ProjectName = issue.ProjectName,
						Bug_CreatedBy = issue.Creator,
						Bug_CreatedOn = issue.Created.ToString(),
						Bug_Resolution = issue.Resolution.ToString(),
						Bug_Status = issue.Status,
						Bug_Priority = issue.Priority
	                })
			);
			
		}
		
		private async void FetchData()
		{			
			IsProcessing = true;
			SetInfo( "Initializing TestRail WebService connection" );
			try
			{
				if( collector == null )
					collector = new APICollector( api );
			}
			catch
			{
				SetInfo( "ERROR::Could not establish a connection to the WebService" + Environment.NewLine );
				IsProcessing = false;
				return;
			}
			try
			{
				_start = DateTime.Now;
				SetInfo( Environment.NewLine + "TIMEWATCH::_START_TIME_ " + _start.ToLongTimeString() );
				SetInfo( "Started to fetch TestRail Projects" );
				var projects = await collector.GetProjectsAsync();
				SetInfo( "Projects fetch { Found [" + projects.Count() + "] projects }" + Environment.NewLine );	
				CurrentCallBack = 0;
				MaxCallBack = projects.Count();
				foreach( var p in projects )
				{
					if( EnableMultitask )
					{	
						Thread processProject = new Thread( () => ProcessProject( p )  )
						{ 
							Priority = ThreadPriority.Highest,
							IsBackground = true
						};
						
						processProject.TrySetApartmentState( ApartmentState.MTA );
						MultiThreadingProjects.Add( processProject );
						processProject.Start();
					}
					else
					{
						await ProcessProjectTask( p );
					}				
				}
			}
			catch( Exception ex )
			{
				SetInfo( "EXCEPTION::Could not establish a connection to the WebService. Current connections are '" + api.TestRailServer + "' and '" + api.JiraServer + "'" );
				SetInfo( "EXCEPTION::" + ex.Source + Environment.NewLine + ex.Message );
				SetInfo( "Global Log: " + Environment.NewLine + Environment.NewLine + Logger.Log );
				IsProcessing = false;
				return;
			}
		}
		
		public String Username
		{
			get
			{
				return api.Username;
			}
		}
		
		public String TestRailServer
		{
			get
			{
				return api.TestRailServer;
			}
		}
		
		public String JiraServer
		{
			get
			{
				return api.JiraServer;
			}
		}
		
		public String Server
		{
			get
			{
				return TestRailServer + ", " + JiraServer;
			}
		}
		
		private String _infoMessage;
		public String CurrentOperation
		{
			get
			{
				return "Info [" + DateTime.Now.ToLongTimeString() + "]: " + _infoMessage;
			}
			
			set
			{
				_infoMessage = value;
			}
		}
		
		public String Log
		{
			get;
			set;
		}
		
		public String ConsoleInput
		{
			get;
			set;
		}
		
		public Boolean IsProcessing
		{
			get;
			set;
		}
		
		public Boolean IsExporting
		{
			get;
			set;
		}
		
		public ObservableCollection<ReportRow> QueriedItems {
			get;
			set;
		}
		
		public void SetInfo( String info )
		{
			CurrentOperation = info;
			Log += CurrentOperation + Environment.NewLine;
			Console.WriteLine( CurrentOperation );
			NotifyPropertyChanged( "Log" );
			NotifyPropertyChanged( "CurrentOperation" );
		}
	}
}
