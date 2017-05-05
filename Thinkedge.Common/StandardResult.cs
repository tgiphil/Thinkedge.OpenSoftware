namespace Thinkedge.Common
{
	public class StandardResult<T> : BaseStandardResult
	{
		public T Result { get; protected set; }

		public StandardResult(T result, BaseStandardResult standard)
		{
			Errors = standard.Errors;
			Exception = standard.Exception;
		}
	}
}