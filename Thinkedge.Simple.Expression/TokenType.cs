﻿namespace Thinkedge.Simple.Expression
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

		// others
		Field,

		Variable,
		Method,
		Identifier  // temporary
	}
}