using System;
using System.Collections.Generic;
using System.Linq;
using ConsumerTestRail.Application.Models.TestRailModels;
using ConsumerTestRail.Application.Models.JiraModels;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ConsumerTestRail.Application.Helpers
{
	public class APICollector
	{
		private readonly API _api;
		//Use this contant to validate untested tests
		const Int32 UNTESTED = 3;
		private IEnumerable<Status> _globalStatuses;
		private IEnumerable<Status> GlobalStatuses
		{
			get
			{
				if( _globalStatuses == null )
					_globalStatuses = GetStatuses();
				
				return _globalStatuses;
			}
		}
		
		private IEnumerable<Priority> _globalPriorities;
		private IEnumerable<Priority> GlobalPriorities
		{
			get
			{
				if( _globalPriorities == null )
					_globalPriorities = GetPriorities();
				
				return _globalPriorities;
			}
		}
		
		private IEnumerable<ConsumerTestRail.Application.Models.TestRailModels.Type> _globalCaseTypes;
		public IEnumerable<ConsumerTestRail.Application.Models.TestRailModels.Type> GlobalCaseTypes
		{
			get
			{
				if( _globalCaseTypes == null )
					_globalCaseTypes = GetCaseTypes();
				
				return _globalCaseTypes;
			}
		}
		
		private IEnumerable<Area> _globalAreas;
		public IEnumerable<Area> GlobalAreas
		{
			get
			{
				if( _globalAreas == null )
					_globalAreas = GetAreas();
				
				return _globalAreas;
			}
		}
		
		private IEnumerable<Feature> _globalFeatures;
		public IEnumerable<Feature> GlobalFeatures
		{
			get
			{
				if( _globalFeatures == null )
					_globalFeatures = GetFeatures();
				
				return _globalFeatures;
			}
		}
		
		private IEnumerable<User> _globalUsers;
		public IEnumerable<User> GlobalUsers
		{
			get
			{
				if( _globalUsers == null )
					_globalUsers = GetUsers();
				
				return _globalUsers;
			}
		}
		
		public APICollector( API api )
		{
			_api = api;
		}
		
		//JIRA API Implementation
		
		public Issue GetIssue( String key )
		{
			try
			{
			var ticket = (JObject)_api.JiraAPI.SendGet( "rest/api/latest/issue/" + key );
				var fields = ticket["fields"];
				
				return new Issue()
				{
					Key = ticket["key"].Value<String>(),
					ProjectName = fields["project"]["name"].Value<String>() ?? "N/A",
					Resolution = fields["resolutiondate"].Value<DateTime?>().HasValue ? fields["resolutiondate"].Value<DateTime>() : DateTime.MinValue,
					Status = fields["status"]["name"].Value<String>() ?? "N/A",
					Priority = fields["priority"]["name"].Value<String>() ?? "N/A",
					Creator = fields["creator"]["displayName"].Value<String>(),
					Created = fields["created"].Value<DateTime>()
				};
			}
			catch
			{
				return new Issue();
			}
		}
		
		public async Task<Issue> GetIssueAsync( String key )
		{
			return await Task.Run<Issue>
			(
				() =>
				{
					try
					{
						var ticket = (JObject)_api.JiraAPI.SendGet( "rest/api/latest/issue/" + key );
						var fields = ticket["fields"];
						
						return new Issue()
						{
							Key = ticket["key"].Value<String>(),
							ProjectName = fields["project"]["name"].Value<String>() ?? "N/A",
							Resolution = fields["resolutiondate"].Value<DateTime?>().HasValue ? fields["resolutiondate"].Value<DateTime>() : DateTime.MinValue,
							Status = fields["status"]["name"].Value<String>() ?? "N/A",
							Priority = fields["priority"]["name"].Value<String>() ?? "N/A",
							Creator = fields["creator"]["displayName"].Value<String>(),
							Created = fields["created"].Value<DateTime>()
						};
					}
					catch
					{
						return new Issue();
					}
				}
			);
		}
		
		//TestRail API implementation

		#region User OK
		public User FindUser( Int32 ID )
		{
			var user = (JObject)_api.TestRailAPI.SendGet( "get_user/" + ID );
			return new User()
			{
				ID = ID,
				Name = user["name"].Value<String>(),
				Mail = user["email"].Value<String>()
			};
		}
		
		public async Task<User> FindUserAsync( Int32 ID )
		{
			return await Task.Run<User>
			(
				() =>
				{
					var user = (JObject)_api.TestRailAPI.SendGet( "get_user/" + ID );
					return new User()
					{
						ID = ID,
						Name = user["name"].Value<String>(),
						Mail = user["email"].Value<String>()
					};		
				}
			);
		}
		
		public IEnumerable<User> GetUsers()
		{
			var users = (JArray)_api.TestRailAPI.SendGet( "get_users" );
			return users.Select
				(
					user => new User()
					{
						ID = user["id"].Value<Int32>(),
						Name = user["name"].Value<String>(),
						Mail = user["email"].Value<String>()
					}
				);
		}
		
		public async Task<IEnumerable<User>> GetUsersAsync()
		{
			return await Task.Run<IEnumerable<User>>
			(
				() =>
				{
					var users = (JArray)_api.TestRailAPI.SendGet( "get_users" );
					return users.Select
						(
							user => new User()
							{
								ID = user["id"].Value<Int32>(),
								Name = user["name"].Value<String>(),
								Mail = user["email"].Value<String>()
							}
						);
				}
			);
		}
		#endregion
		
		#region Milestone OK
		public Milestone FindMilestone( Int32 ID )
		{
			var milestone = (JObject)_api.TestRailAPI.SendGet( "get_milestone/" + ID );
			return new Milestone()
			{
				SprintName = milestone["name"].Value<String>()
			};
		}
		
		public async Task<Milestone> FindMilestoneAsync( Int32 ID )
		{
			return await Task.Run<Milestone>
			(
				() =>
				{
					var milestone = (JObject)_api.TestRailAPI.SendGet( "get_milestone/" + ID );
					return new Milestone()
					{
						SprintName = milestone["name"].Value<String>()
					};
				}
			);
					
		}
		#endregion
		
		#region Type OK
		public IEnumerable<Models.TestRailModels.Type> GetCaseTypes()
		{
			var types = (JArray)_api.TestRailAPI.SendGet( "get_case_types" );
			return types.Select
				( type => new Models.TestRailModels.Type()
				 {
				 	ID = type["id"].Value<Int32>(),
				 	Name = type["name"].Value<String>()
				 });
		}
		
		public async Task<IEnumerable<Models.TestRailModels.Type>> GetCaseTypesAsync()
		{
			return await Task.Run<IEnumerable<Models.TestRailModels.Type>>
			(
				() =>
				{
					var types = (JArray)_api.TestRailAPI.SendGet( "get_case_types" );
					return types.Select
						( x => new Models.TestRailModels.Type() 
						 {
						 	ID = x["id"].Value<Int32>(),
		                	Name = x["name"].Value<String>()
						 });
				}
			);
		}
		#endregion
		
		#region Case OK
		public Case FindCase( Int32 ID )
		{
			var _case = (JObject)_api.TestRailAPI.SendGet( "get_case/" + ID );
			return new Case()
			{
				ID = _case["id"].Value<Int32>(),
				Title = _case["title"].Value<String>() ?? "No Title"
			};
		}
		
		public async Task<Case> FindCaseAsync( Int32 ID )
		{
			return await Task.Run<Case>
			(
				() =>
				{
					var _case = (JObject)_api.TestRailAPI.SendGet( "get_case/" + ID );
					return new Case()
					{
						ID = _case["id"].Value<Int32>(),
						Title = _case["title"].Value<String>() ?? "No Title"
					};
				}
			);
		}
		#endregion
		
		#region Test OK
		public Test FindTest( Int32 ID )
		{
			var test = (JObject)_api.TestRailAPI.SendGet( "get_test/" + ID );			
			return new Test()
			{
				ID = test["id"].Value<Int32>(),
				Case = FindCase( test["case_id"].Value<Int32>() ),
				Priority = test["priority_id"].Value<Int32?>().HasValue ? GlobalPriorities.First( x => x.ID == test["priority_id"].Value<Int32>() ) : new Priority(),
				Reference = test["refs"].Value<String>() ?? "N/A",
				Title = test["title"].Value<String>() ?? "N/A",
				Type = test["type_id"].Value<Int32?>().HasValue ? GlobalCaseTypes.First( x => x.ID == test["type_id"].Value<Int32>() ) : new Models.TestRailModels.Type(),
				Area =  ( test["custom_area"] != null && test["custom_area"].Value<Int32?>().HasValue ) ? GlobalAreas.First( x => x.ID == test["custom_area"].Value<Int32>() ): new Area(),
				Feature = ( test["custom_feature"] != null && test["custom_feature"].Value<Int32?>().HasValue ) ? GlobalFeatures.First( x => x.ID == test["custom_feature"].Value<Int32>() ) : new Feature(),
			};
		}
		
		public async Task<Test> FindTestAsync( Int32 ID )
		{
			return await Task.Run<Test>
			(
				async () =>
				{
					var test = (JObject)_api.TestRailAPI.SendGet( "get_test/" + ID );
					return new Test()
					{
						ID = test["id"].Value<Int32>(),
						Case = await FindCaseAsync( test["case_id"].Value<Int32>() ),
						Status = GlobalStatuses.First( x => x.ID == test["status_id"].Value<Int32>() ),
						Priority = test["priority_id"].Value<Int32?>().HasValue ? GlobalPriorities.First( x => x.ID == test["priority_id"].Value<Int32>() ) : new Priority(),
						Reference = test["refs"].Value<String>() ?? "N/A",
						Title = test["title"].Value<String>() ?? "N/A",
						Type = test["type_id"].Value<Int32?>().HasValue ? GlobalCaseTypes.First( x => x.ID == test["type_id"].Value<Int32>() ) : new Models.TestRailModels.Type(),
						Area =  ( test["custom_area"] != null && test["custom_area"].Value<Int32?>().HasValue ) ? GlobalAreas.First( x => x.ID == test["custom_area"].Value<Int32>() ): new Area(),
						Feature = ( test["custom_feature"] != null && test["custom_feature"].Value<Int32?>().HasValue ) ? GlobalFeatures.First( x => x.ID == test["custom_feature"].Value<Int32>() ) : new Feature(),
					};
				}
			);
		}
		
		public IEnumerable<Test> GetTests( Int32 RunID )
		{
			var tests = (JArray)_api.TestRailAPI.SendGet( "get_tests/" + RunID );
			return tests.Select
				(
					test => new Test()
					{
						ID = test["id"].Value<Int32>(),
						Case = FindCase( test["case_id"].Value<Int32>() ),
						Status = GlobalStatuses.First( x => x.ID == test["status_id"].Value<Int32>() ),
						Priority = test["priority_id"].Value<Int32?>().HasValue ? GlobalPriorities.First( x => x.ID == test["priority_id"].Value<Int32>() ) : new Priority(),
						Reference = test["refs"].Value<String>() ?? "N/A",
						Title = test["title"].Value<String>() ?? "N/A",
						Type = test["type_id"].Value<Int32?>().HasValue ? GlobalCaseTypes.First( x => x.ID == test["type_id"].Value<Int32>() ) : new Models.TestRailModels.Type(),
						Area =  ( test["custom_area"] != null && test["custom_area"].Value<Int32?>().HasValue ) ? GlobalAreas.First( x => x.ID == test["custom_area"].Value<Int32>() ): new Area(),
						Feature = ( test["custom_feature"] != null && test["custom_feature"].Value<Int32?>().HasValue ) ? GlobalFeatures.First( x => x.ID == test["custom_feature"].Value<Int32>() ) : new Feature(),
         			});
			
		}
		
		public async Task<IEnumerable<Test>> GetTestsAsync( Int32 RunID )
		{
			return await Task.Run<IEnumerable<Test>>
			(
				async () =>
				{
					var tests = (JArray)_api.TestRailAPI.SendGet( "get_tests/" + RunID );
					Test[] items = new Test[tests.Count];
					var i = 0;
					foreach( var test in tests )
						items[i++] = new Test()
						{
							ID = test["id"].Value<Int32>(),
							Case = await FindCaseAsync( test["case_id"].Value<Int32>() ),
							Status = GlobalStatuses.First( x => x.ID == test["status_id"].Value<Int32>() ),
							Priority = test["priority_id"].Value<Int32?>().HasValue ? GlobalPriorities.First( x => x.ID == test["priority_id"].Value<Int32>() ) : new Priority(),
							Reference = test["refs"].Value<String>() ?? "N/A",
							Title = test["title"].Value<String>() ?? "N/A",
							Type = test["type_id"].Value<Int32?>().HasValue ? GlobalCaseTypes.First( x => x.ID == test["type_id"].Value<Int32>() ) : new Models.TestRailModels.Type(),
							Area =  ( test["custom_area"] != null && test["custom_area"].Value<Int32?>().HasValue ) ? GlobalAreas.First( x => x.ID == test["custom_area"].Value<Int32>() ): new Area(),
							Feature = ( test["custom_feature"] != null && test["custom_feature"].Value<Int32?>().HasValue ) ? GlobalFeatures.First( x => x.ID == test["custom_feature"].Value<Int32>() ) : new Feature(),
						};
					return items;
				});
			
		}
		#endregion
		
		#region Result PENDING REVIEW
		public IEnumerable<Result> GetResultsForRun( Int32 ID )
		{
			var results = (JArray)_api.TestRailAPI.SendGet( "get_results_for_run/" + ID );
			List<Result> items = new List<Result>();
			var untested = GetTests( ID ).Where( x => x.Status.ID == UNTESTED );
			foreach( var test in untested )
			{
				items.Add(
					new Result()
					{
						ID = 0,
						Status = test.Status,
						Test = test,
						TestedBy = new User(),
						TestedOn = DateTime.MinValue,
						Defects = "No defects"
					});
			}
			foreach( var result in results )
			{
				
				if( result["status_id"].Value<Int32?>().HasValue )
				{
					items.Add(
						new Result()
						{
							ID = result["id"].Value<Int32>(),
							Status = GlobalStatuses.First( x => x.ID == result["status_id"].Value<Int32>() ),
							Test = result["test_id"].Value<Int32?>().HasValue ? FindTest( result["test_id"].Value<Int32>() ) : new Test(),
							TestedBy = GlobalUsers.First( x => x.ID == result["created_by"].Value<Int32>() ),
							TestedOn = Helpers.UNIX.FromUnixTime( result["created_on"].Value<Int64>() ),
							Defects = result["defects"].Value<String>() ?? "No defects"
						});
				}
			}
			
			return items;
		}
		
		public async Task<IEnumerable<Result>> GetResultsForRunAsync( Int32 ID )
		{
			return await Task.Run<IEnumerable<Result>>
			(
				async () =>
				{
					var results = (JArray)_api.TestRailAPI.SendGet( "get_results_for_run/" + ID );
					List<Result> items = new List<Result>();
					var untested = (await GetTestsAsync( ID )).Where( x => x.Status.ID == UNTESTED );
					foreach( var test in untested )
					{
						items.Add(
							new Result()
							{
								ID = 0,
								Status = test.Status,
								Test = test,
								TestedBy = new User(),
								TestedOn = DateTime.MinValue,
								Defects = "No defects"
							});
					}
			
					foreach( var result in results )
					{
							
						if( result["status_id"].Value<Int32?>().HasValue )
						items.Add(
							new Result()
							{
								ID = result["id"].Value<Int32>(),
								Status = GlobalStatuses.First( x => x.ID == result["status_id"].Value<Int32>() ),
								Test = result["test_id"].Value<Int32?>().HasValue ? await FindTestAsync( result["test_id"].Value<Int32>() ) : new Test(),
								TestedBy = GlobalUsers.First( x => x.ID == result["created_by"].Value<Int32>() ),
								TestedOn = UNIX.FromUnixTime( result["created_on"].Value<Int64>() ),
								Defects = result["defects"].Value<String>() ?? "No defects"
							});
					}
					
					return items;
				}
			);
		}
		#endregion
		
		#region Project OK
		public IEnumerable<Project> GetProjects()
		{
			var projects = (JArray)_api.TestRailAPI.SendGet( "get_projects" );
			return projects.Select
				( x => new Project() 
				 {
                	Name = x["name"].Value<String>(),
                	ID = x["id"].Value<Int32>()
				 });
		}
		
		public async Task<IEnumerable<Project>> GetProjectsAsync()
		{
			return await Task.Run<IEnumerable<Project>>
			(
				() =>
				{
					var projects = (JArray)_api.TestRailAPI.SendGet( "get_projects" );
					return projects.Select
						( x => new Project() 
						 {
		                	Name = x["name"].Value<String>(),
		                	ID = x["id"].Value<Int32>()
						 });
				}
			);
		}
		
		public Project FindProject( Int32 ID )
		{
			var project = (JObject)_api.TestRailAPI.SendGet( "get_project/" + ID );
			return new Project()
			{
				Name = project["name"].Value<String>(),
				ID = project["id"].Value<Int32>()
			};
		}
		
		public async Task<Project> FindProjectAsync( Int32 ID )
		{
			return await Task.Run<Project>
			(
				() =>
				{
					var project = (JObject)_api.TestRailAPI.SendGet( "get_project/" + ID );
					return new Project()
					{
						Name = project["name"].Value<String>(),
						ID = project["id"].Value<Int32>()
					};
				}
			);
		}
		#endregion
		
		#region Suite OK
		public Suite FindSuite( Int32 ID )
		{
			var suite = (JObject)_api.TestRailAPI.SendGet( "get_suite/" + ID );
			return new Suite()
			{
				Name = suite["name"].Value<String>(),
				ID = suite["id"].Value<Int32>()
			};
		}
		
		public async Task<Suite> FindSuiteAsync( Int32 ID )
		{
			return await Task.Run<Suite>
			(
				() =>
				{
					var suite = (JObject)_api.TestRailAPI.SendGet( "get_suite/" + ID );
					return new Suite()
					{
						Name = suite["name"].Value<String>(),
						ID = suite["id"].Value<Int32>()
					};
				}
			);
		}
		#endregion
		
		#region Run OK
		public Run FindRun( Int32 ID )
		{
			var run = (JObject)_api.TestRailAPI.SendGet( "get_run/" + ID );
			return new Run()
			{
				Name = run["name"].Value<String>(),
				ID = run["id"].Value<Int32>(),
				IsCompleted = run["is_completed"].Value<Boolean>(),
				Suite = FindSuite( run["suite_id"].Value<Int32>() ),
				Milestone = run["milestone_id"].Value<Int32?>().HasValue ? FindMilestone( run["milestone_id"].Value<Int32>() ) : new Milestone()
			};
		}
		
		public async Task<Run> FindRunAsync( Int32 ID )
		{
			return await Task.Run<Run>
			(
				async () =>
				{
					var run = (JObject)_api.TestRailAPI.SendGet( "get_run/" + ID );
					return new Run()
					{
						Name = run["name"].Value<String>(),
						ID = run["id"].Value<Int32>(),
						IsCompleted = run["is_completed"].Value<Boolean>(),
						Suite = await FindSuiteAsync( run["suite_id"].Value<Int32>() ),
						Milestone = run["milestone_id"].Value<Int32?>().HasValue ? await FindMilestoneAsync( run["milestone_id"].Value<Int32>() ) : new Milestone()
					};
				}
			);
		}
		
		public IEnumerable<Run> GetRuns( Int32 ProjectID )
		{
			var runs = (JArray)_api.TestRailAPI.SendGet( "get_runs/" + ProjectID );
			return runs.Select
				( run => new Run()
				  {
					Name = run["name"].Value<String>(),
					ID = run["id"].Value<Int32>(),
					IsCompleted = run["is_completed"].Value<Boolean>(),
					Suite = FindSuite( run["suite_id"].Value<Int32>() ),
					Milestone = run["milestone_id"].Value<Int32?>().HasValue ? FindMilestone( run["milestone_id"].Value<Int32>() ) : new Milestone()
				  });
		}
		
		public async Task<IEnumerable<Run>> GetRunsAsync( Int32 ProjectID )
		{
			return await Task.Run<IEnumerable<Run>>
			(
				async () =>
				{
					var runs = (JArray)_api.TestRailAPI.SendGet( "get_runs/" + ProjectID );
					Run[] all_runs = new Run[runs.Count];
					var len = all_runs.Length;
					for( int i = 0; i < len; i++ )
					{
						all_runs[i] = new Run()
						{
							Name = runs[i]["name"].Value<String>(),
							ID = runs[i]["id"].Value<Int32>(),
							IsCompleted = runs[i]["is_completed"].Value<Boolean>(),
							Suite = await FindSuiteAsync( runs[i]["suite_id"].Value<Int32>() ),
							Milestone = !runs[i]["milestone_id"].Value<Int32?>().HasValue ? new Milestone() : await FindMilestoneAsync( runs[i]["milestone_id"].Value<Int32>() )
						};
					}
					
					return all_runs;
				}
			);
		}
		#endregion
		
		#region Status OK
		public IEnumerable<Status> GetStatuses()
		{
			var statuses = (JArray)_api.TestRailAPI.SendGet( "get_statuses" );
			return statuses.Select
				( status => new Status()
				 {
				 	Name = status["label"].Value<String>(),
				 	ID = status["id"].Value<Int32>()
				 });
		}
		
		public async Task<IEnumerable<Status>> GetStatusesAsync()
		{
			return await Task.Run<IEnumerable<Status>>
			(
				() =>
				{
					var statuses = (JArray)_api.TestRailAPI.SendGet( "get_statuses" );
					return statuses.Select
						( status => new Status()
						 {
						 	Name = status["label"].Value<String>(),
						 	ID = status["id"].Value<Int32>()
						 });
				}
			);
		}
		#endregion
		
		#region Priority OK
		public IEnumerable<Priority> GetPriorities()
		{
			var priorities = (JArray)_api.TestRailAPI.SendGet( "get_priorities" );
			return priorities.Select
				( priority => new Priority()
				 {
				 	Name = priority["name"].Value<String>(),
				 	ID = priority["id"].Value<Int32>(),
				 	Level = priority["priority"].Value<Int32>()
				 });
		}
		
		public async Task<IEnumerable<Priority>> GetPrioritiesAsync()
		{
			return await Task.Run<IEnumerable<Priority>>
			(
				() =>
				{
					var priorities = (JArray)_api.TestRailAPI.SendGet( "get_priorities" );
					return priorities.Select
						( priority => new Priority()
						 {
						 	Name = priority["name"].Value<String>(),
						 	ID = priority["id"].Value<Int32>(),
						 	Level = priority["priority"].Value<Int32>()
						 });
				}
			);
		}
		#endregion
		
		#region Area OK
		public IEnumerable<Area> GetAreas()
		{
			var areas = (JArray)_api.TestRailAPI.SendGet( "get_case_fields" );
			var custom_areas = areas.First( x => x["name"].Value<String>() == "area" );
			
			var configs = (JArray)custom_areas["configs"];
			
			var options = configs.First()["options"];
			var items = options["items"].Value<String>();
			
			var values = items.Split( '\n' );
			return values.Select
				( x => new Area()
				 {
				 	ID = Int32.Parse( x.Split( ',' )[0] ),
				 	Name = x.Split( ',' )[1].Trim()
				 });
		}
		
		public async Task<IEnumerable<Area>> GetAreasAsync()
		{
			return await Task.Run<IEnumerable<Area>>
			(
				() =>
				{
					var areas = (JArray)_api.TestRailAPI.SendGet( "get_case_fields" );
					var custom_areas = areas.First( x => x["name"].Value<String>() == "area" );
					
					var configs = (JArray)custom_areas["configs"];
					
					var options = configs.First()["options"];
					var items = options["items"].Value<String>();
					
					var values = items.Split( '\n' );
					return values.Select
						( x => new Area()
						 {
						 	ID = Int32.Parse( x.Split( ',' )[0] ),
						 	Name = x.Split( ',' )[1].Trim()
						 }); 
				}
			);
		}
		#endregion
		
		#region Feature OK
		public IEnumerable<Feature> GetFeatures()
		{
			var areas = (JArray)_api.TestRailAPI.SendGet( "get_case_fields" );
			var custom_areas = areas.First( x => x["name"].Value<String>() == "feature" );
			
			var configs = (JArray)custom_areas["configs"];
			
			var options = configs.First()["options"];
			var items = options["items"].Value<String>();
			
			var values = items.Split( '\n' );
			return values.Select
				( x => new Feature()
				 {
				 	ID = Int32.Parse( x.Split( ',' )[0] ),
				 	Name = x.Split( ',' )[1].Trim()
				 });
		}
		
		public async Task<IEnumerable<Feature>> GetFeaturesAsync()
		{
			return await Task.Run<IEnumerable<Feature>>
			(
				() =>
				{
					var areas = (JArray)_api.TestRailAPI.SendGet( "get_case_fields" );
					var custom_areas = areas.First( x => x["name"].Value<String>() == "feature" );
					
					var configs = (JArray)custom_areas["configs"];
					
					var options = configs.First()["options"];
					var items = options["items"].Value<String>();
					
					var values = items.Split( '\n' );
					return values.Select
						( x => new Feature()
						 {
						 	ID = Int32.Parse( x.Split( ',' )[0] ),
						 	Name = x.Split( ',' )[1].Trim()
						 }); 
				}
			);
		}
		#endregion
		
	}
}
