namespace Thinkedge.Simple.Evaluator
{
	public interface IVariableDestination
	{
		void SetVariable(string name, Value value);
	}
}