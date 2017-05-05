using System;
using System.Collections.Generic;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Process
{
	public class ParseCustomINI : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(string data, string newRow, char delimiter = '=', IList<string> columns = null)
		{
			return new ParseCustomINI().ExecuteEx(data, newRow, delimiter, columns);
		}

		internal StandardResult<SimpleTable> ExecuteEx(string data, string newRow, char delimiter = '=', IList<string> columns = null)
		{
			var lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			var table = new SimpleTable();

			if (columns != null)
			{
				for (int i = 0; i < columns.Count; i++)
				{
					table.SetColumnName(i, columns[i]);
				}
			}

			SimpleTableRow row = null;

			foreach (var line in lines)
			{
				if (string.IsNullOrEmpty(line))
					continue;

				if (line.StartsWith("#"))
					continue;

				if (line == newRow)
				{
					row = table.CreateRow();

					continue;
				}

				int pos = line.IndexOf(delimiter);

				if (pos < 0)
					continue;

				string name = line.Substring(0, pos);
				string value = line.Substring(pos + 1);

				if (columns == null && !table.ContainColumn(name))
				{
					table.SetColumnName(table.ColumnNames.Count, name);
				}

				if (table.ContainColumn(name))
				{
					row.SetField(name, value);
				}
			}

			return ReturnResult<SimpleTable>(table);
		}

		public static object EXecute(string data, string newRow, char delimiter)
		{
			throw new NotImplementedException();
		}
	}
}