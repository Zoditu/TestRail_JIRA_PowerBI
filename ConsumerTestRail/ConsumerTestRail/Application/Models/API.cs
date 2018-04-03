using System;
using System.Collections.Generic;
using System.IO;
using Gurock.TestRail;

namespace ConsumerTestRail.Application
{
	public class API
	{
		private APIClient _apiTR, _apiJIRA, _common;
		private const String _config = "gurock.cfg";
		private Dictionary<String, String> Configuration;
		
		public API()
		{				
			ReloadConfig();
		}
		
		public Boolean HasDefaultConfiguration
		{
			get;
			set;
		}
		
		public Boolean IsMultiTask
		{
			get
			{
				Boolean assertion;
				return Boolean.TryParse(Configuration.ContainsKey("MULTITASK") ? Configuration["MULTITASK"] : "null", out assertion) && assertion;
			}
		}
		
		public String Username
		{
			get
			{
				return Configuration.ContainsKey( "USER" ) ? Configuration["USER"] : "null";
			}
		}
		
		public String TestRailServer
		{
			get
			{
				return Configuration.ContainsKey( "TESTRAIL" ) ? Configuration["TESTRAIL"] : "null";
			}
		}
		
		public String JiraServer 
		{
			get
			{
				return Configuration.ContainsKey( "JIRA" ) ? Configuration["JIRA"] : "null";
			}
		}
		
		public APIClient TestRailAPI
		{
			get
			{
				return _apiTR;
			}
		}
		
		public APIClient JiraAPI
		{
			get
			{
				return _apiJIRA;
			}
		}
		
		public APIClient CommonAPI
		{
			get
			{
				return _common;
			}
		}
		
		public String ExportLocation
		{
			get
			{
				if ( Configuration.ContainsKey( "EXPORT" ) )
				{
					if( Configuration["EXPORT"].StartsWith( "this" ) )
						return Environment.CurrentDirectory + Configuration["EXPORT"].Remove( 0, 4 );
					
					return Configuration["EXPORT"];
				}
				
				return Environment.CurrentDirectory + "\\default_export.xls";
			}
		}
		
		public String LogLocation
		{
			get
			{
				if ( Configuration.ContainsKey( "LOG" ) )
				{
					if( Configuration["LOG"].StartsWith( "this" ) )
						return Environment.CurrentDirectory + Configuration["LOG"].Remove( 0, 4 );
					
					return Configuration["LOG"];
				}
				
				return Environment.CurrentDirectory + "\\default_export.xls";
			}
		}
		
		public void SetProperty( String property, String value )
		{
			var key = property.ToUpper();
			if( Configuration.ContainsKey( key ) )
				Configuration[key] = value;
			else
				Configuration.Add( key, value );
			
			var config = "";
			foreach( KeyValuePair<String, String> kv in Configuration )
				config += kv.Key + " = " + kv.Value + Environment.NewLine;
			
			System.IO.File.WriteAllText( _config, config );
			FetchConfig( _config );
		}
		
		private Dictionary<String, String> FetchConfig( String path )
		{
			String[] configs = System.IO.File.ReadAllLines( path );
			Dictionary<String, String> result = new Dictionary<String, String>();
			foreach( String s in configs )
				if( !String.IsNullOrWhiteSpace( s ) )
				{
					var values = s.Split( '=' );
					result.Add( values[0].Trim().ToUpper(), values[1].Trim() );
				}
			
			return result;
		}
		
		public String ShowConfig()
		{
			var config = "";
			foreach( KeyValuePair<String, String> kv in Configuration )
				config += "\t\t" + kv.Key + " = " + kv.Value + Environment.NewLine;
			
			return config;
		}
		
		public void ReloadConfig()
		{
			if( !File.Exists( _config ) )
			{
				HasDefaultConfiguration = true;
				//Create file with defaults
				var file = File.Create( "gurock.cfg" );
				using( StreamWriter w = new StreamWriter( file ) )
				{
					w.WriteLine( "testrail = https://servername" );
					w.WriteLine( "jira = https://servername" );
					w.WriteLine( "user = null" );
					w.WriteLine( "export = DRIVE:\\PATH" );
					w.WriteLine( "log = DRIVE:\\PATH" );
					w.WriteLine( "password = null" );
					w.WriteLine( "multitask = true" );
				}
				
			}
			else
				HasDefaultConfiguration = false;
			
			Configuration = FetchConfig( _config );
			
			_apiTR = new APIClient( TestRailServer ){ IsTestRail = true };
			_apiTR.User = Username;
			_apiTR.Password = Configuration.ContainsKey( "PASSWORD" ) ? Configuration["PASSWORD"] : "null";
			
			_apiJIRA = new APIClient( JiraServer ){ IsTestRail = false };
			_apiJIRA.User = Username;
			_apiJIRA.Password = Configuration.ContainsKey( "PASSWORD" ) ? Configuration["PASSWORD"] : "null";
		}
	}
}
