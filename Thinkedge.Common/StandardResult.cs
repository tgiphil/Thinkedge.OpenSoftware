using System;

namespace Thinkedge.Common
{
	public class StandardResult<T> : BaseStandardResult
	{
		public T Result;

		public StandardResult(T result, string errorMessage, Exception exception = null)
		{
			SetError(errorMessage, exception);
			Result = result;
		}

		public StandardResult(T result, BaseStandardResult standard) : this(result, standard.ErrorMessage, standard.Exception)
		{
		}

		public StandardResult(string errorMessage, Exception exception = null)
		{
			SetError(errorMessage, exception);
		}
	}
}