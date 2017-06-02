using Thinkedge.Common;
using Thinkedge.Simple.Evaluator;

namespace Thinkedge.Simple.Table.Process
{
	public static class ExpandTable
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable expandTable)
		{
			if (sourceTable == null)
				return StandardResult<SimpleTable>.ReturnError("ExpandTable() error: source table null");
			if (expandTable == null)
				return StandardResult<SimpleTable>.ReturnError("ExpandTable() error: aggregate table null");

			var newTable = sourceTable.Copy();

			// create columns
			foreach (var map in expandTable)
			{
				var source = map["source"];
				var destination = map["destination"];
				newTable.AddColumnName(destination);

				var expression = ExpressionCache.Compile(source);

				if (expression == null)
					return StandardResult<SimpleTable>.ReturnError("AggregateTable() error: evaluator returns null", "Source: " + source);
			}

			var fieldSource = new FieldDataSource();

			foreach (var newRow in newTable)
			{
				fieldSource.Row = newRow;

				foreach (var map in expandTable)
				{
					var source = map["source"];
					var destination = map["destination"];

					var expression = ExpressionCache.Compile(source);

					var result = expression.Evaluate(new Context() { FieldSource = fieldSource });

					if (result.IsError)
						return StandardResult<SimpleTable>.ReturnError("ExpandTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression, result.String);

					newRow[destination] = ToString(result);
				}
			}

			return StandardResult<SimpleTable>.ReturnResult(newTable);
		}

		public static string ToString(Value value)
		{
			if (value.IsNull) return string.Empty;
			else if (value.IsString) return value.String;
			else if (value.IsInteger) return value.Integer.ToString();
			else if (value.IsDecimal) return value.Decimal.ToString();
			else if (value.IsBoolean) return value.Boolean ? "true" : "false";
			else if (value.IsDate) return value.Date.ToShortDateString();
			else if (value.IsFloat) return value.Float.ToString();

			//todo

			return null;
		}
	}
}