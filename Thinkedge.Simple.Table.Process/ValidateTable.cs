using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Process
{
	public class ValidateTable : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable validationRules, bool rowPerMatch, bool overwrite)
		{
			return new ValidateTable().ExecuteEx(sourceTable, validationRules, rowPerMatch, overwrite);
		}

		internal StandardResult<SimpleTable> ExecuteEx(SimpleTable sourceTable, SimpleTable validationRules, bool rowPerMatch, bool overwrite)
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

			var fieldSource = new TableDataSource();

			foreach (var sourceRow in sourceTable)
			{
				fieldSource.Row = sourceRow;

				SimpleTableRow row = null;

				foreach (var rule in validationRules)
				{
					var match = rule["Match"];

					if (string.IsNullOrWhiteSpace(match))
						continue;

					var evaluator = cache.Compile(match);

					if (evaluator == null)
						return ReturnError<SimpleTable>("ValidateTable() error: evaluator returns null");

					if (!evaluator.IsValid)
						return ReturnError<SimpleTable>("ValidateTable() error: occurred during evaluating: " + evaluator.Parser.Tokenizer.Expression);

					var result = evaluator.Evaluate(new Context() { FieldSource = fieldSource });

					if (result.IsError)
						return ReturnError<SimpleTable>("ValidateTable() error: occurred during evaluating: " + evaluator.Parser.Tokenizer.Expression, result.String);

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

						if (evaluator2 == null)
							return ReturnError<SimpleTable>("ValidateTable() error: evaluator returns null");

						if (!evaluator2.IsValid)
							return ReturnError<SimpleTable>("ValidateTable() error: occurred during evaluating: " + evaluator2.Parser.Tokenizer.Expression);

						var result2 = evaluator2.Evaluate(new Context() { FieldSource = fieldSource });

						if (result2.IsError)
							return ReturnError<SimpleTable>("ValidateTable() error: occurred during evaluating: " + evaluator2.Parser.Tokenizer.Expression, result2.String);

						if (overwrite || string.IsNullOrWhiteSpace(row[column]))
							row[column] = result2.String;
					}
				}
			}

			return ReturnResult<SimpleTable>(resultsTable);
		}
	}
}