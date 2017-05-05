using System.Collections.Generic;
using Thinkedge.Common;

namespace Thinkedge.Simple.Expression
{
	public class Parser : BaseStandardResult
	{
		public Tokenizer Tokenizer { get; private set; }

		protected List<Token> Tokens { get { return Tokenizer.Tokens; } }
		protected Token CurrentToken { get { return Tokens[Index]; } }
		protected TokenType CurrentTokenType { get { return CurrentToken.TokenType; } }
		protected bool IsOutOfTokens { get { return Index >= Tokens.Count; } }
		protected int Index = 0;

		public ExpressionNode Root;

		public Parser(Tokenizer tokenizer)
		{
			Tokenizer = tokenizer;

			if (tokenizer.HasError)
			{
				ErrorMessage = tokenizer.ErrorMessage;
				return;
			}

			Parse();
		}

		private void Parse()
		{
			Root = ParseAddSub();
		}

		private ExpressionNode ParseAddSub()
		{
			var lhs = ParseMulDiv();

			while (true)
			{
				if (HasError) return null;

				if (IsOutOfTokens) return lhs;

				Token op = null;

				if (IsAddSubOperand(CurrentToken.TokenType))
				{
					op = CurrentToken;
					Index++;
				}
				else if (IsLogicalOperand(CurrentToken.TokenType))
				{
					op = CurrentToken;
					Index++;
				}
				else
				{
					return lhs;
				}

				var rhs = ParseMulDiv();

				lhs = new ExpressionNode(op, lhs, rhs);
			}
		}

		private ExpressionNode ParseMulDiv()
		{
			var lhs = ParseUnary();

			while (true)
			{
				if (HasError) return null;

				if (IsOutOfTokens) return lhs;

				Token op = null;

				if (IsMulDivOperand(CurrentToken.TokenType))
				{
					op = CurrentToken;
					Index++;
				}
				else if (IsComparisonOperand(CurrentToken.TokenType))
				{
					op = CurrentToken;
					Index++;
				}
				else
				{
					return lhs;
				}

				var rhs = ParseUnary();

				lhs = new ExpressionNode(op, lhs, rhs);
			}
		}

		private ExpressionNode ParseUnary()
		{
			while (true)
			{
				if (HasError) return null;

				if (IsOutOfTokens)
				{
					ErrorMessage = "parser: unexpected end";
					return null;
				}

				if (CurrentTokenType == TokenType.Not)
				{
					var token = CurrentToken;
					Index++;

					var rhs = ParseUnary();

					var node = new ExpressionNode(token, rhs);
					return node;
				}

				if (CurrentTokenType == TokenType.Field)
				{
					var fieldToken = new Token(TokenType.Field, CurrentToken.Value, Index);
					Index++;

					var node = new ExpressionNode(fieldToken);
					return node;
				}
				else if (CurrentTokenType == TokenType.Variable)
				{
					var variableToken = new Token(TokenType.Variable, CurrentToken.Value, Index);
					Index++;

					var node = new ExpressionNode(variableToken);
					return node;
				}
				else if (CurrentTokenType == TokenType.Method)
				{
					var methodToken = new Token(TokenType.Method, CurrentToken.Value, Index);
					Index++;

					Index++; // skip opening paraens

					var parameters = new List<ExpressionNode>();

					while (CurrentTokenType != TokenType.CloseParens)
					{
						if (CurrentTokenType == TokenType.Comma)
						{
							Index++;
							continue;
						}

						var parameter = ParseAddSub();

						parameters.Add(parameter);
					}

					Index++; // skip closing parens

					var node = new ExpressionNode(methodToken, parameters);
					return node;
				}

				return ParseLeaf();
			}
		}

		private ExpressionNode ParseLeaf()
		{
			if (IsLiteral(CurrentToken.TokenType))
			{
				var node = new ExpressionNode(CurrentToken);
				Index++;
				return node;
			}

			if (CurrentToken.TokenType == TokenType.OpenParens)
			{
				Index++;

				var node = ParseAddSub();

				if (CurrentToken.TokenType != TokenType.CloseParens)
				{
					ErrorMessage = "error at " + CurrentToken.Index.ToString() + ": missing closing parenthesis token";
					return null;
				}

				Index++;

				return node;
			}

			if (CurrentTokenType == TokenType.Field)
			{
				var node = new ExpressionNode(CurrentToken);
				Index++;
				return node;
			}
			else if (CurrentTokenType == TokenType.Variable)
			{
				var node = new ExpressionNode(CurrentToken);
				Index++;
				return node;
			}

			ErrorMessage = "error at " + CurrentToken.Index.ToString() + ": unexpected token: " + CurrentToken.ToString();
			return null;
		}

		protected static bool IsLiteral(TokenType tokenType)
		{
			return
				tokenType == TokenType.IntegerLiteral ||
				tokenType == TokenType.DecimalLiteral ||
				tokenType == TokenType.StringLiteral ||
				tokenType == TokenType.DateLiteral ||
				tokenType == TokenType.FloatLiteral ||
				tokenType == TokenType.BooleanTrueLiteral ||
				tokenType == TokenType.BooleanFalseLiteral;
		}

		protected static bool IsAddSubOperand(TokenType tokenType)
		{
			return tokenType == TokenType.Addition || tokenType == TokenType.Subtract;
		}

		protected static bool IsMulDivOperand(TokenType tokenType)
		{
			return tokenType == TokenType.Multiplication || tokenType == TokenType.Division || tokenType == TokenType.Modulus;
		}

		protected static bool IsLogicalOperand(TokenType tokenType)
		{
			return tokenType == TokenType.And || tokenType == TokenType.Or;
		}

		protected static bool IsComparisonOperand(TokenType tokenType)
		{
			return
				tokenType == TokenType.CompareEqual ||
				tokenType == TokenType.CompareNotEqual ||
				tokenType == TokenType.CompareGreaterThanOrEqual ||
				tokenType == TokenType.CompareLessThanOrEqual ||
				tokenType == TokenType.CompareLessThan ||
				tokenType == TokenType.CompareGreaterThan;
		}

		public override string ToString()
		{
			return Tokenizer.Expression;
		}
	}
}