using System.Collections.Generic;

namespace Thinkedge.Simple.ExpressionEngine
{
	public static class ExpressionCache
	{
		public static Dictionary<string, Expression> cache = new Dictionary<string, Expression>();

		public static Expression Compile(string text)
		{
			if (!cache.TryGetValue(text, out Expression expression))
			{
				var tokenizer = new Tokenizer((string)text);

				var parser = new Parser(tokenizer);

				expression = new Expression(parser);

				cache.Add(text, expression);
			}

			return expression;
		}

		public static void Clear()
		{
			cache.Clear();
		}
	}
}