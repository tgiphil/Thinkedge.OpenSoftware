using System;
using System.Collections.Generic;
using System.Text;

namespace Thinkedge.Common
{
	public class BaseStandardResult
	{
		public List<string> Errors { get; protected set; } = new List<string>();
		public bool HasError { get { return Errors.Count != 0; } }
		public bool IsValid { get { return Errors.Count == 0; } }

		public string FirstErrorMessage { get { return IsValid ? null : Errors[0]; } }
		public string LastErrorMessage { get { return IsValid ? null : Errors[Errors.Count - 1]; } }

		public string ErrorMessage { get { return GetAllErrorMessage(); } set { AddError(value); } }
		public Exception Exception { get; protected set; } = null;

		protected void ClearError()
		{
			Errors.Clear();
			Exception = null;
		}

		protected void AddError(string message)
		{
			Errors.Add(message);
		}

		protected void SetException(Exception exception)
		{
			if (exception != null)
				Exception = exception;
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

		protected StandardResult<T> ReturnResult<T>(T result)
		{
			return new StandardResult<T>(result, this);
		}

		protected StandardResult<T> ReturnError<T>(string error, Exception exception = null)
		{
			SetError(error, exception);
			return new StandardResult<T>(default(T), this);
		}

		protected StandardResult<T> ReturnError<T>(string errorOuter, string errorInner, Exception exception = null)
		{
			AddError(errorOuter);
			AddError(errorInner);
			SetException(exception);
			return new StandardResult<T>(default(T), this);
		}

		protected StandardResult<T> ReturnError<T>()
		{
			if (!HasError)
				ErrorMessage = "An exception has occurred";

			return new StandardResult<T>(default(T), this);
		}

		protected StandardResult<T> ReturnError<T>(Exception exception = null)
		{
			ErrorMessage = "An exception has occurred. See exception for details.";
			Exception = exception;
			return new StandardResult<T>(default(T), this);
		}

		protected string GetAllErrorMessage()
		{
			if (Errors.Count == 0) return null;
			else if (Errors.Count == 1) return Errors[0];

			var sb = new StringBuilder();

			foreach (string error in Errors)
			{
				sb.AppendLine(error);
			}

			sb.Length--;

			return sb.ToString();
		}
	}
}