namespace Thinkedge.Simple.Evaluator
{
	public interface IVariableSource
	{
		Value GetVariable(string name);
	}
}