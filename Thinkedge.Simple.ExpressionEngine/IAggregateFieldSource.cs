using System.Collections.Generic;

namespace Thinkedge.Simple.ExpressionEngine
{
	public interface IAggregateFieldSource
	{
		IEnumerable<IFieldSource> Rows { get; }
	}
}