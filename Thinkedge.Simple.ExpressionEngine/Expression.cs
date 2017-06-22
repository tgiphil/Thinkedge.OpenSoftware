using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Thinkedge.Simple.ExpressionEngine
{
	public delegate Value Evaluation(ExpressionNode node, Context context);

	public class Expression
	{
		public Parser Parser { get; protected set; }

		public bool IsValid { get { return !Parser.HasError; } }

		protected ExpressionNode Root { get { return Parser.Root; } }

		protected static IMethodSource BuiltInMethods = new BuiltInMethods();

		protected static IAggregateMethodSource BuiltInAggregateMethods = new BuiltInAggregateMethods();

		protected static Context EmptyContext = new Context();

		public Expression(Parser parser)
		{
			Parser = parser;
		}

		public Value Evaluate()
		{
			return Evaluate(EmptyContext);
		}

		public Value Evaluate(Context context)
		{
			if (Parser.HasError)
				return Value.CreateErrorValue(Parser.ErrorMessage);

			if (Root == null)
				return Value.CreateErrorValue("invalid parameter to parser");

			var result = Evaluate(Root, context);

			return result;
		}

		protected static Value Evaluate(ExpressionNode root, Context context)
		{
			Debug.Assert(root != null);

			if (root.Left != null && root.Right != null)
			{
				var left = Evaluate(root.Left, context);

				if (left.IsError)
					return left;

				// shortcut evaluation
				if (root.Token.TokenType == TokenType.And & left.IsBoolean && left.Boolean == false)
				{
					return left;
				}
				else if (root.Token.TokenType == TokenType.Or & left.IsBoolean && left.Boolean == true)
				{
					return left;
				}

				var right = Evaluate(root.Right, context);

				if (right.IsError)
					return right;

				var result = Eval(root.Token.TokenType, left, right);
				return result;
			}
			else if (root.Left != null && root.Right == null)
			{
				var left = Evaluate(root.Left, context);

				if (left.IsError)
					return left;

				var result = Eval(root.Token.TokenType, left);
				return result;
			}
			else if (root.Left == null && root.Right == null)
			{
				if (root.Token.TokenType == TokenType.If)
				{
					var result = IfStatement(root, context);
					return result;
				}
				else if (root.Token.TokenType == TokenType.Field)
				{
					var result = Field(root, context);
					return result;
				}
				else if (root.Token.TokenType == TokenType.Variable)
				{
					var result = Variable(root, context);
					return result;
				}
				else if (root.Token.TokenType == TokenType.Method)
				{
					var result = Method(root, context);
					return result;
				}
				else
				{
					// must be a literal
					var result = EvalLiteral(root.Token);
					return result;
				}
			}

			return Value.CreateErrorValue("unexpected token type: " + root.Token.ToString());
		}

		protected static Value Field(ExpressionNode node, Context context)
		{
			Value value = null;
			var name = node.Token.Value;

			if (context.FieldSource != null)
			{
				value = context.FieldSource.GetField(name);
			}

			if (value == null)
				return Value.CreateErrorValue("unknown field: " + name);

			return value;
		}

		protected static Value Variable(ExpressionNode node, Context context)
		{
			Value value = null;
			var name = node.Token.Value;

			if (context.VariableSource != null)
			{
				value = context.VariableSource.GetVariable(name);
			}

			if (value == null)
				return Value.CreateErrorValue("variable unassigned: " + name);

			return value;
		}

		protected static Value EvalLiteral(Token token)
		{
			switch (token.TokenType)
			{
				case TokenType.IntegerLiteral: return new Value(Convert.ToInt32(token.Value));
				case TokenType.DateLiteral: return new Value(Convert.ToDateTime(token.Value));
				case TokenType.StringLiteral: return new Value(token.Value);
				case TokenType.BooleanTrueLiteral: return new Value(true);
				case TokenType.BooleanFalseLiteral: return new Value(false);
				case TokenType.DecimalLiteral: return new Value(Convert.ToDecimal(token.Value));
				case TokenType.FloatLiteral: return new Value(Convert.ToDouble(token.Value));
				default: break;
			}

			return Value.CreateErrorValue("invalid token type: " + token.ToString());
		}

		protected static Value Eval(TokenType tokenType, Value left, Value right)
		{
			switch (tokenType)
			{
				case TokenType.Addition: return Addition(left, right);
				case TokenType.Subtract: return Subtract(left, right);
				case TokenType.Multiplication: return Multiplication(left, right);
				case TokenType.Division: return Division(left, right);
				case TokenType.And: return And(left, right);
				case TokenType.Or: return Or(left, right);
				case TokenType.CompareEqual: return CompareEqual(left, right);
				case TokenType.CompareNotEqual: return CompareNotEqual(left, right);
				case TokenType.CompareGreaterThanOrEqual: return CompareGreaterThanOrEqual(left, right);
				case TokenType.CompareLessThanOrEqual: return CompareLessThanOrEqual(left, right);
				case TokenType.CompareLessThan: return CompareLessThan(left, right);
				case TokenType.CompareGreaterThan: return CompareGreaterThan(left, right);
				default: break;
			}

			return Value.CreateErrorValue("invalid token type: " + tokenType.ToString());
		}

		protected static Value Eval(TokenType tokenType, Value left)
		{
			switch (tokenType)
			{
				case TokenType.Not: return Not(left);
				case TokenType.Negate: return Negate(left);
				default: break;
			}

			return Value.CreateErrorValue("invalid token type: " + tokenType.ToString());
		}

		protected static Value Not(Value left)
		{
			if (left.IsNull)
				return Value.CreateErrorValue("incompatible operation due to null value: " + left.ToString());

			if (left.IsBoolean)
			{
				return new Value(!left.Boolean);
			}

			return Value.CreateErrorValue("incompatible type for not operator: " + left.ToString());
		}

		protected static Value Negate(Value left)
		{
			if (left.IsInteger)
			{
				if (left.IsNull)
					return Value.CreateNullValue(ValueType.Integer);

				return new Value(-left.Integer);
			}
			else if (left.IsFloat)
			{
				if (left.IsNull)
					return Value.CreateNullValue(ValueType.Float);

				return new Value(-left.Float);
			}
			else if (left.IsDecimal)
			{
				if (left.IsNull)
					return Value.CreateNullValue(ValueType.Decimal);

				return new Value(-left.Decimal);
			}

			return Value.CreateErrorValue("incompatible type for negate operator: " + left.ToString());
		}

		protected static Value Addition(Value left, Value right)
		{
			if (left.IsInteger && right.IsInteger)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Integer);

				return new Value(left.Integer + right.Integer);
			}
			else if (left.IsDate && right.IsInteger)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Date);

				return new Value(left.Date.AddDays(right.Integer));
			}
			else if (left.IsInteger && right.IsDate)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Date);

				return new Value(right.Date.AddDays(left.Integer));
			}
			else if (left.IsString && right.IsString)
			{
				return new Value(left.String + right.String);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Decimal);

				return new Value(left.Decimal + right.Decimal);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Float);

				return new Value(left.Float + right.Float);
			}

			return Value.CreateErrorValue("incompatible types for addition operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value Subtract(Value left, Value right)
		{
			if (left.IsInteger && right.IsInteger)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Integer);

				return new Value(left.Integer - right.Integer);
			}
			else if (left.IsDate && right.IsInteger)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Date);

				return new Value(left.Date.AddDays(-right.Integer));
			}
			else if (left.IsDate && right.IsDate)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Date);

				return new Value((left.Date - right.Date).Days);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Decimal);

				return new Value(left.Decimal - right.Decimal);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Float);

				return new Value(left.Float - right.Float);
			}

			return Value.CreateErrorValue("incompatible types for substraction operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value Multiplication(Value left, Value right)
		{
			if (left.IsInteger && right.IsInteger)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Integer);

				return new Value(left.Integer * right.Integer);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Decimal);

				return new Value(left.Decimal * right.Decimal);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Float);

				return new Value(left.Float * right.Float);
			}

			return Value.CreateErrorValue("incompatible types for multiplication operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value Division(Value left, Value right)
		{
			if (left.IsInteger && right.IsInteger)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Integer);

				//if (right.Integer == 0) ;

				return new Value(left.Integer / right.Integer);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Decimal);

				return new Value(left.Decimal / right.Decimal);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Float);

				return new Value(left.Float / right.Float);
			}

			return Value.CreateErrorValue("incompatible types for division operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value And(Value left, Value right)
		{
			if (left.IsBoolean && right.IsBoolean)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Boolean);

				return new Value(left.Boolean && right.Boolean);
			}

			return Value.CreateErrorValue("incompatible types for and operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value Or(Value left, Value right)
		{
			if (left.IsBoolean && right.IsBoolean)
			{
				if (left.IsNull || right.IsNull)
					return Value.CreateNullValue(ValueType.Boolean);

				return new Value(left.Boolean || right.Boolean);
			}

			return Value.CreateErrorValue("incompatible types for or operator operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareEqual(Value left, Value right)
		{
			if ((left.IsNull || right.IsNull) && left.ValueType == right.ValueType)
				return Value.CreateNullValue(ValueType.Boolean);

			if (left.IsBoolean && right.IsBoolean)
			{
				return new Value(left.Boolean == right.Boolean);
			}
			else if (left.IsInteger && right.IsInteger)
			{
				return new Value(left.Integer == right.Integer);
			}
			else if (left.IsString && right.IsString)
			{
				return new Value(left.String == right.String);
			}
			else if (left.IsDate && right.IsDate)
			{
				return new Value(left.Date == right.Date);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				return new Value(left.Float == right.Float);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				return new Value(left.Decimal == right.Decimal);
			}

			return Value.CreateErrorValue("incompatible types for comparison operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareNotEqual(Value left, Value right)
		{
			if ((left.IsNull || right.IsNull) && left.ValueType == right.ValueType)
				return Value.CreateNullValue(ValueType.Boolean);

			if (left.IsBoolean && right.IsBoolean)
			{
				return new Value(left.Boolean != right.Boolean);
			}
			else if (left.IsInteger && right.IsInteger)
			{
				return new Value(left.Integer != right.Integer);
			}
			else if (left.IsString && right.IsString)
			{
				return new Value(left.String != right.String);
			}
			else if (left.IsDate && right.IsDate)
			{
				return new Value(left.Date != right.Date);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				return new Value(left.Float != right.Float);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				return new Value(left.Decimal != right.Decimal);
			}

			return Value.CreateErrorValue("incompatible types for comparison operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareGreaterThanOrEqual(Value left, Value right)
		{
			if ((left.IsNull || right.IsNull) && left.ValueType == right.ValueType)
				return Value.CreateNullValue(ValueType.Boolean);

			if (left.IsInteger && right.IsInteger)
			{
				return new Value(left.Integer >= right.Integer);
			}
			else if (left.IsDate && right.IsDate)
			{
				return new Value(left.Date >= right.Date);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				return new Value(left.Float >= right.Float);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				return new Value(left.Decimal >= right.Decimal);
			}

			return Value.CreateErrorValue("incompatible types for comparison operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareLessThanOrEqual(Value left, Value right)
		{
			if ((left.IsNull || right.IsNull) && left.ValueType == right.ValueType)
				return Value.CreateNullValue(ValueType.Boolean);

			if (left.IsInteger && right.IsInteger)
			{
				return new Value(left.Integer <= right.Integer);
			}
			else if (left.IsDate && right.IsDate)
			{
				return new Value(left.Date <= right.Date);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				return new Value(left.Float <= right.Float);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				return new Value(left.Decimal <= right.Decimal);
			}

			return Value.CreateErrorValue("incompatible types for comparison operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareLessThan(Value left, Value right)
		{
			if ((left.IsNull || right.IsNull) && left.ValueType == right.ValueType)
				return Value.CreateNullValue(ValueType.Boolean);

			if (left.IsInteger && right.IsInteger)
			{
				return new Value(left.Integer < right.Integer);
			}
			else if (left.IsDate && right.IsDate)
			{
				return new Value(left.Date < right.Date);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				return new Value(left.Float < right.Float);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				return new Value(left.Decimal < right.Decimal);
			}

			return Value.CreateErrorValue("incompatible types for comparison operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareGreaterThan(Value left, Value right)
		{
			if ((left.IsNull || right.IsNull) && left.ValueType == right.ValueType)
				return Value.CreateNullValue(ValueType.Boolean);

			if (left.IsInteger && right.IsInteger)
			{
				return new Value(left.Integer > right.Integer);
			}
			else if (left.IsDate && right.IsDate)
			{
				return new Value(left.Date > right.Date);
			}
			else if (left.IsFloat && right.IsFloat)
			{
				return new Value(left.Float > right.Float);
			}
			else if (left.IsDecimal && right.IsDecimal)
			{
				return new Value(left.Decimal > right.Decimal);
			}

			return Value.CreateErrorValue("incompatible types for comparison operator: " + left.ToString() + " and " + right.ToString());
		}

		protected static Value IfStatement(ExpressionNode node, Context context)
		{
			if (node.Parameters.Count < 2 || node.Parameters.Count > 3)
				return Value.CreateErrorValue("Incomplete if statement");

			var condition = Evaluate(node.Parameters[0], context);

			if (!condition.IsBoolean)
				return Value.CreateErrorValue("if statement condition does not evaluate to true or false");

			if (condition.IsNull)
				return Value.CreateErrorValue("incompatible operation due to null value: " + condition.ToString());

			if (node.Parameters.Count == 2 && !condition.Boolean)
				return new Value(string.Empty); // default value is emptry string when missing else statement

			var value = Evaluate(node.Parameters[condition.Boolean ? 1 : 2], context);

			return value;
		}

		protected static Value Method(ExpressionNode node, Context context)
		{
			// aggregate methods (first)
			var name = node.Token.Value;

			var result = BuiltInAggregateMethods.Evaluate(name, context, node, Evaluate);

			if (result != null)
				return result;

			// non aggregate methods (second)
			var parameters = new List<Value>(node.Parameters.Count);

			foreach (var parameter in node.Parameters)
			{
				result = Evaluate(parameter, context);

				if (result.IsError)
					return result;

				parameters.Add(result);
			}

			result = BuiltInMethods.Evaluate(name, parameters, context);

			if (result != null)
				return result;

			if (context.MethodSource != null)
			{
				result = context.MethodSource.Evaluate(name, parameters, context);

				if (result != null)
					return result;
			}

			return Value.CreateErrorValue("unknown method: " + name);
		}

		public override string ToString()
		{
			return Parser.ToString();
		}

		public static Value ValidateHelper(string method, IList<Value> parameters, int minParameters, IList<ValueType> types)
		{
			if (parameters.Count < minParameters)
			{
				return Value.CreateErrorValue(method + "() error too few parameters");
			}
			if (parameters.Count > types.Count)
			{
				return Value.CreateErrorValue(method + "() too many parameters");
			}

			for (int i = 0; i < types.Count; i++)
			{
				if (i >= parameters.Count)
					return null;

				var type = types[i];
				var parameter = parameters[i];

				if (parameter.ValueType != type)
				{
					var typename = Enum.GetName(typeof(ValueType), type);

					return Value.CreateErrorValue(method + "() parameter #" + i.ToString() + " is not of the expected type: " + typename);
				}
			}

			return null;
		}
	}
}