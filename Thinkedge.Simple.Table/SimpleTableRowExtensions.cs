using System;

namespace Thinkedge.Simple.Table
{
	public static class SimpleTableRowExtensions
	{
		public static int GetIntegerField(this SimpleTableRow row, string name)
		{
			var value = row.GetField(name);

			return Convert.ToInt32(value);
		}

		private static int? GetNullableIntegerField(this SimpleTableRow row, string name)
		{
			var value = row.GetField(name);

			if (string.IsNullOrWhiteSpace(value))
				return null;

			return Convert.ToInt32(value);
		}
	}
}