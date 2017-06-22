using System.Collections.Generic;
using Thinkedge.Simple.ExpressionEngine;

namespace Thinkedge.Simple.Table.Method
{
	public class FieldDataSource : IFieldSource
	{
		public SimpleTableRow Row { get; set; } = null;

		protected Dictionary<string, string> Variables = new Dictionary<string, string>();

		public FieldDataSource()
		{
		}

		public FieldDataSource(SimpleTableRow simpleTableRow)
		{
			Row = simpleTableRow;
		}

		Value IFieldSource.GetField(string name)
		{
			if (Row.Table.GetColumnIndex(name) < 0)
				return Value.CreateErrorValue("unknown column: " + name);

			var data = Row.GetField(name);

			return new Value(data);
		}
	}
}