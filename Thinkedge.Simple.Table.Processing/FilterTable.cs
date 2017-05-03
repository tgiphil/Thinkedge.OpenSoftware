using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Processing
{
	internal class FilterTable : BaseStandardResult
	{
		internal SimpleTable Execute(SimpleTable table, string includeExpression)
		{
			ClearError();

			if (string.IsNullOrWhiteSpace(includeExpression))
			{
				return table.Copy();
			}

			var newTable = new SimpleTable();
			var cache = new EvaluatorCache();

			foreach (var column in table.ColumnNames)
			{
				newTable.AddColumnName(column);
			}

			var tableSource = new TableDataSource();

			var evaluator = cache.Compile(includeExpression);

			foreach (var sourceRow in table)
			{
				tableSource.Row = sourceRow;

				if (string.IsNullOrWhiteSpace(includeExpression))
					continue;

				var result = evaluator.Evaluate(tableSource);

				if (!evaluator.IsValid)
				{
					SetError("compile error");
					return null;
				}

				if (!result.IsBoolean)
				{
					SetError("filter error: invalid result");
					return null;
				}

				if (result.Boolean)
				{
					var newRow = newTable.CreateRow();

					for (int i = 0; i < sourceRow.ColumnCount; i++)
					{
						var value = sourceRow[i];

						if (value == null)
							continue;

						newRow[i] = value;
					}
				}
			}

			return newTable;
		}
	}
}