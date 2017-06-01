using System.Collections.Generic;

namespace Thinkedge.Simple.Evaluator
{
	public class BuiltInAggregateMethods : IAggregateMethodSource
	{
		Value IAggregateMethodSource.Evaluate(string name, Context context, ExpressionNode node, Evaluation eval)
		{
			switch (name)
			{
				//short versions
				case "Count": return Count(node, context, eval);
				case "Sum": return Sum(node, context, eval);
				case "Average": return Sum(node, context, eval);
				//Max
				//Min
				//STDEV
				//VAR - variance

				default: break;
			}

			return null;
		}

		public static Value Count(ExpressionNode node, Context context, Evaluation eval)
		{
			if (context.AggregateFieldSource == null)
				return Value.CreateErrorValue("Count() error - not allowed in this context");

			int rows = 0;

			foreach (var row in context.AggregateFieldSource.Rows)
			{
				rows++;
			}

			return new Value(rows);
		}

		public static Value Sum(ExpressionNode node, Context context, Evaluation eval)
		{
			if (context.AggregateFieldSource == null)
				return Value.CreateErrorValue("Sum() error - not allowed in this context");

			Value sum = null;

			var rowContext = new Context() { VariableSource = context.VariableSource, MethodSource = context.MethodSource };

			foreach (var row in context.AggregateFieldSource.Rows)
			{
				rowContext.FieldSource = row;

				var parameters = EvaulateParameters(node, rowContext, eval);

				if (parameters.Count < 1)
					return Value.CreateErrorValue("Sum() error - missing parameter");

				var element = parameters[0];

				if (!(element.IsInteger || element.IsDecimal || element.IsFloat))
					return Value.CreateErrorValue("Sum() error - invalid parameter: " + element.ToString());

				if (sum == null)
				{
					sum = element;
					continue;
				}
				else
				{
					if (element.IsError)
						return element;

					if (element.ValueType != sum.ValueType)
						return Value.CreateErrorValue("Sum() error - mismatch parameter: " + element.ToString() + " and " + sum.ToString());

					if (element.IsInteger)
						sum = new Value(sum.Integer + element.Integer);
					else if (element.IsDecimal)
						sum = new Value(sum.Decimal + element.Decimal);
					else if (element.IsFloat)
						sum = new Value(sum.Float + element.Float);
				}
			}

			return sum;
		}

		public static Value Average(ExpressionNode node, Context context, Evaluation eval)
		{
			if (context.AggregateFieldSource == null)
				return Value.CreateErrorValue("Average() error - not allowed in this context");

			Value sum = null;
			int count = 0;

			var rowContext = new Context() { VariableSource = context.VariableSource, MethodSource = context.MethodSource };

			foreach (var row in context.AggregateFieldSource.Rows)
			{
				count++;
				rowContext.FieldSource = row;

				var parameters = EvaulateParameters(node, rowContext, eval);

				if (parameters.Count < 1)
					return Value.CreateErrorValue("Average() error - missing parameter");

				var element = parameters[0];

				if (!(element.IsInteger || element.IsDecimal || element.IsFloat))
					return Value.CreateErrorValue("Average() error - invalid parameter: " + element.ToString());

				if (sum == null)
				{
					sum = element;
					continue;
				}
				else
				{
					if (element.IsError)
						return element;

					if (element.ValueType != sum.ValueType)
						return Value.CreateErrorValue("Sum() error - mismatch parameter: " + element.ToString() + " and " + sum.ToString());

					if (element.IsInteger)
						sum = new Value(sum.Integer + element.Integer);
					else if (element.IsDecimal)
						sum = new Value(sum.Decimal + element.Decimal);
					else if (element.IsFloat)
						sum = new Value(sum.Float + element.Float);
				}
			}

			if (sum.IsInteger)
				return new Value(sum.Integer / count);
			else if (sum.IsDecimal)
				return new Value(sum.Decimal / count);
			else if (sum.IsFloat)
				return new Value(sum.Float / count);

			return Value.CreateErrorValue("Sum() error - something went wrong");
		}

		public static List<Value> EvaulateParameters(ExpressionNode node, Context context, Evaluation eval)
		{
			var parameters = new List<Value>(node.Parameters.Count);

			foreach (var parameter in node.Parameters)
			{
				var result = eval(parameter, context);

				parameters.Add(result);
			}

			return parameters;
		}
	}
}