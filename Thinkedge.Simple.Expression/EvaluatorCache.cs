using System.Collections.Generic;

namespace Thinkedge.Simple.Expression
{
	public class EvaluatorCache
	{
		public Dictionary<string, Evaluator> cache = new Dictionary<string, Evaluator>();

		public Evaluator Compile(string expression)
		{
			if (!cache.TryGetValue(expression, out Evaluator evaluator))
			{
				var tokenizer = new Tokenizer(expression);

				var parser = new Parser(tokenizer);

				evaluator = new Evaluator(parser);

				cache.Add(expression, evaluator);
			}

			return evaluator;
		}
	}
}