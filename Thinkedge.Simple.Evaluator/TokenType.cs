namespace Thinkedge.Simple.Evaluator
{
	public enum TokenType
	{
		Unknown,

		// syntax
		OpenParens,

		CloseParens,
		Comma,

		// boolean logic
		And,

		Or,
		Not,

		// if-then-else
		If,

		Then,
		Else,

		Questionmark,
		Colon,

		// math
		Addition,

		Subtract,
		Multiplication,
		Division,
		Modulus,
		Negate,

		// literals
		IntegerLiteral,

		DecimalLiteral,
		StringLiteral,
		DateLiteral,
		BooleanTrueLiteral,
		BooleanFalseLiteral,
		FloatLiteral,

		// comparisons
		CompareEqual,

		CompareNotEqual,
		CompareGreaterThanOrEqual,
		CompareLessThanOrEqual,
		CompareLessThan,
		CompareGreaterThan,

		// assignment
		Equal,

		// others
		Field,

		Variable,
		Method,
		Identifier  // temporary
	}
}