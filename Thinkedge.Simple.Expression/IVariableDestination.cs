namespace Thinkedge.Simple.Expression
{
	public interface IVariableDestination
	{
		void SetVariable(string name, Value value);
	}
}