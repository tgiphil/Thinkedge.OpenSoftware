using System;
using System.Collections.Generic;

namespace Thinkedge.Simple.Table
{
	public class SimpleTable
	{
		public string Name { get; set; }
		public List<string> ColumnNames = new List<string>();

		protected Dictionary<string, int> NameToFieldIndex = new Dictionary<string, int>();
		protected List<SimpleTableRow> Rows = new List<SimpleTableRow>();

		public int RowCount { get { return Rows.Count; } }

		public SimpleTableRow CreateRow()
		{
			var row = new SimpleTableRow(this, RowCount, ColumnNames.Count);
			Rows.Add(row);
			return row;
		}

		public SimpleTableRow this[int index] { get { return Rows[index]; } }

		public IEnumerator<SimpleTableRow> GetEnumerator()
		{
			for (int i = 0; i < Rows.Count; i++)
			{
				yield return Rows[i];
			}
		}

		//public SimpleTable Copy()
		//{
		//	throw new NotImplementedException();
		//}

		public bool ContainColumn(string name)
		{
			return NameToFieldIndex.ContainsKey(name);
		}

		public int GetColumnIndex(string name)
		{
			if (!ContainColumn(name))
				return -1;

			int index = NameToFieldIndex[name];

			return index;
		}

		public string GetColumnName(int index)
		{
			if (index > ColumnNames.Count)
				return null;

			return ColumnNames[index];
		}

		public void AddColumnName(string name)
		{
			SetColumnName(ColumnNames.Count, name);
		}

		public void SetColumnName(int index, string name)
		{
			while (ColumnNames.Count <= index)
			{
				ColumnNames.Add(null);
			}

			ColumnNames[index] = name;
			NameToFieldIndex[name] = index;
		}

		public void RenameColumnName(string oldName, string newName)
		{
			if (oldName == newName)
				return;

			int index = NameToFieldIndex[oldName];
			NameToFieldIndex.Remove(oldName);

			SetColumnName(index, newName);
		}

		public override string ToString()
		{
			return Name ?? "Table";
		}

		//TODO: Delete
		//TODO: Insert
	}
}