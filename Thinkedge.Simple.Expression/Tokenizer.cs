using System.Collections.Generic;
using Thinkedge.Common;

namespace Thinkedge.Simple.Expression
{
	public class Tokenizer : BaseStandardResult
	{
		public string Expression { get; protected set; }

		protected int Index { get; set; } = 0;

		public List<Token> Tokens { get; protected set; } = new List<Token>();

		internal static List<KeyValuePair<string, TokenType>> operands = new List<KeyValuePair<string, TokenType>>()
		{
			new KeyValuePair<string, TokenType>("(", TokenType.OpenParens),
			new KeyValuePair<string, TokenType>(")", TokenType.CloseParens),
			new KeyValuePair<string, TokenType>("!=", TokenType.CompareNotEqual),
			new KeyValuePair<string, TokenType>("==", TokenType.CompareEqual),
			new KeyValuePair<string, TokenType>(">=", TokenType.CompareGreaterThanOrEqual),
			new KeyValuePair<string, TokenType>("<=", TokenType.CompareLessThanOrEqual),
			new KeyValuePair<string, TokenType>("&&", TokenType.And),
			new KeyValuePair<string, TokenType>("||", TokenType.Or),
			new KeyValuePair<string, TokenType>("<", TokenType.CompareLessThan),
			new KeyValuePair<string, TokenType>(">", TokenType.CompareGreaterThan),
			new KeyValuePair<string, TokenType>(",", TokenType.Comma),
			new KeyValuePair<string, TokenType>("!", TokenType.Not),
			new KeyValuePair<string, TokenType>("+", TokenType.Addition),
			new KeyValuePair<string, TokenType>("-", TokenType.Subtract),
			new KeyValuePair<string, TokenType>("*", TokenType.Multiplication),
			new KeyValuePair<string, TokenType>("/", TokenType.Division),
			new KeyValuePair<string, TokenType>("%", TokenType.Modulus),
			new KeyValuePair<string, TokenType>(",", TokenType.Comma),
		};

		public Tokenizer(string expression)
		{
			Expression = expression;
			Parse();
		}

		private void Parse()
		{
			while (Index < Expression.Length && !HasError)
			{
				char c = Expression[Index];

				if (c == ' ') // whitespace
				{
					Index++;
					continue;
				}

				if (c == '"')
				{
					ExtractString(c);
					continue;
				}

				if (c == '\'')
				{
					ExtractString(c);
					continue;
				}

				if (c == '#')
				{
					ExtractDate();
					continue;
				}

				if (c >= '0' && c <= '9')
				{
					ExtractNumber();
					continue;
				}

				if (c == '{')
				{
					ExtractVariable();
					continue;
				}

				if (c == '[')
				{
					ExtractField();
					continue;
				}

				if (ExtractOperand())
					continue;

				if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == '_') || (c == '$'))
				{
					ExtractIdentifier();
					continue;
				}

				ErrorMessage = "error at " + Index.ToString() + ": syntax error";
			}

			for (int i = 0; i < Tokens.Count - 1; i++)
			{
				var token = Tokens[i];
				var nextToken = Tokens[i + 1];

				if (token.TokenType == TokenType.Identifier)
				{
					Tokens[i] = new Token((nextToken.TokenType == TokenType.OpenParens) ? TokenType.Method : TokenType.Variable, token.Value, token.Index);
				}
			}
		}

		private bool Match(string symbol, TokenType tokenType)
		{
			if (Index + symbol.Length > Expression.Length)
				return false;

			var part = Expression.Substring(Index, symbol.Length);

			if (symbol != part)
				return false;

			Index = Index + symbol.Length;

			Tokens.Add(new Token(tokenType));

			return true;
		}

		private void Extract(char termChar, TokenType tokenType)
		{
			int start = Index;
			int term = Expression.IndexOf(termChar, Index + 1);

			if (term < 0)
			{
				ErrorMessage = "error at " + Index.ToString() + ": missing closing " + termChar + " character";
				return;
			}

			var value = Expression.Substring(Index + 1, term - Index - 1);

			Tokens.Add(new Token(tokenType, value, Index + 1));

			Index = term + 1;
		}

		private void ExtractString(char c)
		{
			Extract(c, TokenType.StringLiteral);
		}

		private void ExtractDate()
		{
			Extract('#', TokenType.DateLiteral);
		}

		private void ExtractVariable()
		{
			Extract('}', TokenType.Variable);
		}

		private void ExtractField()
		{
			Extract(']', TokenType.Field);
		}

		private void ExtractNumber()
		{
			bool decimalsymbol = false;
			int start = Index;

			while (Index < Expression.Length)
			{
				char c = Expression[Index];

				if (c == '.')
				{
					if (decimalsymbol)
					{
						ErrorMessage = "error at " + Index.ToString() + ": too many decimal characters";
						return;
					}

					decimalsymbol = true;
					Index++;
					continue;
				}

				if (c >= '0' && c <= '9')
				{
					Index++;
					continue;
				}

				break;
			}

			var value = Expression.Substring(start, Index - start);

			if (decimalsymbol)
				Tokens.Add(new Token(TokenType.DecimalLiteral, value, Index));
			else
				Tokens.Add(new Token(TokenType.IntegerLiteral, value, Index));
		}

		private bool ExtractOperand()
		{
			foreach (var op in operands)
			{
				if (Match(op.Key, op.Value))
				{
					return true;
				}
			}

			return false;
		}

		private void ExtractIdentifier()
		{
			int start = Index;

			while (Index < Expression.Length)
			{
				char c = Expression[Index];

				if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || (c == '_') || (c == '.') || (c == '$'))
				{
					Index++;
					continue;
				}

				break;
			}

			var value = Expression.Substring(start, Index - start);

			// special case for true/false
			if (value == "true")
			{
				Tokens.Add(new Token(TokenType.BooleanTrueLiteral));
				return;
			}
			if (value == "false")
			{
				Tokens.Add(new Token(TokenType.BooleanFalseLiteral));
				return;
			}
			if (value.StartsWith("$"))
			{
				Tokens.Add(new Token(TokenType.Variable, value, Index));
				return;
			}

			Tokens.Add(new Token(TokenType.Identifier, value, Index));
		}

		public override string ToString()
		{
			return Expression;
		}
	}
}