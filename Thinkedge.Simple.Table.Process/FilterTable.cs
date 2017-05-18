using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Process
{
	public class FilterTable : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable table, string includeExpression)
		{
			return new FilterTable().ExecuteEx(table, includeExpression);
		}

		internal StandardResult<SimpleTable> ExecuteEx(SimpleTable table, string includeExpression)
		{
			if (string.IsNullOrWhiteSpace(includeExpression))
			{
				return ReturnResult<SimpleTable>(table.Copy());
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
					return ReturnError<SimpleTable>("FilterTable() error: occurred during evaluating: " + evaluator.Parser.Tokenizer.Expression, "invalid result: " + result.String);

				if (!result.IsBoolean)
					return ReturnError<SimpleTable>("FilterTable() error: occurred during evaluating: " + evaluator.Parser.Tokenizer.Expression, "result was not boolean: " + result.String);

				if (!result.Boolean)
					continue;

				var newRow = newTable.CreateRow();

				for (int i = 0; i < sourceRow.ColumnCount; i++)
				{
					var value = sourceRow[i];

					if (value == null)
						continue;

					newRow[i] = value;
				}
			}

			return ReturnResult<SimpleTable>(newTable);
		}
	}
}