using System;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Processing
{
	internal class ParseValuePairs : BaseStandardResult
	{
		internal SimpleTable Execute(string data, string destination = "name", string source = "value", char delimiter = '=')
		{
			var lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);

			var table = new SimpleTable();

			table.SetColumnName(0, source);
			table.SetColumnName(1, destination);

			foreach (var line in lines)
			{
				if (string.IsNullOrEmpty(line))
					continue;

				if (line.StartsWith("#"))
					continue;

				int pos = line.IndexOf(delimiter);

				if (pos < 0)
					continue;

				string dst = line.Substring(0, pos);
				string src = line.Substring(pos + 1);

				var row = table.CreateRow();

				row.SetField(0, dst);
				row.SetField(1, src);
			}

			return table;
		}
	}
}