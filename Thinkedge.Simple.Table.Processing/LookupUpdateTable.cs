using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Processing
{
	internal class LookupUpdateTable : BaseStandardResult
	{
		internal SimpleTable Execute(SimpleTable sourceTable, string sourceKeyField, string mergeField, SimpleTable lookupTable, string lookupKeyField, string lookupDataField, bool overwrite, bool caseInsensitive = true)
		{
			ClearError();

			if (!sourceTable.ColumnNames.Contains(sourceKeyField))
			{
				SetError("merge error - source key field does not exists: " + sourceKeyField);
				return null;
			}

			if (!sourceTable.ColumnNames.Contains(mergeField))
			{
				SetError("merge error - merge field does not exists: " + mergeField);
				return null;
			}

			var newTable = new SimpleTable();
			var cache = new EvaluatorCache();

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

			return newTable;
		}

		public static string Lookup(SimpleTable source, string key, string keyField, string dataField, bool caseInsensitive = true)
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