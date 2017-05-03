namespace Thinkedge.Simple.Expression
{
	public class Token
	{
		public TokenType TokenType { get; set; } = TokenType.Unknown;
		public string Value { get; set; } = null;
		public int Index;

		public Token(TokenType tokenType, string value, int index = -1)
		{
			TokenType = tokenType;
			Value = value;
			Index = index;
		}

		public Token(TokenType tokenType) : this(tokenType, null)
		{
		}

		public override string ToString()
		{
			return TokenType.ToString() + (Value != null ? " = " + Value : string.Empty);
		}
	}
}