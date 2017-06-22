namespace Thinkedge.Simple.ExpressionEngine
{
	public interface IAggregateMethodSource
	{
		Value Evaluate(string name, Context context, ExpressionNode node, Evaluation eval);
	}
}