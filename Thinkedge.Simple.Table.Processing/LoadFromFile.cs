using System;
using System.IO;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Processing
{
	internal class LoadFromFile : BaseStandardResult
	{
		internal string Execute(string filename)
		{
			ClearError();

			try
			{
				var s = File.ReadAllText(filename);
				return s;
			}
			catch (FileNotFoundException)
			{
				SetError("file not found: " + filename);
			}
			catch (Exception e)
			{
				SetError("unable to read file: " + filename, e);
			}

			return null;
		}
	}
}