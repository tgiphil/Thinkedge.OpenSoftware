using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Process
{
	public class TransformTable : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable mapTable)
		{
			return new TransformTable().ExecuteEx(sourceTable, mapTable);
		}

		internal StandardResult<SimpleTable> ExecuteEx(SimpleTable sourceTable, SimpleTable mapTable)
		{
			if (mapTable == null)
			{
				return ReturnError<SimpleTable>("transformation error: map table null");
			}

			var newTable = new SimpleTable();
			var cache = new EvaluatorCache();

			// create columns
			foreach (var map in mapTable)
			{
				var evaluator = cache.Compile(map["source"]);

				if (evaluator == null)
					return ReturnError<SimpleTable>("transformation error: evaluator returns null");

				newTable.AddColumnName(map["destination"]);
			}

			var tableSource = new TableDataSource();

			foreach (var sourceRow in sourceTable)
			{
				tableSource.Row = sourceRow;

				var destinationRow = newTable.CreateRow();

				foreach (var map in mapTable)
				{
					var source = map["source"];
					var destination = map["destination"];

					var evaluator = cache.Compile(source);

					var result = evaluator.Evaluate(tableSource);

					if (result.IsError)
					{
						return ReturnError<SimpleTable>(result.String);
					}

					destinationRow[destination] = ExpandTable.ToString(result);
				}
			}

			return ReturnResult<SimpleTable>(newTable);
		}
	}
}