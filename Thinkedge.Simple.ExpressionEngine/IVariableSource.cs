namespace Thinkedge.Simple.ExpressionEngine
{
	public interface IVariableSource
	{
		Value GetVariable(string name);
	}
}