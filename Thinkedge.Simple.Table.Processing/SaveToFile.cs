using System;
using System.IO;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Processing
{
	internal class SaveToFile : BaseStandardResult
	{
		internal void Execute(string filename, string data)
		{
			ClearError();

			try
			{
				File.WriteAllText(filename, data);
			}
			catch (Exception e)
			{
				SetError("unable to write file: " + filename, e);
			}
		}
	}
}