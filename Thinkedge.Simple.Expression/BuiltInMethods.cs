using System;
using System.Collections.Generic;

namespace Thinkedge.Simple.Expression
{
	public class BuiltInMethods : IMethodSource
	{
		Value IMethodSource.Evaluate(string name, IList<Value> parameters)
		{
			switch (name)
			{
				//short versions
				case "ToInteger": return ToInteger(parameters);
				case "ToInt": return ToInteger(parameters);
				case "ToDate": return ToDate(parameters);
				case "ToDecimal": return ToDecimal(parameters);
				case "IsDate": return IsDate(parameters);
				case "IsInteger": return IsInteger(parameters);
				case "IsDecimal": return IsDecimal(parameters);
				case "IsEmpty": return IsEmpty(parameters);
				case "IsNull": return IsEmpty(parameters);
				case "IsNot": return IsNot(parameters);
				case "IsError": return IsError(parameters);
				case "Length": return Length(parameters);
				case "Contains": return Contains(parameters);
				case "Trim": return Trim(parameters);
				case "Today": return Today(parameters);
				case "DaysInMonth": return DaysInMonth(parameters);
				case "FirstDayOfMonth": return FirstDayOfMonth(parameters);
				case "LastDayOfMonth": return LastDayOfMonth(parameters);
				case "FirstDayOfYear": return FirstDayOfYear(parameters);
				case "LastDayOfYear": return LastDayOfYear(parameters);
				case "GetIndex": return GetIndex(parameters);
				case "GetIndexValue": return GetIndexValue(parameters);
				//long versions
				case "Index.Get": return GetIndex(parameters);
				case "Index.GetValue": return GetIndexValue(parameters);
				default: break;
			}

			return null;
		}

		public static Value ToInteger(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("ToInteger() missing parameter");

			var param = parameters[0];

			if (param.IsString)
				return new Value(Convert.ToInt32(param.String));
			else if (param.IsFloat)
				return new Value(Convert.ToInt32(param.Float));
			else if (param.IsInteger)
				return new Value(param.Integer);

			return Value.CreateErrorValue("ToInteger(): invalid parameter type");
		}

		public static Value ToDecimal(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("ToDecimal() missing parameter");

			var param = parameters[0];

			if (param.IsString)
				return new Value(Convert.ToDecimal(param.String));
			else if (param.IsFloat)
				return new Value(Convert.ToDecimal(param.Float));
			else if (param.IsInteger)
				return new Value(Convert.ToDecimal(param.IsInteger));
			else if (param.IsDecimal)
				return new Value(param.Decimal);

			return Value.CreateErrorValue("ToDecimal() contains invalid parameter type");
		}

		public static Value ToDate(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("ToDate() missing parameter");

			var param = parameters[0];

			if (param.IsString)
				return new Value(Convert.ToDateTime(param.String));
			else if (param.IsDate)
				return new Value(param.Date);

			return Value.CreateErrorValue("ToDate(): invalid parameter type");
		}

		public static Value IsDate(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("IsDate() missing parameter");

			var param = parameters[0];

			if (param.IsString)
			{
				if (string.IsNullOrWhiteSpace(param.String))
				{
					return new Value(false);
				}

				return new Value(DateTime.TryParse(param.String, out DateTime date));
			}
			else if (param.IsDate)
				return new Value(true);

			return Value.CreateErrorValue("ToDate() contains invalid parameter type");
		}

		public static Value IsDecimal(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("IsDecimal() missing parameter");

			var param = parameters[0];

			if (param.IsString)
				return new Value(Decimal.TryParse(param.String, out Decimal dec));
			else if (param.IsInteger)
				return new Value(true);
			else if (param.IsFloat)
				return new Value(true);
			else if (param.IsDecimal)
				return new Value(true);

			return Value.CreateErrorValue("IsDecimal() contains invalid parameter type");
		}

		public static Value IsInteger(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("IsInteger() missing parameter");

			var param = parameters[0];

			if (param.IsString)
				return new Value(Int32.TryParse(param.String, out int i));
			else if (param.IsInteger)
				return new Value(true);
			else if (param.IsFloat)
				return new Value(true);
			else if (param.IsDecimal)
				return new Value(true);

			return Value.CreateErrorValue("IsInteger() contains invalid parameter type");
		}

		public static Value IsError(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("IsError() missing parameter");

			var param = parameters[0];

			if (param.IsError)
				return new Value(param.IsError);

			return Value.CreateErrorValue("IsError() contains invalid parameter type");
		}

		public static Value IsEmpty(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("IsEmpty() missing parameter");

			var param = parameters[0];

			if (param.IsString)
				return new Value(string.IsNullOrWhiteSpace(param.String));

			return Value.CreateErrorValue("IsEmpty() contains invalid parameter type");
		}

