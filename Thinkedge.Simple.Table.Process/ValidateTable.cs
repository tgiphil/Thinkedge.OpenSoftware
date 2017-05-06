using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Process
{
	public class ValidateTable : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable validationRules, bool rowPerMatch)
		{
			return new ValidateTable().ExecuteEx(sourceTable, validationRules, rowPerMatch);
		}

		internal StandardResult<SimpleTable> ExecuteEx(SimpleTable sourceTable, SimpleTable validationRules, bool rowPerMatch)
		{
			ClearError();

			var resultsTable = new SimpleTable();
			var cache = new EvaluatorCache();

			foreach (var column in sourceTable.ColumnNames)
			{
				resultsTable.AddColumnName(column);
			}

			foreach (var column in validationRules.ColumnNames)
			{
				if (column == "Match")
					continue;

				resultsTable.AddColumnName(column);
			}

			var tableSource = new TableDataSource();

			foreach (var sourceRow in sourceTable)
			{
				tableSource.Row = sourceRow;

				SimpleTableRow row = null;

				foreach (var rule in validationRules)
				{
					var match = rule["Match"];

					if (string.IsNullOrWhiteSpace(match))
						continue;

					var evaluator = cache.Compile(match);

					if (evaluator == null || !evaluator.IsValid)
						return ReturnError<SimpleTable>("validation error: evaluator returns null");

					var result = evaluator.Evaluate(tableSource);

					if (!result.Boolean)
						continue;

					if (row == null || rowPerMatch)
					{
						row = resultsTable.CreateRow();
					}

					foreach (var column in sourceTable.ColumnNames)
					{
						row[column] = sourceRow[column];
					}

					foreach (var column in validationRules.ColumnNames)
					{
						if (column == "Match")
							continue;

						var text = rule[column];

						if (string.IsNullOrWhiteSpace(text))
							continue;

						var evaluator2 = cache.Compile(text);

						if (evaluator2 == null || !evaluator2.IsValid)
							return ReturnError<SimpleTable>("validation error: evaluator returns null");

						var result2 = evaluator2.Evaluate(tableSource);

						if (result2.IsError)
							return ReturnError<SimpleTable>(result2.String);

						row[column] = result2.String;
					}
				}
			}

			return ReturnResult<SimpleTable>(resultsTable);
		}
	}
}