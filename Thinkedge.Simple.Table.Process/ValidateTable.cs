using Thinkedge.Common;
using Thinkedge.Simple.Evaluator;

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

					var expression = ExpressionCache.Compile(match);

					if (expression == null)
						return ReturnError<SimpleTable>("ValidateTable() error: evaluator returns null");

					if (!expression.IsValid)
						return ReturnError<SimpleTable>("ValidateTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression);

					var result = expression.Evaluate(new Context() { FieldSource = fieldSource });

					if (result.IsError)
						return ReturnError<SimpleTable>("ValidateTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression, result.String);

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

						var expression2 = ExpressionCache.Compile(text);

						if (expression2 == null)
							return ReturnError<SimpleTable>("ValidateTable() error: evaluator returns null");

						if (!expression2.IsValid)
							return ReturnError<SimpleTable>("ValidateTable() error: occurred during evaluating: " + expression2.Parser.Tokenizer.Expression);

						var result2 = expression2.Evaluate(new Context() { FieldSource = fieldSource });

						if (result2.IsError)
							return ReturnError<SimpleTable>("ValidateTable() error: occurred during evaluating: " + expression2.Parser.Tokenizer.Expression, result2.String);

						if (overwrite || string.IsNullOrWhiteSpace(row[column]))
							row[column] = result2.String;
					}
				}
			}

			return ReturnResult<SimpleTable>(resultsTable);
		}
	}
}