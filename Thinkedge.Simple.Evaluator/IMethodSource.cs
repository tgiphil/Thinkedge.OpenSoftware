using System.Collections.Generic;

namespace Thinkedge.Simple.Evaluator
{
	public interface IMethodSource
	{
		Value Evaluate(string name, IList<Value> parameters, Context context);
	}
}