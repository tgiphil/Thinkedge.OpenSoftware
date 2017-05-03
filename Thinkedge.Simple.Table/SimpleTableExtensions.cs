namespace Thinkedge.Simple.Table
{
	public static class SimpleTableExtensions
	{
		private static SimpleTable Copy(this SimpleTable table)
		{
			var copy = new SimpleTable();

			// copy of column names
			for (int i = 0; i < table.ColumnNames.Count; i++)
			{
				var name = table.GetColumnName(i);

				if (name == null)
					continue;

				copy.SetColumnName(i, name);
			}

			// copy row
			foreach (var row in table)
			{
				var newRow = copy.CreateRow();

				for (int i = 0; i < row.ColumnCount; i++)
				{
					var value = row[i];

					if (value == null)
						continue;

					newRow[i] = value;
				}
			}

			return copy;
		}
	}
}