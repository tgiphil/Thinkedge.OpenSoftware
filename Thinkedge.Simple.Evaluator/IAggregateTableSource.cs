using System.Collections.Generic;

namespace Thinkedge.Simple.Evaluator
{
	public interface IAggregateTableSource
	{
		IEnumerable<IFieldSource> Rows { get; }
	}
}