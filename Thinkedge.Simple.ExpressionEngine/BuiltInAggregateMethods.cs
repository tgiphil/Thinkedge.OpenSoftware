using System;
using System.Collections.Generic;

namespace Thinkedge.Simple.ExpressionEngine
{
	public class BuiltInAggregateMethods : IAggregateMethodSource
	{
		Value IAggregateMethodSource.Evaluate(string name, Context context, ExpressionNode node, Evaluation eval)
		{
			switch (name)
			{
				//short versions
				case "Count": return Count(node, context, eval);
				case "CountNonEmpty": return CountNonEmpty(node, context, eval);
				case "Sum": return Sum(node, context, eval);
				case "Average": return Average(node, context, eval);
				case "Max": return Max(node, context, eval);
				case "Min": return Min(node, context, eval);
				case "VarianceFromSample": return VarianceAndStandardDeviation(node, context, eval, false, false);
				case "VarianceFromPopulation": return VarianceAndStandardDeviation(node, context, eval, false, true);
				case "StandardDeviationFromSample": return VarianceAndStandardDeviation(node, context, eval, true, false);
				case "StandardDeviationFromPopulation": return VarianceAndStandardDeviation(node, context, eval, true, true);

				// nicknames
				case "Var": return VarianceAndStandardDeviation(node, context, eval, false, false);
				case "VarP": return VarianceAndStandardDeviation(node, context, eval, false, true);
				case "Stdev": return VarianceAndStandardDeviation(node, context, eval, true, false);
				case "StdevP": return VarianceAndStandardDeviation(node, context, eval, true, true);

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

		public static Value CountNonEmpty(ExpressionNode node, Context context, Evaluation eval)
		{
			if (context.AggregateFieldSource == null)
				return Value.CreateErrorValue("Count() error - not allowed in this context");

			int rows = 0;

			var rowContext = new Context() { VariableSource = context.VariableSource, MethodSource = context.MethodSource };

			foreach (var row in context.AggregateFieldSource.Rows)
			{
				rowContext.FieldSource = row;

				var parameters = EvaulateParameters(node, rowContext, eval);

				if (parameters.Count < 1)
					return Value.CreateErrorValue("Sum() error - missing parameter");

				var element = parameters[0];

				if (element.IsNull || element.IsStringEmptyNonWhiteSpace)
					continue; /// skip it

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

				if (element.IsNull || element.IsStringEmptyNonWhiteSpace)
					continue; /// skip it

				if (!(element.IsInteger || element.IsDecimal || element.IsFloat))
					return Value.CreateErrorValue("Sum() error - invalid parameter: " + element.ToString());

				if (sum == null)
				{
					sum = element;
					continue;
				}

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

			return sum;
		}

		public static Value Average(ExpressionNode node, Context context, Evaluation eval)
		{
			if (context.AggregateFieldSource == null)
				return Value.CreateErrorValue(node.Token.Value + "() error - not allowed in this context");

			Value sum = null;
			int count = 0;

			var rowContext = new Context() { VariableSource = context.VariableSource, MethodSource = context.MethodSource };

			foreach (var row in context.AggregateFieldSource.Rows)
			{
				rowContext.FieldSource = row;

				var parameters = EvaulateParameters(node, rowContext, eval);

				if (parameters.Count < 1)
					return Value.CreateErrorValue(node.Token.Value + "() error - missing parameter");

				var element = parameters[0];

				if (element.IsNull || element.IsStringEmptyNonWhiteSpace)
					continue; /// skip it

				if (!(element.IsInteger || element.IsDecimal || element.IsFloat))
					return Value.CreateErrorValue(node.Token.Value + "() error - invalid parameter: " + element.ToString());

				count++;

				if (sum == null)
				{
					sum = element;
					continue;
				}

				if (element.IsError)
					return element;

				if (element.ValueType != sum.ValueType)
					return Value.CreateErrorValue(node.Token.Value + "() error - mismatch parameter: " + element.ToString() + " and " + sum.ToString());

				if (element.IsInteger)
					sum = new Value(sum.Integer + element.Integer);
				else if (element.IsDecimal)
					sum = new Value(sum.Decimal + element.Decimal);
				else if (element.IsFloat)
					sum = new Value(sum.Float + element.Float);
			}

			if (count == 0)
				return new Value(string.Empty);

			if (sum.IsInteger)
				return new Value(sum.Integer / count);
			else if (sum.IsDecimal)
				return new Value(sum.Decimal / count);
			else if (sum.IsFloat)
				return new Value(sum.Float / count);

			return Value.CreateErrorValue(node.Token.Value + "() error - something went wrong");
		}

		public static Value Max(ExpressionNode node, Context context, Evaluation eval)
		{
			if (context.AggregateFieldSource == null)
				return Value.CreateErrorValue("Max() error - not allowed in this context");

			Value max = null;

			var rowContext = new Context() { VariableSource = context.VariableSource, MethodSource = context.MethodSource };

			foreach (var row in context.AggregateFieldSource.Rows)
			{
				rowContext.FieldSource = row;

				var parameters = EvaulateParameters(node, rowContext, eval);

				if (parameters.Count < 1)
					return Value.CreateErrorValue("Max() error - missing parameter");

				var element = parameters[0];

				if (element.IsNull || element.IsStringEmptyNonWhiteSpace)
					continue; /// skip it

				if (!(element.IsInteger || element.IsDecimal || element.IsFloat || element.IsDate))
					return Value.CreateErrorValue("Max() error - invalid parameter: " + element.ToString());

				if (max == null)
				{
					max = element;
					continue;
				}

				if (element.IsError)
					return element;

				if (element.ValueType != max.ValueType)
					return Value.CreateErrorValue("Max() error - mismatch parameter: " + element.ToString() + " and " + max.ToString());

				if (element.IsInteger)
					max = max.Integer > element.Integer ? max : element;
				else if (element.IsDecimal)
					max = max.Decimal > element.Decimal ? max : element;
				else if (element.IsFloat)
					max = max.Float > element.Float ? max : element;
				else if (element.IsDate)
					max = max.Date > element.Date ? max : element;
			}

			if (max == null)
				return new Value(string.Empty);

			return max;
		}

		public static Value Min(ExpressionNode node, Context context, Evaluation eval)
		{
			if (context.AggregateFieldSource == null)
				return Value.CreateErrorValue("Min() error - not allowed in this context");

			Value min = null;

			var rowContext = new Context() { VariableSource = context.VariableSource, MethodSource = context.MethodSource };

			foreach (var row in context.AggregateFieldSource.Rows)
			{
				rowContext.FieldSource = row;

				var parameters = EvaulateParameters(node, rowContext, eval);

				if (parameters.Count < 1)
					return Value.CreateErrorValue("Min() error - missing parameter");

				var element = parameters[0];

				if (element.IsNull || element.IsStringEmptyNonWhiteSpace)
					continue; /// skip it

				if (!(element.IsInteger || element.IsDecimal || element.IsFloat || element.IsDate))
					return Value.CreateErrorValue("Min() error - invalid parameter: " + element.ToString());

				if (min == null)
				{
					min = element;
					continue;
				}

				if (element.IsError)
					return element;

				if (element.ValueType != min.ValueType)
					return Value.CreateErrorValue("Min() error - mismatch parameter: " + element.ToString() + " and " + min.ToString());

				if (element.IsInteger)
					min = min.Integer < element.Integer ? min : element;
				else if (element.IsDecimal)
					min = min.Decimal < element.Decimal ? min : element;
				else if (element.IsFloat)
					min = min.Float < element.Float ? min : element;
				else if (element.IsDate)
					min = min.Date < element.Date ? min : element;
			}

			if (min == null)
				return new Value(string.Empty);

			return min;
		}

		public static Value VarianceAndStandardDeviation(ExpressionNode node, Context context, Evaluation eval, bool standardDeviation, bool population)
		{
			if (context.AggregateFieldSource == null)
				return Value.CreateErrorValue(node.Token.Value + "() error - not allowed in this context");

			double sum = 0;
			int count = 0;

			var rowContext = new Context() { VariableSource = context.VariableSource, MethodSource = context.MethodSource };

			// pass one for average
			foreach (var row in context.AggregateFieldSource.Rows)
			{
				rowContext.FieldSource = row;

				var parameters = EvaulateParameters(node, rowContext, eval);

				if (parameters.Count < 1)
					return Value.CreateErrorValue(node.Token.Value + "() error - missing parameter");

				var element = parameters[0];

				if (element.IsNull || element.IsStringEmptyNonWhiteSpace)
					continue; /// skip it

				if (!(element.IsInteger || element.IsDecimal || element.IsFloat))
					return Value.CreateErrorValue(node.Token.Value + "() error - invalid parameter: " + element.ToString());

				count++;

				if (element.IsInteger)
					sum = sum + element.Integer;
				else if (element.IsDecimal)
					sum = sum + (double)element.Decimal;
				else if (element.IsFloat)
					sum = sum + element.Float;
			}

			if (!population)
				count--;

			if (count == 0)
				return new Value(string.Empty);

			double mean = sum / count;

			// pass two for variance
			double sum2 = 0;
			foreach (var row in context.AggregateFieldSource.Rows)
			{
				rowContext.FieldSource = row;

				var parameters = EvaulateParameters(node, rowContext, eval);

				if (parameters.Count < 1)
					return Value.CreateErrorValue(node.Token.Value + "() error - missing parameter");

				var element = parameters[0];

				if (element.IsNull || element.IsStringEmptyNonWhiteSpace)
					continue; /// skip it

				if (!(element.IsInteger || element.IsDecimal || element.IsFloat))
					return Value.CreateErrorValue(node.Token.Value + "() error - invalid parameter: " + element.ToString());

				if (element.IsError)
					return element;

				if (element.IsInteger)
					sum2 = sum2 + ((element.Integer - mean) * (element.Integer - mean));
				else if (element.IsDecimal)
					sum2 = sum2 + (((double)element.Decimal - mean) * ((double)element.Decimal - mean));
				else if (element.IsFloat)
					sum2 = sum2 + ((element.Float - mean) * (element.Float - mean));
			}

			double variance = sum2 / count;

			if (standardDeviation)
			{
				double std = Math.Sqrt(variance);
				return new Value(std);
			}
			else
			{
				return new Value(variance);
			}
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