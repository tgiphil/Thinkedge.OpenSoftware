using System.Collections.Generic;
using Thinkedge.Simple.Evaluator;

namespace Thinkedge.Simple.Table.Process
{
	public class AggregateFieldSource : IAggregateFieldSource
	{
		public IList<SimpleTableRow> TableRows;

		public AggregateFieldSource()
		{
		}

		public AggregateFieldSource(IList<SimpleTableRow> rows)
		{
			TableRows = rows;
		}

		IEnumerable<IFieldSource> IAggregateFieldSource.Rows
		{
			get
			{
				foreach (var row in TableRows)
				{
					var source = new FieldDataSource(row);
					yield return source;
				}
			}
		}
	}
}