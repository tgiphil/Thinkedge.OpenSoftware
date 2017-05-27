using System.Collections.Generic;

namespace Thinkedge.Simple.Evaluator
{
	public interface IAggregateMethodSource
	{
		Value Evaluate(string name, Context context, ExpressionNode node, Evaluation eval);
	}
}