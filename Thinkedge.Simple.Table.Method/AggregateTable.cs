using System.Collections.Generic;
using Thinkedge.Common.Result;
using Thinkedge.Simple.ExpressionEngine;

namespace Thinkedge.Simple.Table.Method
{
	public static class AggregateTable
	{
		private static IList<string> aggregateMethodsNames = new string[] { "Count", "Sum", "Average", "Max", "Min", "Min", "VarianceFromSample", "VarianceFromPopulation", "StandardDeviationFromSample", "StandardDeviationFromPopulation", "Var", "VarP", "Stdev", "StdevP", "CountNonEmpty" };

		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, SimpleTable aggregateTable)
		{
			if (sourceTable == null)
				return StandardResult<SimpleTable>.ReturnError("AggregateTable() error: source table null");
			if (aggregateTable == null)
				return StandardResult<SimpleTable>.ReturnError("AggregateTable() error: aggregate table null");

			var newTable = new SimpleTable();

			var nonaggregateFields = new Dictionary<string, Expression>();
			var aggregateFields = new Dictionary<string, Expression>();

			// create columns
			foreach (var map in aggregateTable)
			{
				var source = map["source"];
				var destination = map["destination"];
				newTable.AddColumnName(destination);

				var expression = ExpressionCache.Compile(source);

				if (expression == null)
					return StandardResult<SimpleTable>.ReturnError("AggregateTable() error: evaluator returns null", "Source: " + source);

				// does expression contain any aggregate methods?
				bool aggregateMethods = false;
				foreach (var token in expression.Parser.Tokenizer.Tokens)
				{
					if (token.TokenType == TokenType.Method && aggregateMethodsNames.Contains(token.Value))
					{
						aggregateMethods = true;
						break;
					}
				}

				if (aggregateMethods)
				{
					aggregateFields.Add(destination, expression);
				}
				else
				{
					nonaggregateFields.Add(destination, expression);
				}
			}

			var associateBuckets = new Dictionary<SimpleTableRow, List<SimpleTableRow>>();

			// create new rows with non-aggregate expressions & track assocations
			{
				var fieldSource = new FieldDataSource();
				var context = new Context() { FieldSource = fieldSource };
				var bucketsMap = new Dictionary<List<string>, SimpleTableRow>();

				foreach (var sourceRow in sourceTable)
				{
					fieldSource.Row = sourceRow;

					var fields = new List<string>();

					foreach (var field in nonaggregateFields)
					{
						var destination = field.Key;
						var expression = field.Value;

						var result = expression.Evaluate(context);

						if (result.IsError)
							return StandardResult<SimpleTable>.ReturnError("AggregateTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression, result.String);

						fields.Add(ExpandTable.ToString(result));
					}

					var row = Find(fields, bucketsMap);

					if (row == null)
					{
						row = newTable.CreateRow();
						bucketsMap.Add(fields, row);

						// populate non-aggregated rows
						int index = 0;
						foreach (var non in nonaggregateFields)
						{
							var destination = non.Key;
							var expression = non.Value;

							var result = fields[index++];

							row[destination] = result;
						}
					}

					List<SimpleTableRow> bucketRows;
					if (!associateBuckets.TryGetValue(row, out bucketRows))
					{
						bucketRows = new List<SimpleTableRow>();
						associateBuckets.Add(row, bucketRows);
					}

					bucketRows.Add(sourceRow);
				}
			}

			// evaluation aggregate expressions
			{
				var fieldSource = new FieldDataSource();
				var aggregateFieldSource = new AggregateFieldSource();
				var context = new Context() { FieldSource = fieldSource, AggregateFieldSource = aggregateFieldSource };

				foreach (var aggregateRow in newTable)
				{
					var bucketRows = associateBuckets[aggregateRow];

					fieldSource.Row = bucketRows[0]; // any row -- note: undefine to reference non-aggregate fields outside of the aggregate expression
					aggregateFieldSource.TableRows = bucketRows;

					foreach (var field in aggregateFields)
					{
						var destination = field.Key;
						var expression = field.Value;

						var result = expression.Evaluate(context);

						if (result.IsError)
							return StandardResult<SimpleTable>.ReturnError("AggregateTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression, result.String);

						aggregateRow[destination] = ExpandTable.ToString(result);
					}
				}
			}

			return StandardResult<SimpleTable>.ReturnResult(newTable);
		}

		private static SimpleTableRow Find(List<string> fields, Dictionary<List<string>, SimpleTableRow> collection)
		{
			foreach (var item in collection)
			{
				var list = item.Key;

				bool match = false;

				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == fields[i])
					{
						match = true;
					}
					else
					{
						match = false;
						break;
					}
				}

				if (match)
				{
					return item.Value;
				}
			}

			return null;
		}
	}
}