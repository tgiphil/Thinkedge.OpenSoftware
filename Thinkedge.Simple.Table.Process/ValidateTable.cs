using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Process
{
	public class ValidateTable : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable validationRules, SimpleTable validationMap)
		{
			return new ValidateTable().ExecuteEx(sourceTable, validationRules, validationMap);
		}

		internal StandardResult<SimpleTable> ExecuteEx(SimpleTable sourceTable, SimpleTable validationRules, SimpleTable validationMap)
		{
			ClearError();

			var resultsTable = new SimpleTable();
			var cache = new EvaluatorCache();

			foreach (var map in validationMap)
			{
				var evaluator = cache.Compile(map["source"]);

				if (evaluator == null)
					return ReturnError<SimpleTable>("validation error: evaluator returns null");

				resultsTable.AddColumnName(map["destination"]);
			}

			var tableSource = new TableDataSource();
			var variableSource = new VariableSource();

			foreach (var sourceRow in sourceTable)
			{
				tableSource.Row = sourceRow;

				foreach (var rule in validationRules)
				{
					var match = rule["Match"];

					if (string.IsNullOrWhiteSpace(match))
						continue;

					var evaluator = cache.Compile(match);

					if (evaluator == null || !evaluator.IsValid)
						return ReturnError<SimpleTable>("validation error: evaluator returns null");

					var result = evaluator.Evaluate(variableSource, tableSource);

					if (!result.Boolean)
						continue;

					var error = rule["Message"];

					var evaluator2 = cache.Compile(error);

					if (evaluator2 == null || !evaluator2.IsValid)
						return ReturnError<SimpleTable>("validation error: evaluator returns null");

					var result2 = evaluator2.Evaluate(variableSource, tableSource);

					if (result2.IsError)
						return ReturnError<SimpleTable>(result2.String);

					variableSource.ClearVariables();
					variableSource.SetVariable("Message", result2.String);

					var row = resultsTable.CreateRow();

					foreach (var map in validationMap)
					{
						var evaluator3 = cache.Compile(map["source"]);

						if (evaluator3 == null || !evaluator3.IsValid)
							return ReturnError<SimpleTable>("validation error: evaluator returns null");

						var result3 = evaluator3.Evaluate(variableSource, tableSource);

						if (result3.IsError)
						{
							return ReturnError<SimpleTable>(result3.String);
						}

						row.SetField(map["destination"], result3.String);
					}
				}
			}

			return ReturnResult<SimpleTable>(resultsTable);
		}
	}
}