using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Thinkedge.Simple.Expression
{
	public class Evaluator
	{
		public Parser Parser { get; protected set; }

		public bool IsValid { get; protected set; }

		protected ExpressionNode Root { get { return Parser.Root; } }

		protected IMethodSource BuiltInMethods = new BuiltInMethods();

		public Evaluator(Parser parser)
		{
			Parser = parser;

			IsValid = !Parser.HasError;
		}

		public Value Evaluate()
		{
			return Evaluate(null, null, null);
		}

		public Value Evaluate(IVariableSource variableSource = null, IMethodSource methodSource = null)
		{
			return Evaluate(variableSource, null, methodSource);
		}

		public Value Evaluate(ITableSource tableSource = null, IMethodSource methodSource = null)
		{
			return Evaluate(null, tableSource, methodSource);
		}

		public Value Evaluate(IVariableSource variableSource = null, ITableSource tableSource = null, IMethodSource methodSource = null)
		{
			if (Parser.HasError)
			{
				return Value.CreateErrorValue(Parser.ErrorMessage);
			}

			if (Root == null)
			{
				return Value.CreateErrorValue("invalid parameter to parser");
			}

			var result = Evaluate(Root, BuiltInMethods, variableSource, tableSource, methodSource);

			return result;
		}

		protected static Value Evaluate(ExpressionNode root, IMethodSource builtInMethods, IVariableSource variableSource, ITableSource tableSource, IMethodSource methodSource)
		{
			Debug.Assert(root != null);

			if (root.Left != null && root.Right != null)
			{
				var left = Evaluate(root.Left, builtInMethods, variableSource, tableSource, methodSource);

				// shortcut evaluation
				if (root.Token.TokenType == TokenType.And & left.IsBoolean && left.Boolean == false)
				{
					return left;
				}
				else if (root.Token.TokenType == TokenType.Or & left.IsBoolean && left.Boolean == true)
				{
					return left;
				}

				var right = Evaluate(root.Right, builtInMethods, variableSource, tableSource, methodSource);

				var result = Eval(root.Token.TokenType, left, right);
				return result;
			}
			else if (root.Left != null && root.Right == null)
			{
				var left = Evaluate(root.Left, builtInMethods, variableSource, tableSource, methodSource);

				var result = Eval(root.Token.TokenType, left);
				return result;
			}
			else if (root.Left == null && root.Right == null)
			{
				if (root.Token.TokenType == TokenType.If)
				{
					var result = IfStatement(root, builtInMethods, variableSource, tableSource, methodSource);
					return result;
				}
				else if (root.Token.TokenType == TokenType.Field)
				{
					var result = Field(root, tableSource);
					return result;
				}
				else if (root.Token.TokenType == TokenType.Variable)
				{
					var result = Variable(root, variableSource);
					return result;
				}
				else if (root.Token.TokenType == TokenType.Method)
				{
					var result = Method(root, builtInMethods, variableSource, tableSource, methodSource);
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

		protected static Value Field(ExpressionNode node, ITableSource tableSource)
		{
			Value value = null;
			var name = node.Token.Value;

			if (tableSource != null)
			{
				value = tableSource.GetField(name);
			}

			if (value == null)
			{
				return Value.CreateErrorValue("unknown field: " + name);
			}

			return value;
		}

		protected static Value Variable(ExpressionNode node, IVariableSource variableSource)
		{
			Value value = null;
			var name = node.Token.Value;

			if (variableSource != null)
			{
				value = variableSource.GetVariable(name);
			}

			if (value == null)
			{
				return Value.CreateErrorValue("variable unassigned: " + name);
			}

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

			return Value.CreateErrorValue("invalid token type:" + token.ToString());
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

			return Value.CreateErrorValue("invalid token type:" + tokenType.ToString());
		}

		protected static Value Eval(TokenType tokenType, Value left)
		{
			switch (tokenType)
			{
				case TokenType.Not: return Not(left);
				case TokenType.Negate: return Negate(left);
				default: break;
			}

			return Value.CreateErrorValue("invalid token type:" + tokenType.ToString());
		}

		protected static Value Not(Value left)
		{
			if (left.IsBoolean)
			{
				return new Value(!left.Boolean);
			}

			return Value.CreateErrorValue("incompatible type for not operator:" + left.ToString());
		}

		protected static Value Negate(Value left)
		{
			if (left.IsInteger)
			{
				return new Value(-left.Integer);
			}
			else if (left.IsFloat)
			{
				return new Value(-left.Float);
			}

			return Value.CreateErrorValue("incompatible type for Negate operator:" + left.ToString());
		}

		protected static Value Addition(Value left, Value right)
		{
			if (left.IsInteger && right.IsInteger)
			{
				return new Value(left.Integer + right.Integer);
			}
			else if (left.IsDate && right.IsInteger)
			{
				return new Value(left.Date.AddDays(right.Integer));
			}
			if (left.IsString && right.IsString)
			{
				return new Value(left.String + right.String);
			}

			return Value.CreateErrorValue("incompatible types for addition operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value Subtract(Value left, Value right)
		{
			if (left.IsInteger && right.IsInteger)
			{
				return new Value(left.Integer - right.Integer);
			}
			else if (left.IsDate && right.IsInteger)
			{
				return new Value(left.Date.AddDays(-right.Integer));
			}

			return Value.CreateErrorValue("incompatible types for substraction operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value Multiplication(Value left, Value right)
		{
			if (left.IsInteger && right.IsInteger)
			{
				return new Value(left.Integer * right.Integer);
			}

			return Value.CreateErrorValue("incompatible types for multiplication operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value Division(Value left, Value right)
		{
			if (left.IsInteger && right.IsInteger)
			{
				//if (right.Integer == 0) ;

				return new Value(left.Integer / right.Integer);
			}

			return Value.CreateErrorValue("incompatible types for division operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value And(Value left, Value right)
		{
			if (left.IsBoolean && right.IsBoolean)
			{
				return new Value(left.Boolean && right.Boolean);
			}

			return Value.CreateErrorValue("incompatible types for and operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value Or(Value left, Value right)
		{
			if (left.IsBoolean && right.IsBoolean)
			{
				return new Value(left.Boolean || right.Boolean);
			}

			return Value.CreateErrorValue("incompatible types for or operator operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareEqual(Value left, Value right)
		{
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

			return Value.CreateErrorValue("incompatible types for comparison operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareNotEqual(Value left, Value right)
		{
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

			return Value.CreateErrorValue("incompatible types for comparison operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareGreaterThanOrEqual(Value left, Value right)
		{
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

			return Value.CreateErrorValue("incompatible types for comparison operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareLessThanOrEqual(Value left, Value right)
		{
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

			return Value.CreateErrorValue("incompatible types for comparison operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareLessThan(Value left, Value right)
		{
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

			return Value.CreateErrorValue("incompatible types for comparison operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value CompareGreaterThan(Value left, Value right)
		{
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

			return Value.CreateErrorValue("incompatible types for comparison operator:" + left.ToString() + " and " + right.ToString());
		}

		protected static Value IfStatement(ExpressionNode node, IMethodSource builtInMethods, IVariableSource variableSource, ITableSource tableSource, IMethodSource methodSource)
		{
			if (node.Parameters.Count < 2 || node.Parameters.Count > 3)
				return Value.CreateErrorValue("Incomplete if statement");

			var condition = Evaluate(node.Parameters[0], builtInMethods, variableSource, tableSource, methodSource);

			if (!condition.IsBoolean)
				return Value.CreateErrorValue("if statement condition does not evaluate to true or false");

			if (node.Parameters.Count == 2 && !condition.Boolean)
				return new Value(string.Empty); // default value is emptry string when missing else statement

			var value = Evaluate(node.Parameters[condition.Boolean ? 1 : 2], builtInMethods, variableSource, tableSource, methodSource);

			return value;
		}

		protected static Value Method(ExpressionNode node, IMethodSource builtInMethods, IVariableSource variableSource, ITableSource tableSource, IMethodSource methodSource)
		{
			var parameters = new List<Value>(node.Parameters.Count);

			foreach (var parameter in node.Parameters)
			{
				var results = Evaluate(parameter, builtInMethods, variableSource, tableSource, methodSource);

				parameters.Add(results);
			}

			var name = node.Token.Value;

			var result = builtInMethods.Evaluate(name, parameters);

			if (result != null)
				return result;

			if (methodSource != null)
			{
				result = methodSource.Evaluate(name, parameters);

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
					var typename = System.Enum.GetName(typeof(ValueType), type);

					return Value.CreateErrorValue(method + "() parameter #" + i.ToString() + " is not of the expected type: " + typename);
				}
			}

			return null;
		}
	}
}