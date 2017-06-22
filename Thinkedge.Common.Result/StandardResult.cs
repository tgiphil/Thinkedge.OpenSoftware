using System;

namespace Thinkedge.Common.Result
{
	public class StandardResult<T> : BaseStandardResult
	{
		public T Result { get; protected set; }

		public StandardResult(T result, BaseStandardResult standard)
		{
			Result = result;
			Errors = standard.Errors;
			Exception = standard.Exception;
		}

		public StandardResult(T result)
		{
			Result = result;
		}

		protected StandardResult(string error)
		{
			AddError(error);
		}

		protected StandardResult(string error, Exception exception)
		{
			AddError(error);
			SetException(exception);
		}

		protected StandardResult(string innerError, string outerError)
		{
			AddError(innerError);
			AddError(outerError);
		}

		protected StandardResult(string innerError, string outerError, Exception exception)
		{
			AddError(innerError);
			AddError(outerError);
		}

		public static StandardResult<T> ReturnResult(T result)
		{
			return new StandardResult<T>(result);
		}

		public static StandardResult<T> ReturnError(string error)
		{
			return new StandardResult<T>(error);
		}

		public static StandardResult<T> ReturnError(string error, Exception exception)
		{
			return new StandardResult<T>(error, exception);
		}

		public static StandardResult<T> ReturnError(string innerError, string outerError)
		{
			return new StandardResult<T>(innerError, outerError);
		}

		public static StandardResult<T> ReturnError(string innerError, string outerError, Exception exception)
		{
			return new StandardResult<T>(innerError, outerError, exception);
		}
	}
}