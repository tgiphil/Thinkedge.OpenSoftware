using System.IO;
using System.Text;

namespace Thinkedge.Simple.Table
{
	public static class SimpleTableHelpers
	{
		public static SimpleTable LoadDelimitedFile(string filename, bool containsHeader = true, bool mapHeaderNames = true, char delimiter = '\t', bool unespace = true)
		{
			var lines = File.ReadAllLines(filename);
			var delimiterList = new char[] { delimiter };
			var start = 0;
			var table = new SimpleTable();

			if (containsHeader)
			{
				start = 1;
				if (mapHeaderNames)
				{
					var parts = lines[0].Split(delimiterList);

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
				var parts = lines[index].Split(delimiterList);

				var row = table.CreateRow();

				for (int i = 0; i < parts.Length; i++)
				{
					var part = parts[i];

					row.SetField(i, part);
				}
			}

			return table;
		}

		public static void SaveTableAsTabDelimited(SimpleTable table, string filename)
		{
			var sb = new StringBuilder();

			foreach (var column in table.ColumnNames)
			{
				sb.Append(column);
				sb.Append('\t');
			}

			sb.Length = sb.Length - 1;
			sb.AppendLine();

			foreach (var row in table)
			{
				for (int i = 0; i < row.ColumnCount; i++)
				{
					var value = row[i];
					sb.Append(value);
					sb.Append('\t');
				}

				sb.Length = sb.Length - 1;
				sb.AppendLine();
			}

			File.WriteAllText(filename, sb.ToString());
		}
	}
}