using System;
using System.IO;
using Thinkedge.Common.Result;

namespace Thinkedge.TableScript.Method
{
	public class LoadFromFile : BaseStandardResult
	{
		public static StandardResult<string> Execute(string filename)
		{
			return new LoadFromFile().ExecuteEx(filename);
		}

		internal StandardResult<string> ExecuteEx(string filename)
		{
			try
			{
				var s = File.ReadAllText(filename);
				return ReturnResult<string>(s);
			}
			catch (FileNotFoundException)
			{
				return ReturnError<string>("LoadFromFile() error: file not found: " + filename);
			}
			catch (Exception e)
			{
				return ReturnError<string>("LoadFromFile() error: unable to read file: " + filename, e);
			}
		}
	}
}