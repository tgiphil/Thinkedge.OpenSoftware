using System.Collections.Generic;

namespace Thinkedge.Simple.ExpressionEngine
{
	public interface IMethodSource
	{
		Value Evaluate(string name, IList<Value> parameters, Context context);
	}
}