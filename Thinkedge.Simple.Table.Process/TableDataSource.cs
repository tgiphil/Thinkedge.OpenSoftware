using System.Collections.Generic;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Process
{
	public class TableDataSource : ITableSource
	{
		public SimpleTableRow Row { get; set; } = null;

		protected Dictionary<string, string> Variables = new Dictionary<string, string>();

		public TableDataSource()
		{
		}

		public TableDataSource(SimpleTableRow simpleTableRow)
		{
			Row = simpleTableRow;
		}

		Value ITableSource.GetField(string name)
		{
			if (Row.Table.GetColumnIndex(name) < 0)
				return Value.CreateErrorValue("unknown column: " + name);

			var data = Row.GetField(name);

			return new Value(data);
		}
	}
}