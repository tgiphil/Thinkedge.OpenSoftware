namespace Thinkedge.Simple.Expression
{
	public class Context
	{
		public IVariableSource VariableSource { get; set; } = null;
		public IFieldSource FieldSource { get; set; } = null;
		public IMethodSource MethodSource { get; set; } = null;

		//public ISimpleTable GroupTableSource { get; set; } = null;
	}
}