using System.Collections.Generic;

namespace Thinkedge.Simple.Expression
{
	public interface IMethodSource
	{
		Value Evaluate(string name, IList<Value> parameters);
	}
}