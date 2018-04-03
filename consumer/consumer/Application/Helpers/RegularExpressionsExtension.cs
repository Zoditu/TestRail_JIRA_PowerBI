using System;

namespace ConsumerTestRail.Application.Helpers
{
	public static class RegularExpressionsExtension
	{
		public static String ToRegex( this String text )
		{
			var chars = text.ToCharArray();
			var _regex = "";
			
			foreach( var c in chars )
				_regex += "[" + Char.ToUpper( c ) + Char.ToLower( c ) + "]";
			
			return _regex;
		}
	}
}
