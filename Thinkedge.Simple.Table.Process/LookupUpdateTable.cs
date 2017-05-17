using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Process
{
	public class LookupUpdateTable : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, string sourceKeyField, string mergeField, SimpleTable lookupTable, string lookupKeyField, string lookupDataField, bool overwrite, bool caseInsensitive = true)
		{
			return new LookupUpdateTable().ExecuteEx(sourceTable, sourceKeyField, mergeField, lookupTable, lookupKeyField, lookupDataField, overwrite, caseInsensitive);
		}

		internal StandardResult<SimpleTable> ExecuteEx(SimpleTable sourceTable, string sourceKeyField, string mergeField, SimpleTable lookupTable, string lookupKeyField, string lookupDataField, bool overwrite, bool caseInsensitive = true)
		{
			if (!sourceTable.ColumnNames.Contains(sourceKeyField))
				return ReturnError<SimpleTable>("LookupUpdateTable() error: source key field does not exists: " + sourceKeyField);

			if (!sourceTable.ColumnNames.Contains(mergeField))
				return ReturnError<SimpleTable>("LookupUpdateTable() error: merge field does not exists: " + mergeField);

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

			return ReturnResult<SimpleTable>(newTable);
		}

		protected static string Lookup(SimpleTable source, string key, string keyField, string dataField, bool caseInsensitive = true)
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