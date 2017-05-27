using System;
using Xunit;

namespace Thinkedge.Simple.Evaluator.Tests
{
	public class EvaluatorTests
	{
		[Fact]
		public void EvaluatorTest1()
		{
			int value = 1 + 2 + 10 * 8 + 10 * 20 + 7;

			var tokenizer = new Tokenizer("1 + 2 + 10 * 8 + 10 * 20 + 7");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Integer, value);
		}

		[Fact]
		public void EvaluatorTest2()
		{
			int value = 1 + 2 + (10 * 8) + 10 * 20 + 7;

			var tokenizer = new Tokenizer("1 + 2 + (10 * 8) + 10 * 20 + 7");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Integer, value);
		}

		[Fact]
		public void EvaluatorTest3()
		{
			int value = 1 + (2 + 10) * 8 + 10 * 20 + 7;

			var tokenizer = new Tokenizer("1 + (2 + 10) * 8 + 10 * 20 + 7");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Integer, value);
		}

		[Fact]
		public void EvaluatorTest4()
		{
			bool value = 1 == 2;

			var tokenizer = new Tokenizer("1 == 2");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest5()
		{
			bool value = true == true;

			var tokenizer = new Tokenizer("true == true");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest6()
		{
			bool value = false == false;

			var tokenizer = new Tokenizer("false == false");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest7()
		{
			bool value = true && true;

			var tokenizer = new Tokenizer("true && true");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest8()
		{
			bool value = false && true;

			var tokenizer = new Tokenizer("false && true");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest9()
		{
			DateTime value = new DateTime(2017, 1, 1);

			var tokenizer = new Tokenizer("#1/1/2017#");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Date, value);
		}

		[Fact]
		public void EvaluatorTest10()
		{
			DateTime value = new DateTime(2017, 1, 2);

			var tokenizer = new Tokenizer("#1/1/2017# + 1");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Date, value);
		}

		[Fact]
		public void EvaluatorTest11()
		{
			bool value = 1 > 2;

			var tokenizer = new Tokenizer("1 > 2");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest12()
		{
			bool value = "Test" == "Test";

			var tokenizer = new Tokenizer("\"Test\" == \"Test\"");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest13()
		{
			bool value = "Test1" == "Test2";

			var tokenizer = new Tokenizer("\"Test1\" == \"Test2\"");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest14()
		{
			bool value = !(1 > 2);

			var tokenizer = new Tokenizer("!(1 > 2)");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest15()
		{
			bool value = !(1 > 2) && ((1 < 6) || !(1 > 3));

			var tokenizer = new Tokenizer(" !(1 > 2) && ((1 < 6) || !(1 > 3))");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, value);
		}

		[Fact]
		public void EvaluatorTest16()
		{
			var tokenizer = new Tokenizer("if (true, 1, 2)==1");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Boolean, true);
		}

		[Fact]
		public void EvaluatorTest17()
		{
			var tokenizer = new Tokenizer("if (false, 1, 2)");
			var parser = new Parser(tokenizer);
			var expression = new Expression(parser);

			var result = expression.Evaluate();

			Assert.Equal(result.Integer, 2);
		}

		//[Fact]
		//public void EvaluatorTest18()
		//{
		//	var tokenizer = new Tokenizer("food='test'");
		//	var parser = new Parser(tokenizer);
		//	var evaluator = new Evaluator(parser);

		//	var result = expression.Evaluate();

		//	Assert.Equal(result.String, "test");
		//}
	}
}