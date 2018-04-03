using System;
using ConsumerTestRail;

namespace consumer
{
	class Program
	{
		static Boolean PROCESSING = false;
		public static void Main(string[] args)
		{
			if( args.Length > 0 )
			{
				if( args[0] == "-run" )
				{
					System.Threading.Tasks.Task.WaitAll
						(
							System.Threading.Tasks.Task.Run
							( 
							 async () =>
							 {
							 	PROCESSING = true;
								ConsumerTestRail.Application.Controllers.APIController controller = new ConsumerTestRail.Application.Controllers.APIController();
								controller.RunSilent();
								
								while( PROCESSING )
								{
									await System.Threading.Tasks.Task.Delay( 1500 );
									PROCESSING = controller.IsProcessing;
								}
							 }
							)
						);
				}
			}
		}
	}
}