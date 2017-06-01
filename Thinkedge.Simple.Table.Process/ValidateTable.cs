using Thinkedge.Common;
using Thinkedge.Simple.Evaluator;

namespace Thinkedge.Simple.Table.Process
{
	public static class ValidateTable
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable validationRules, bool rowPerMatch, bool overwrite)
		{
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

			var fieldSource = new FieldDataSource();

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
						return StandardResult<SimpleTable>.ReturnError("ValidateTable() error: evaluator returns null");

					if (!expression.IsValid)
						return StandardResult<SimpleTable>.ReturnError("ValidateTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression);

					var result = expression.Evaluate(new Context() { FieldSource = fieldSource });

					if (result.IsError)
						return StandardResult<SimpleTable>.ReturnError("ValidateTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression, result.String);

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
							return StandardResult<SimpleTable>.ReturnError("ValidateTable() error: evaluator returns null");

						if (!expression2.IsValid)
							return StandardResult<SimpleTable>.ReturnError("ValidateTable() error: occurred during evaluating: " + expression2.Parser.Tokenizer.Expression);

						var result2 = expression2.Evaluate(new Context() { FieldSource = fieldSource });

						if (result2.IsError)
							return StandardResult<SimpleTable>.ReturnError("ValidateTable() error: occurred during evaluating: " + expression2.Parser.Tokenizer.Expression, result2.String);

						if (overwrite || string.IsNullOrWhiteSpace(row[column]))
							row[column] = result2.String;
					}
				}
			}

			return StandardResult<SimpleTable>.ReturnResult(resultsTable);
		}
	}
}