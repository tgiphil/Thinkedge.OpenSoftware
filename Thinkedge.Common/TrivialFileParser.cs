using System;
using System.Collections.Generic;
using System.IO;

namespace Thinkedge.Common
{
	public class TrivialFileParser
	{
		private Dictionary<string, int> headerMap = new Dictionary<string, int>();
		private string[] lines;
		private string[] parts;
		private bool containsHeader;

		private int row = 0;
		private char delimiter = '\t';

		public TrivialFileParser(string filename, bool containsHeader = true, char delimiter = '\t', bool mapHeaderNames = true)
		{
			this.delimiter = delimiter;
			this.containsHeader = containsHeader;

			lines = File.ReadAllLines(filename);

			if (containsHeader)
			{
				row++;

				if (mapHeaderNames)
				{
					var parts = lines[0].Split(new char[] { delimiter });

					for (int i = 0; i < parts.Length; i++)
					{
						headerMap.Add(parts[i], i);
					}
				}
			}
		}

		public bool Next()
		{
			row++;

			if (IsEOF)
				return false;

			parts = lines[row].Split(new char[] { delimiter });

			return true;
		}

		public bool IsEOF { get { return row > lines.Length; } }

		public bool AddAliasedField(string alias, string field)
		{
			int index = -1;

			if (headerMap.TryGetValue(field, out index))
			{
				headerMap.Add(alias, index);
				return true;
			}
			return false;
		}

		public string GetField(int index)
		{
			return parts[index];
		}

		private string GetField(string field)
		{
			int index = -1;

			if (headerMap.TryGetValue(field, out index))
			{
				if (index > parts.Length)
					return null;

				return GetField(index);
			}

			return null;
		}

		public string GetString(string field)
		{
			return GetField(field);
		}

		public DateTime? GetDate(string field)
		{
			return ConvertDate(GetField(field));
		}

		public decimal GetDecimal(string field)
		{
			return Convert.ToDecimal(GetField(field));
		}

		public int? GetInteger(string field)
		{
			return ConvertInteger(GetField(field));
		}

		static private DateTime? ConvertDate(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			if (!IsDate(value))
				return null;

			return Convert.ToDateTime(value);
		}

		static private int? ConvertInteger(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			return Convert.ToInt32(value);
		}

		static private bool IsDate(string value)
		{
			return DateTime.TryParse(value, out DateTime parse);
		}
	}
}