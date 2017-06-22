namespace Thinkedge.Simple.ExpressionEngine
{
	public interface IVariableDestination
	{
		void SetVariable(string name, Value value);
	}
}