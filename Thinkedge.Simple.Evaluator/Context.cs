namespace Thinkedge.Simple.Evaluator
{
	public class Context
	{
		public IVariableSource VariableSource { get; set; } = null;
		public IFieldSource FieldSource { get; set; } = null;
		public IMethodSource MethodSource { get; set; } = null;
		//public IAggregateMethodSource AggregateMethodSource { get; set; } = null;

		public IAggregateTableSource AggregateTableSource { get; set; } = null;
	}
}