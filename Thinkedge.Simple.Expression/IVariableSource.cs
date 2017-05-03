namespace Thinkedge.Simple.Expression
{
	public interface IVariableSource
	{
		Value GetVariable(string name);
	}
}