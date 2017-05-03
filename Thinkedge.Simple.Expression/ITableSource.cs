namespace Thinkedge.Simple.Expression
{
	public interface ITableSource
	{
		Value GetField(string name);
	}
}