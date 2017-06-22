using Thinkedge.Common.Result;
using Thinkedge.Simple.ExpressionEngine;

namespace Thinkedge.Simple.Table.Method
{
	public static class FilterTable
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable table, string includeExpression)
		{
			if (string.IsNullOrWhiteSpace(includeExpression))
				return StandardResult<SimpleTable>.ReturnResult(table.Copy());

			var newTable = new SimpleTable();
			foreach (var column in table.ColumnNames)
			{
				newTable.AddColumnName(column);
			}

			var fieldSource = new FieldDataSource();

			var expression = ExpressionCache.Compile(includeExpression);

			foreach (var sourceRow in table)
			{
				fieldSource.Row = sourceRow;

				if (string.IsNullOrWhiteSpace(includeExpression))
					continue;

				var result = expression.Evaluate(new Context() { FieldSource = fieldSource });

				if (!expression.IsValid)
					return StandardResult<SimpleTable>.ReturnError("FilterTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression, "invalid result: " + result.String);

				if (!result.IsBoolean)
					return StandardResult<SimpleTable>.ReturnError("FilterTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression, "result was not boolean: " + result.String);

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

			return StandardResult<SimpleTable>.ReturnResult(newTable);
		}
	}
}