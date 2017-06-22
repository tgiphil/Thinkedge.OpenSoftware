using System.Collections.Generic;
using Thinkedge.Simple.ExpressionEngine;
using Thinkedge.Simple.Table;
using Thinkedge.Simple.Table.Method;

namespace Thinkedge.TableScript.Method
{
	public class TableMethods : IMethodSource
	{
		Value IMethodSource.Evaluate(string name, IList<Value> parameters, Context context)
		{
			switch (name)
			{
				//short versions
				case "ParseTabDelimited": return ParseTabDelimited(parameters);
				case "ParseValuePairs": return ParseValuePairs(parameters);
				case "ParseCustomINI": return ParseCustomINI(parameters);
				case "Transform": return Transform(parameters);
				case "Expand": return Expand(parameters);
				case "Filter": return Filter(parameters);
				case "Aggregate": return Aggregate(parameters);
				case "Validate": return Validate(parameters);
				case "LookupUpdate": return LookupUpdate(parameters);
				case "FormatToTabDelimited": return FormatToTabDelimited(parameters);
				//long versions
				case "Table.ParseTabDelimited": return ParseTabDelimited(parameters);
				case "Table.ParseValuePairs": return ParseValuePairs(parameters);
				case "Table.ParseCustomINI": return ParseCustomINI(parameters);
				case "Table.Transform": return Transform(parameters);
				case "Table.Expand": return Expand(parameters);
				case "Table.Filter": return Filter(parameters);
				case "Table.Aggregate": return Aggregate(parameters);
				case "Table.Validate": return Validate(parameters);
				case "Table.LookupUpdate": return LookupUpdate(parameters);
				case "Table.FormatToTabDelimited": return FormatToTabDelimited(parameters);

				default: break;
			}

			return null;
		}

		public static Value ParseTabDelimited(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("ParseTabDelimited", parameters, 1, new List<ValueType>() { ValueType.String, ValueType.Boolean, ValueType.Boolean });

			if (validate != null)
				return validate;

			string data = parameters[0].String;
			bool containsHeader = (parameters.Count >= 2) ? true : parameters[1].Boolean;
			bool mapHeaderNames = (parameters.Count >= 3) ? true : parameters[2].Boolean;

			try
			{
				var result = Simple.Table.Method.ParseTabDelimited.Execute(data, containsHeader, mapHeaderNames);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("ParseTabDelimited() unable to parse data", e);
			}
		}

		public static Value ParseValuePairs(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("ParseValuePairs", parameters, 1, new List<ValueType>() { ValueType.String, ValueType.String, ValueType.String, ValueType.String });

			if (validate != null)
				return validate;

			string data = parameters[0].String;
			string destination = (parameters.Count >= 2) ? "source" : parameters[1].String;
			string source = (parameters.Count >= 3) ? "destination" : parameters[2].String;
			char delimiter = (parameters.Count >= 4) ? '=' : parameters[3].String[0];

			try
			{
				var result = Simple.Table.Method.ParseValuePairs.Execute(data, destination, source, delimiter);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("ParseValuePairs() unable to parse data", e);
			}
		}

		public static Value ParseCustomINI(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("ParseCustomINI", parameters, 1, new List<ValueType>() { ValueType.String, ValueType.String, ValueType.String });

			if (validate != null)
				return validate;

			string data = parameters[0].String;
			string newRow = (parameters.Count >= 2) ? parameters[1].String : null;
			char delimiter = (parameters.Count >= 3) ? parameters[2].String[0] : '=';

			try
			{
				var result = Simple.Table.Method.ParseCustomINI.Execute(data, newRow, delimiter);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("ParseCustomINI() unable to parse data", e);
			}
		}

