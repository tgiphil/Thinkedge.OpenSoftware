using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Process
{
	public static class LookupUpdateTable
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, string sourceKeyField, string mergeField, SimpleTable lookupTable, string lookupKeyField, string lookupDataField, bool overwrite, bool caseInsensitive = true)
		{
			if (!sourceTable.ColumnNames.Contains(sourceKeyField))
				return StandardResult<SimpleTable>.ReturnError("LookupUpdateTable() error: source key field does not exists: " + sourceKeyField);

			if (!sourceTable.ColumnNames.Contains(mergeField))
				return StandardResult<SimpleTable>.ReturnError("LookupUpdateTable() error: merge field does not exists: " + mergeField);

			var newTable = new SimpleTable();

			foreach (var column in sourceTable.ColumnNames)
			{
				newTable.AddColumnName(column);
			}

			foreach (var sourceRow in sourceTable)
			{
				var newRow = newTable.CreateRow();

				for (int i = 0; i < sourceRow.ColumnCount; i++)
				{
					var value = sourceRow[i];

					if (value == null)
						continue;

					newRow[i] = value;
				}
			}

			foreach (var row in newTable)
			{
				string key = row[sourceKeyField];

				if (!overwrite && !string.IsNullOrWhiteSpace(row[mergeField]))
					continue;

				if (caseInsensitive)
					key = key.ToLower();

				string lookup = Lookup(lookupTable, key, lookupKeyField, lookupDataField);

				row[mergeField] = lookup;
			}

			return StandardResult<SimpleTable>.ReturnResult(newTable);
		}

		private static string Lookup(SimpleTable source, string key, string keyField, string dataField, bool caseInsensitive = true)
		{
			if (string.IsNullOrEmpty(key))
				return string.Empty;

			foreach (var row in source)
			{
				var keyvalue = row[keyField];

				if (caseInsensitive)
					keyvalue = keyvalue.ToLower();

				if (keyvalue == key)
					return row[dataField];
			}

			return string.Empty;
		}
	}
}