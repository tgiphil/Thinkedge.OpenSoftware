using System;

namespace Thinkedge.Common
{
	public class BaseStandardResult
	{
		public string ErrorMessage { get; protected set; }
		public bool HasError { get { return ErrorMessage != null; } }
		public Exception Exception { get; protected set; } = null;

		//protected StandardResult<T> ReturnResult<T>(T result)
		//{
		//	var ret = new StandardResult<T>(result, ErrorMessage);

		//	return ret;
		//}

		protected void ClearError()
		{
			ErrorMessage = null;
			Exception = null;
		}

		protected void SetError(string message, Exception exception = null)
		{
			ErrorMessage = message;
			Exception = exception;
		}

		protected void SetError(Exception exception)
		{
			ErrorMessage = "An exception has occurred. See excepton for details.";
			Exception = exception;
		}
	}
}