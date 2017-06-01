using System.Collections.Generic;

namespace Thinkedge.Simple.Evaluator
{
	public interface IAggregateFieldSource
	{
		IEnumerable<IFieldSource> Rows { get; }
	}
}