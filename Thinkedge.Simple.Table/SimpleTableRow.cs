using System.Collections.Generic;

namespace Thinkedge.Simple.Table
{
	public class SimpleTableRow
	{
		public SimpleTable Table { get; private set; }
		internal int RowNbr;

		protected readonly List<string> Columns;

		public int ColumnCount { get { return Columns.Count; } }

		public string this[int index] { get { return GetField(index); } set { SetField(index, value); } }
		public string this[string name] { get { return GetField(name); } set { SetField(name, value); } }

		internal SimpleTableRow(SimpleTable table, int rowNbr = 0, int initalizeColumns = 4)
		{
			Table = table;
			RowNbr = rowNbr;
			Columns = new List<string>(initalizeColumns);
		}

		public string GetField(int index)
		{
			if (index >= Columns.Count)
				return string.Empty;

			var value = Columns[index];

			return value;
		}

		public string GetField(string name)
		{
			int index = Table.GetColumnIndex(name);

			return GetField(index);
		}

		protected void Expand(int index)
		{
			while (Columns.Count <= index)
			{
				Columns.Add(string.Empty);
			}
		}

		public void SetField(string name, string value)
		{
			int index = Table.GetColumnIndex(name);

			Expand(index);

			Columns[index] = value;
		}

		public void SetField(int index, string value)
		{
			Expand(index);

			Columns[index] = value;
		}
	}
}