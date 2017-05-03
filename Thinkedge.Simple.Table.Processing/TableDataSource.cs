using System.Collections.Generic;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Processing
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

		public void SetVariable(string name, string value)
		{
			Variables.Remove(name);
			Variables.Add(name, value);
		}

		public void ClearVariables()
		{
			Variables.Clear();
		}

		Value ITableSource.GetField(string name)
		{
			var data = Row.GetField(name);

			return new Value(data);
		}
	}
}