		public static Value Length(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("Length() missing parameter");

			var param = parameters[0];

			if (param.IsString)
				return new Value(param.String.Length);

			return Value.CreateErrorValue("Length() contains invalid parameter type");
		}

		public static Value Trim(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("Trim() missing parameter");

			var param = parameters[0];

			if (param.IsString)
				return new Value(param.String.Trim(' '));

			return Value.CreateErrorValue("Trim() contains invalid parameter type");
		}

		public static Value Contains(IList<Value> parameters)
		{
			if (parameters.Count != 2)
				return Value.CreateErrorValue("Contains() missing parameter");

			var param = parameters[0];
			var param2 = parameters[1];

			if (param.IsString && param2.IsString)
				return new Value(param.String.Contains(param2.String));

			return Value.CreateErrorValue("Contains() contains invalid parameter type");
		}

		public static Value Today(IList<Value> parameters)
		{
			return new Value(DateTime.Now.Date);
		}

		public static Value DaysInMonth(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("DaysInMonth() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
			{
				return new Value(DateTime.DaysInMonth(param.Date.Year, param.Date.Month));
			}

			return Value.CreateErrorValue("DaysInMonth() contains invalid parameter type");
		}

		public static Value LastDayOfMonth(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("LastDayOfMonth() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
			{
				return new Value(new DateTime(param.Date.Year, param.Date.Month, DateTime.DaysInMonth(param.Date.Year, param.Date.Month)));
			}

			return Value.CreateErrorValue("LastDayOfMonth() contains invalid parameter type");
		}

		public static Value FirstDayOfMonth(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("FirstDayOfMonth() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
			{
				return new Value(new DateTime(param.Date.Year, param.Date.Month, 1));
			}

			return Value.CreateErrorValue("FirstDayOfMonth() contains invalid parameter type");
		}

		public static Value LastDayOfYear(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("LastDayOfYear() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
			{
				return new Value(new DateTime(param.Date.Year, param.Date.Month, 31));
			}

			return Value.CreateErrorValue("LastDayOfYear() contains invalid parameter type");
		}

		public static Value FirstDayOfYear(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("FirstDayOfMonth() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
			{
				return new Value(new DateTime(param.Date.Year, 1, 1));
			}

			return Value.CreateErrorValue("FirstDayOfMonth() contains invalid parameter type");
		}

		public static Value IsNot(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("IsNot() missing parameter");

			var param = parameters[0];

			if (param.IsBoolean)
				return new Value(!param.Boolean);

			return Value.CreateErrorValue("IsNot() contains invalid parameter type");
		}

		public static Value GetIndex(IList<Value> parameters)
		{
			if (parameters.Count <= 2)
				return Value.CreateErrorValue("GetIndex() error too few parameters");

			var match = parameters[0];

			if (match.IsError)
				return match;

			if (match.IsObject)
				return Value.CreateErrorValue("GetIndex() can not find index of type object");

			for (int i = 1; i < parameters.Count; i++)
			{
				var param = parameters[i];

				if (match.IsString && match.String == param.String)
				{
					return new Value(i);
				}
				else if (match.IsInteger && match.Integer == param.Integer)
				{
					return new Value(i);
				}
				else if (match.IsBoolean && match.Boolean == param.Boolean)
				{
					return new Value(i);
				}
				else if (match.IsDate && match.Date == param.Date)
				{
					return new Value(i);
				}
				else if (match.IsDecimal && match.Decimal == param.Decimal)
				{
					return new Value(i);
				}
				else if (match.IsFloat && match.Float == param.Float)
				{
					return new Value(i);
				}
				else if (param.IsError)
				{
					return param;
				}
			}

			return new Value(0);
		}

		public static Value GetIndexValue(IList<Value> parameters)
		{
			if (parameters.Count <= 2)
				return Value.CreateErrorValue("GetIndexValue() error too few parameters");

			var indexParam = parameters[0];

			if (indexParam.IsError)
				return indexParam;

			if (!(indexParam.IsInteger || indexParam.IsBoolean))
				return Value.CreateErrorValue("GetIndexValue() index parameter is not not an integer or boolean");

			int index = indexParam.Integer;

			if (indexParam.IsBoolean)
				index = indexParam.Boolean ? 1 : 2;

			if (index <= 0)
				return Value.CreateErrorValue("GetIndexValue() index must be greater than zero");

			if (index >= parameters.Count)
				return Value.CreateErrorValue("GetIndexValue() indexed a non-existing parameter");

			return parameters[index];
		}
	}
}