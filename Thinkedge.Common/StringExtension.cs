using System;

namespace Thinkedge.Common
{
	public static class StringExtension
	{
		public static int ToInteger(this string value)
		{
			return Convert.ToInt32(value);
		}

		public static decimal ToDecimal(this string value)
		{
			return Convert.ToDecimal(value);
		}
	}
}