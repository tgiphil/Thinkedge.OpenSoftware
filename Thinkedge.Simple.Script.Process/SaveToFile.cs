using System;
using System.IO;
using Thinkedge.Common;

namespace Thinkedge.Simple.Script.Process
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
				return ReturnError<bool>("file not found: " + filename);
			}
			catch (Exception e)
			{
				return ReturnError<bool>("unable to read file: " + filename, e);
			}
		}
	}
}