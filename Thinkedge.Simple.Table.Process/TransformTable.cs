using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Process
{
	public class TransformTable : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable mapTable)
		{
			return new TransformTable().ExecuteEx(sourceTable, mapTable);
		}

		internal StandardResult<SimpleTable> ExecuteEx(SimpleTable sourceTable, SimpleTable mapTable)
		{
			if (mapTable == null)
				return ReturnError<SimpleTable>("TransformTable() error: map table null");

			var newTable = new SimpleTable();
			var cache = new EvaluatorCache();

			// create columns
			foreach (var map in mapTable)
			{
				var evaluator = cache.Compile(map["source"]);

				if (evaluator == null)
					return ReturnError<SimpleTable>("TransformTable() error: evaluator returns null");

				newTable.AddColumnName(map["destination"]);
			}

			var tableSource = new TableDataSource();

			foreach (var sourceRow in sourceTable)
			{
				tableSource.Row = sourceRow;

				var destinationRow = newTable.CreateRow();

				foreach (var map in mapTable)
				{
					var source = map["source"];
					var destination = map["destination"];

					var evaluator = cache.Compile(source);

					if (evaluator == null)
						return ReturnError<SimpleTable>("ExpandTable() error: evaluator returns null");

					if (!evaluator.IsValid)
						return ReturnError<SimpleTable>("ExpandTable() error: occurred during evaluating: " + evaluator.Parser.Tokenizer.Expression);

					var result = evaluator.Evaluate(tableSource);

					if (result.IsError)
						return ReturnError<SimpleTable>("ExpandTable() error: occurred during evaluating: " + evaluator.Parser.Tokenizer.Expression, result.String);

					destinationRow[destination] = ExpandTable.ToString(result);
				}
			}

			return ReturnResult<SimpleTable>(newTable);
		}
	}
}