		public static Value Transform(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("Transform", parameters, 2, new List<ValueType>() { ValueType.Object, ValueType.Object });

			if (validate != null)
				return validate;

			if (!(parameters[0].Object is SimpleTable))
				return Value.CreateErrorValue("Transform() parameter #1 is not a table");
			else if (!(parameters[1].Object is SimpleTable))
				return Value.CreateErrorValue("Transform() parameter #2 is not a table");

			var sourceTable = parameters[0].Object as SimpleTable;
			var mapTable = parameters[1].Object as SimpleTable;

			//try
			{
				var result = TransformTable.Execute(sourceTable, mapTable);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			//catch (System.Exception e)
			//{
			//	return Value.CreateErrorValue("unable to transform table", e);
			//}
		}

		public static Value Expand(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("Expand", parameters, 2, new List<ValueType>() { ValueType.Object, ValueType.Object });

			if (validate != null)
				return validate;

			if (!(parameters[0].Object is SimpleTable))
				return Value.CreateErrorValue("Expand() parameter #1 is not a table");
			else if (!(parameters[1].Object is SimpleTable))
				return Value.CreateErrorValue("Expand() parameter #2 is not a table");

			var sourceTable = parameters[0].Object as SimpleTable;
			var mapTable = parameters[1].Object as SimpleTable;

			try
			{
				var result = ExpandTable.Execute(sourceTable, mapTable);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("Expand() unable to expand table: " + e.ToString(), e);
			}
		}

		public static Value Aggregate(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("Aggregate", parameters, 2, new List<ValueType>() { ValueType.Object, ValueType.Object });

			if (validate != null)
				return validate;

			if (!(parameters[0].Object is SimpleTable))
				return Value.CreateErrorValue("Aggregate() parameter #1 is not a table");
			else if (!(parameters[1].Object is SimpleTable))
				return Value.CreateErrorValue("Aggregate() parameter #2 is not a table");

			var sourceTable = parameters[0].Object as SimpleTable;
			var mapTable = parameters[1].Object as SimpleTable;

			try
			{
				var result = AggregateTable.Execute(sourceTable, mapTable);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("Aggregate() unable to expand table: " + e.ToString(), e);
			}
		}

		public static Value Filter(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("Filter", parameters, 2, new List<ValueType>() { ValueType.Object, ValueType.String });

			if (validate != null)
				return validate;

			if (!(parameters[0].Object is SimpleTable))
			{
				return Value.CreateErrorValue("Filter() parameter #1 is not a table");
			}

			var sourceTable = parameters[0].Object as SimpleTable;
			var expression = parameters[1].String;

			try
			{
				var result = FilterTable.Execute(sourceTable, expression);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("Filter() unable to filter table", e);
			}
		}

		public static Value Validate(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("Validate", parameters, 4, new List<ValueType>() { ValueType.Object, ValueType.Object, ValueType.Boolean, ValueType.Boolean });

			if (validate != null)
				return validate;

			if (!(parameters[0].Object is SimpleTable))
				return Value.CreateErrorValue("Validate() parameter #1 is not a table");
			else if (!(parameters[1].Object is SimpleTable))
				return Value.CreateErrorValue("Validate() parameter #2 is not a table");

			var sourceTable = parameters[0].Object as SimpleTable;
			var validationRules = parameters[1].Object as SimpleTable;
			var rowPerMatch = parameters[2].Boolean;
			var overwrite = parameters[3].Boolean;

			try
			{
				var result = ValidateTable.Execute(sourceTable, validationRules, rowPerMatch, overwrite);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("Validate() unable to filter table", e);
			}
		}

		public static Value LookupUpdate(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("LookupUpdate", parameters, 7, new List<ValueType>() { ValueType.Object, ValueType.String, ValueType.String, ValueType.Object, ValueType.String, ValueType.String, ValueType.Boolean, ValueType.Boolean });

			if (validate != null)
				return validate;

			if (!(parameters[0].Object is SimpleTable))
				return Value.CreateErrorValue("LookupUpdate() parameter #1 is not a table");
			else if (!(parameters[3].Object is SimpleTable))
				return Value.CreateErrorValue("LookupUpdate() parameter #4 is not a table");

			var sourceTable = parameters[0].Object as SimpleTable;
			var sourceKeyField = parameters[1].String;
			var mergeField = parameters[2].String;
			var lookupTable = parameters[3].Object as SimpleTable;
			var lookupKeyField = parameters[4].String;
			var lookupDataField = parameters[5].String;
			var overwrite = parameters.Count >= 7 ? true : parameters[6].Boolean;
			var caseInsenstive = parameters.Count >= 8 ? true : parameters[7].Boolean;

			try
			{
				var result = LookupUpdateTable.Execute(sourceTable, sourceKeyField, mergeField, lookupTable, lookupKeyField, lookupDataField, overwrite, caseInsenstive);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("LookupUpdate() unable to perform lookup update", e);
			}
		}

		public static Value FormatToTabDelimited(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("FormatToTabDelimited", parameters, 1, new List<ValueType>() { ValueType.Object, ValueType.Boolean });

			if (validate != null)
				return validate;

			if (!parameters[0].IsObject)
				return Value.CreateErrorValue("LookupUpdate() parameter #1 is not a table");

			var table = parameters[0].Object as SimpleTable;
			var escape = parameters.Count != 2 ? false : parameters[1].Boolean;

			try
			{
				var result = Simple.Table.Method.FormatToTabDelimited.Execute(table, escape);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("LookupUpdate() unable to read file: " + parameters[0].String, e);
			}
		}
	}
}