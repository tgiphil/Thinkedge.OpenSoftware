using Xunit;

namespace Thinkedge.Simple.Evaluator.Tests
{
	public class ExpressionTests
	{
		[Fact]
		public void TokenizerTest1()
		{
			var tokenizer = new Tokenizer("(test==\"Test\") && (IsDate(date) || Apple(true,false))");

			Assert.Equal(tokenizer.Tokens[0].TokenType, TokenType.OpenParens);
			Assert.Equal(tokenizer.Tokens[1].TokenType, TokenType.Variable);
			Assert.Equal(tokenizer.Tokens[2].TokenType, TokenType.CompareEqual);
			Assert.Equal(tokenizer.Tokens[3].TokenType, TokenType.StringLiteral);
			Assert.Equal(tokenizer.Tokens[4].TokenType, TokenType.CloseParens);
			Assert.Equal(tokenizer.Tokens[5].TokenType, TokenType.And);
			Assert.Equal(tokenizer.Tokens[6].TokenType, TokenType.OpenParens);
			Assert.Equal(tokenizer.Tokens[7].TokenType, TokenType.Method);
			Assert.Equal(tokenizer.Tokens[8].TokenType, TokenType.OpenParens);
			Assert.Equal(tokenizer.Tokens[9].TokenType, TokenType.Variable);
			Assert.Equal(tokenizer.Tokens[10].TokenType, TokenType.CloseParens);
			Assert.Equal(tokenizer.Tokens[11].TokenType, TokenType.Or);
			Assert.Equal(tokenizer.Tokens[12].TokenType, TokenType.Method);
			Assert.Equal(tokenizer.Tokens[13].TokenType, TokenType.OpenParens);
			Assert.Equal(tokenizer.Tokens[14].TokenType, TokenType.BooleanTrueLiteral);
			Assert.Equal(tokenizer.Tokens[15].TokenType, TokenType.Comma);
			Assert.Equal(tokenizer.Tokens[16].TokenType, TokenType.BooleanFalseLiteral);
			Assert.Equal(tokenizer.Tokens[17].TokenType, TokenType.CloseParens);
			Assert.Equal(tokenizer.Tokens[18].TokenType, TokenType.CloseParens);
		}
	}
}