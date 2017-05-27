using Thinkedge.Common;
using Thinkedge.Simple.Evaluator;

namespace Thinkedge.Simple.Table.Process
{
	public static class TransformTable
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable mapTable)
		{
			if (mapTable == null)
				return StandardResult<SimpleTable>.ReturnError("TransformTable() error: map table null");

			var newTable = new SimpleTable();

			// create columns
			foreach (var map in mapTable)
			{
				var expression = ExpressionCache.Compile(map["source"]);

				if (expression == null)
					return StandardResult<SimpleTable>.ReturnError("TransformTable() error: evaluator returns null");

				newTable.AddColumnName(map["destination"]);
			}

			var fieldSource = new TableDataSource();

			foreach (var sourceRow in sourceTable)
			{
				fieldSource.Row = sourceRow;

				var destinationRow = newTable.CreateRow();

				foreach (var map in mapTable)
				{
					var source = map["source"];
					var destination = map["destination"];

					var expression = ExpressionCache.Compile(source);

					if (expression == null)
						return StandardResult<SimpleTable>.ReturnError("ExpandTable() error: evaluator returns null");

					if (!expression.IsValid)
						return StandardResult<SimpleTable>.ReturnError("ExpandTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression);

					var result = expression.Evaluate(new Context() { FieldSource = fieldSource });

					if (result.IsError)
						return StandardResult<SimpleTable>.ReturnError("ExpandTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression, result.String);

					destinationRow[destination] = ExpandTable.ToString(result);
				}
			}

			return StandardResult<SimpleTable>.ReturnResult(newTable);
		}
	}
}