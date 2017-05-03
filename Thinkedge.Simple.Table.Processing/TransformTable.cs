using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Processing
{
	internal class TransformTable : BaseStandardResult
	{
		internal SimpleTable Execute(SimpleTable sourceTable, SimpleTable mapTable)
		{
			ClearError();

			if (mapTable == null)
			{
				SetError("transformation error: map table null");
				return null;
			}

			var newTable = new SimpleTable();
			var cache = new EvaluatorCache();

			// create columns
			foreach (var map in mapTable)
			{
				var evaluator = cache.Compile(map["source"]);

				if (evaluator == null)
					return null;

				newTable.AddColumnName(map["destination"]);
			}

			var tableSource = new TableDataSource();

			foreach (var sourceRow in sourceTable)
			{
				tableSource.Row = sourceRow;

				var destinationRow = newTable.CreateRow();

				foreach (var map in mapTable)
				{
					var evaluator = cache.Compile(map["source"]);

					var result = evaluator.Evaluate(tableSource);

					if (result.IsError)
					{
						SetError(result.String);
						return null;
					}

					destinationRow.SetField(map["destination"], result.String);
				}
			}

			return newTable;
		}
	}
}