using System.Text;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Process
{
	public class FormatToTabDelimited : BaseStandardResult
	{
		public static StandardResult<string> Execute(SimpleTable table, bool escape = true)
		{
			return new FormatToTabDelimited().ExecuteEx(table, escape);
		}

		internal StandardResult<string> ExecuteEx(SimpleTable table, bool escape = true)
		{
			//ClearError();

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

					if (escape)
						value = Escape(value);

					sb.Append(value);
					sb.Append('\t');
				}

				sb.Length = sb.Length - 1;
				sb.AppendLine();
			}

			return ReturnResult<string>(sb.ToString());
		}

		internal static string Escape(string s)
		{
			string e = s.Replace("\\", "\\s").Replace("\t", "\\t").Replace("\r\n", "\\n");
			return e;
		}
	}
}