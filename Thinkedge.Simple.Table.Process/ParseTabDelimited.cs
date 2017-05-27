using System;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Process
{
	public static class ParseTabDelimited
	{
		public static StandardResult<SimpleTable> Execute(string data, bool containsHeader = true, bool mapHeaderNames = true, bool unespace = true)
		{
			var lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);

			var start = 0;
			var table = new SimpleTable();

			if (containsHeader)
			{
				start = 1;
				if (mapHeaderNames)
				{
					var parts = lines[0].Split('\t');

					for (int i = 0; i < parts.Length; i++)
					{
						var value = parts[i];

						if (string.IsNullOrWhiteSpace(value))
							continue;

						table.SetColumnName(i, value);
					}
				}
			}

			for (int index = start; index < lines.Length; index++)
			{
				var line = lines[index];

				if (index + 1 == lines.Length && string.IsNullOrWhiteSpace(line))
					continue;

				var parts = line.Split('\t');

				var row = table.CreateRow();

				for (int i = 0; i < parts.Length; i++)
				{
					var part = parts[i];

					if (unespace)
						part = Unescape(part);

					row.SetField(i, part);
				}
			}

			return StandardResult<SimpleTable>.ReturnResult(table);
		}

		public static string Unescape(string s)
		{
			string u = s.Replace("\\s", "\\").Replace("\\t", "\t").Replace("\\n", "\r\n");
			return u;
		}
	}
}