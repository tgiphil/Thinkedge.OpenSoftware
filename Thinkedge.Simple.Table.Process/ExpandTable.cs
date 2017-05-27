using Thinkedge.Common;
using Thinkedge.Simple.Evaluator;

namespace Thinkedge.Simple.Table.Process
{
	public class ExpandTable : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable expandTable)
		{
			return new ExpandTable().ExecuteEx(sourceTable, expandTable);
		}

		internal StandardResult<SimpleTable> ExecuteEx(SimpleTable sourceTable, SimpleTable expandTable)
		{
			if (expandTable == null)
				return ReturnError<SimpleTable>("ExpandTable() error: add table null");

			var newTable = sourceTable.Copy();

			// create columns
			foreach (var map in expandTable)
			{
				var source = map["source"];
				var destination = map["destination"];
				newTable.AddColumnName(destination);

				var expression = ExpressionCache.Compile(source);

				if (expression == null)
					return ReturnError<SimpleTable>("ExpandTable() error: evaluator returns null");
			}

			var fieldSource = new TableDataSource();

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
						return ReturnError<SimpleTable>("ExpandTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression, result.String);

					newRow[destination] = ToString(result);
				}
			}

			return ReturnResult<SimpleTable>(newTable);
		}

		public static string ToString(Value value)
		{
			if (value.IsString) return value.String;
			else if (value.IsInteger) return value.Integer.ToString();
			else if (value.IsDecimal) return value.Decimal.ToString();
			else if (value.IsBoolean) return value.Boolean ? "true" : "false";
			else if (value.IsDate) return value.Date.ToShortDateString();

			//todo

			return null;
		}
	}
}