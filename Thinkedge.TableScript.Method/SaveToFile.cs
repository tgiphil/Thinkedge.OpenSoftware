using System;
using System.IO;
using Thinkedge.Common.Result;

namespace Thinkedge.Simple.ScriptEngine.Method
{
	public class SaveToFile : BaseStandardResult
	{
		public static StandardResult<bool> Execute(string filename, string data)
		{
			return new SaveToFile().ExecuteEx(filename, data);
		}

		internal StandardResult<bool> ExecuteEx(string filename, string data)
		{
			try
			{
				File.WriteAllText(filename, data);
				return ReturnResult<bool>(true);
			}
			catch (FileNotFoundException)
			{
				return ReturnError<bool>("SaveToFile() error: file not found: " + filename);
			}
			catch (Exception e)
			{
				return ReturnError<bool>("SaveToFile() error: unable to save file: " + filename, e);
			}
		}
	}
}