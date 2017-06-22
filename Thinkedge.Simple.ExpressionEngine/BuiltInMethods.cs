using System;
using System.Collections.Generic;
using System.Globalization;

namespace Thinkedge.Simple.ExpressionEngine
{
	public class BuiltInMethods : IMethodSource
	{
		Value IMethodSource.Evaluate(string name, IList<Value> parameters, Context context)
		{
			switch (name)
			{
				//short versions
				case "ToInteger": return ToInteger(parameters);
				case "ToInt": return ToInteger(parameters);
				case "ToDate": return ToDate(parameters);
				case "ToDecimal": return ToDecimal(parameters);
				case "ToBoolean": return ToBoolean(parameters);
				case "IsDate": return IsDate(parameters);
				case "IsInteger": return IsInteger(parameters);
				case "IsDecimal": return IsDecimal(parameters);
				case "IsEmpty": return IsEmpty(parameters);
				case "IsNull": return IsNull(parameters);
				case "IsNot": return IsNot(parameters);
				case "IsError": return IsError(parameters);
				case "Length": return Length(parameters);
				case "Contains": return Contains(parameters);
				case "StartsWith": return StartsWith(parameters);
				case "EndsWith": return EndsWith(parameters);
				case "Trim": return Trim(parameters);
				case "Today": return Today(parameters);
				case "Tomorrow": return Tomorrow(parameters);
				case "Yesterday": return Yesterday(parameters);
				case "DaysInMonth": return DaysInMonth(parameters);
				case "FirstDayOfYear": return FirstDayOfYear(parameters);
				case "FirstDayOfMonth": return FirstDayOfMonth(parameters);
				case "FirstDayOfQuarter": return FirstDayOfQuarter(parameters);
				case "LastDayOfQuarter": return LastDayOfQuarter(parameters);
				case "LastDayOfMonth": return LastDayOfMonth(parameters);
				case "LastDayOfYear": return LastDayOfYear(parameters);
				case "GetIndex": return GetIndex(parameters);
				case "GetIndexValue": return GetIndexValue(parameters);
				case "Month": return Month(parameters);
				case "Year": return Year(parameters);
				case "Day": return Day(parameters);
				case "Quarter": return Quarter(parameters);
				case "MonthName": return MonthName(parameters);
				case "MonthShortName": return MonthShortName(parameters);
				case "IsWeekend": return IsWeekend(parameters);
				case "DayOfWeek": return DayOfWeek(parameters);
				case "DayOfYear": return DayOfYear(parameters);
				case "Replace": return Replace(parameters);
				case "In": return InList(parameters);
				case "InList": return InList(parameters);
				case "IsBetween": return IsBetween(parameters);
				case "Max": return Max(parameters);
				case "Min": return Min(parameters);

				//todo:
				//  SubString
				//  Right, Left
				//  CountWeekendsDays
				//  CountNonWeekendsDays
				//  IncrementToFirstNonWeekend

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
			{
				if (param.IsStringEmptyNonWhiteSpace)
					return new Value(string.Empty);

				if (!Int32.TryParse(param.String, out Int32 date))
					return Value.CreateNullValue(ValueType.Integer);

				return new Value(Convert.ToInt32(param.String));
			}
			else if (param.IsFloat)
				return new Value(Convert.ToInt32(param.Float));
			else if (param.IsInteger)
				return new Value(param.Integer);

			return Value.CreateErrorValue("ToInteger() invalid parameter type");
		}

		public static Value ToDecimal(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("ToDecimal() missing parameter");

			var param = parameters[0];

			if (param.IsString)
			{
				if (!Decimal.TryParse(param.String, out Decimal d))
					return Value.CreateNullValue(ValueType.Decimal);

				return new Value(Convert.ToDecimal(param.String));
			}
			else if (param.IsFloat)
				return new Value(Convert.ToDecimal(param.Float));
			else if (param.IsInteger)
				return new Value(Convert.ToDecimal(param.IsInteger));
			else if (param.IsDecimal)
				return new Value(param.Decimal);

			return Value.CreateErrorValue("ToDecimal() contains invalid parameter type");
		}

		public static Value ToBoolean(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("ToBoolean() missing parameter");

			var param = parameters[0];

			if (param.IsString && !string.IsNullOrWhiteSpace(param.String))
			{
				var s = param.String.Trim().ToLower();

				if (s == "true" || s == "yes" || s == "t" || s == "y")
					return new Value(true);
				else if (s == "false" || s == "no" || s == "f" || s == "n")
					return new Value(false);
			}
			else if (param.IsBoolean)
				return param;

			return Value.CreateErrorValue("ToDecimal() contains invalid parameter type");
		}

		public static Value ToDate(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("ToDate() missing parameter");

			var param = parameters[0];

			if (param.IsString)
			{
				if (!DateTime.TryParse(param.String, out DateTime date))
					return Value.CreateNullValue(ValueType.Date);

				return new Value(Convert.ToDateTime(param.String).Date);
			}
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
				if (param.IsStringEmptyNonWhiteSpace)
					return new Value(false);

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
			{
				if (!Int32.TryParse(param.String, out Int32 date))
					return Value.CreateErrorValue("IsInteger(): invalid integer: " + param.String);

				return new Value(Int32.TryParse(param.String, out int i));
			}
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
				return new Value(param.IsStringEmptyNonWhiteSpace);

			return Value.CreateErrorValue("IsEmpty() contains invalid parameter type");
		}

		public static Value IsNull(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("IsNull() missing parameter");

			var param = parameters[0];

			return new Value(param.IsNull);
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

		public static Value StartsWith(IList<Value> parameters)
		{
			if (parameters.Count != 2)
				return Value.CreateErrorValue("StartsWith() missing parameter");

			var param = parameters[0];
			var param2 = parameters[1];

			if (param.IsString && param2.IsString)
				return new Value(param.String.StartsWith(param2.String));

			return Value.CreateErrorValue("StartsWith() contains invalid parameter type");
		}

		public static Value EndsWith(IList<Value> parameters)
		{
			if (parameters.Count != 2)
				return Value.CreateErrorValue("EndsWith() missing parameter");

			var param = parameters[0];
			var param2 = parameters[1];

			if (param.IsString && param2.IsString)
				return new Value(param.String.EndsWith(param2.String));

			return Value.CreateErrorValue("EndsWith() contains invalid parameter type");
		}

		public static Value Replace(IList<Value> parameters)
		{
			if (parameters.Count != 3)
				return Value.CreateErrorValue("Replace() missing parameter");

			var param1 = parameters[0];
			var param2 = parameters[1];
			var param3 = parameters[2];

			if (param1.IsString && param2.IsString && param3.IsString)
				return new Value(param1.String.Replace(param2.String, param3.String));

			return Value.CreateErrorValue("Replace() contains invalid parameter type");
		}

		public static Value Today(IList<Value> parameters)
		{
			return new Value(DateTime.Now.Date);
		}

		public static Value Tomorrow(IList<Value> parameters)
		{
			return new Value(DateTime.Now.Date.AddDays(1));
		}

		public static Value Yesterday(IList<Value> parameters)
		{
			return new Value(DateTime.Now.Date.AddDays(-1));
		}

		public static Value DaysInMonth(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("DaysInMonth() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(DateTime.DaysInMonth(param.Date.Year, param.Date.Month));

			return Value.CreateErrorValue("DaysInMonth() contains invalid parameter type");
		}

		public static Value LastDayOfMonth(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("LastDayOfMonth() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(new DateTime(param.Date.Year, param.Date.Month, DateTime.DaysInMonth(param.Date.Year, param.Date.Month)));

			return Value.CreateErrorValue("LastDayOfMonth() contains invalid parameter type");
		}

		public static Value FirstDayOfQuarter(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("FirstDayOfQuarter() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
			{
				int month = ((((param.Date.Month - 1) / 3) + 1) * 3) - 2;
				return new Value(new DateTime(param.Date.Year, month, 1));
			}

			return Value.CreateErrorValue("FirstDayOfQuarter() contains invalid parameter type");
		}

		public static Value LastDayOfQuarter(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("LastDayOfQuarter() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
			{
				int month = (((param.Date.Month - 1) / 3) + 1) * 3;
				return new Value(new DateTime(param.Date.Year, month, DateTime.DaysInMonth(param.Date.Year, month)));
			}

			return Value.CreateErrorValue("LastDayOfQuarter() contains invalid parameter type");
		}

		public static Value FirstDayOfMonth(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("FirstDayOfMonth() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(new DateTime(param.Date.Year, param.Date.Month, 1));

			return Value.CreateErrorValue("FirstDayOfMonth() contains invalid parameter type");
		}

		public static Value LastDayOfYear(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("LastDayOfYear() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(new DateTime(param.Date.Year, param.Date.Month, 31));

			return Value.CreateErrorValue("LastDayOfYear() contains invalid parameter type");
		}

		public static Value Year(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("GetYear() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(param.Date.Year);

			return Value.CreateErrorValue("GetYear() contains invalid parameter type");
		}

		public static Value Month(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("GetMonth() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(param.Date.Month);

			return Value.CreateErrorValue("GetMonth() contains invalid parameter type");
		}

		public static Value Day(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("GetDay() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(param.Date.Day);

			return Value.CreateErrorValue("GetDay() contains invalid parameter type");
		}

		public static Value Quarter(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("GetQuarter() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(((param.Date.Month - 1) / 3) + 1);

			return Value.CreateErrorValue("GetQuarter() contains invalid parameter type");
		}

		public static Value DayOfWeek(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("DayOfWeek() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(param.Date.DayOfWeek);

			return Value.CreateErrorValue("DayOfWeek() contains invalid parameter type");
		}

		public static Value DayOfYear(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("DayOfWeek() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value((int)param.Date.DayOfYear);

			return Value.CreateErrorValue("DayOfWeek() contains invalid parameter type");
		}

		public static Value IsWeekend(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("IsWeekend() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(param.Date.DayOfWeek == System.DayOfWeek.Saturday || param.Date.DayOfWeek == System.DayOfWeek.Sunday);

			return Value.CreateErrorValue("IsWeekend() contains invalid parameter type");
		}

		public static Value FirstDayOfYear(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("FirstDayOfMonth() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(new DateTime(param.Date.Year, 1, 1));

			return Value.CreateErrorValue("FirstDayOfMonth() contains invalid parameter type");
		}

		public static Value MonthName(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("GetMonthName() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(param.Date.Month));
			else if (param.IsInteger && param.Integer >= 1 && param.Integer <= 12)
				return new Value(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(param.Integer));

			return Value.CreateErrorValue("GetMonthName() contains invalid parameter type");
		}

		public static Value MonthShortName(IList<Value> parameters)
		{
			if (parameters.Count != 1)
				return Value.CreateErrorValue("GetMonthShortName() missing parameter");

			var param = parameters[0];

			if (param.IsDate)
				return new Value(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(param.Date.Month));
			else if (param.IsInteger && param.Integer >= 1 && param.Integer <= 12)
				return new Value(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(param.Integer));

			return Value.CreateErrorValue("GetMonthShortName() contains invalid parameter type");
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
					return new Value(i);
				else if (match.IsInteger && match.Integer == param.Integer)
					return new Value(i);
				else if (match.IsBoolean && match.Boolean == param.Boolean)
					return new Value(i);
				else if (match.IsDate && match.Date == param.Date)
					return new Value(i);
				else if (match.IsDecimal && match.Decimal == param.Decimal)
					return new Value(i);
				else if (match.IsFloat && match.Float == param.Float)
					return new Value(i);
				else if (param.IsError)
					return param;
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

		public static Value InList(IList<Value> parameters)
		{
			if (parameters.Count < 2)
				return Value.CreateErrorValue("InList() missing parameter");

			var match = parameters[0];

			for (int i = 1; i < parameters.Count; i++)
			{
				var param = parameters[i];

				if (match.ValueType != param.ValueType)
					continue;

				if (match.IsString && param.IsString && match.String == param.String)
					return new Value(true);
				else if (match.IsInteger && param.IsInteger && match.Integer == param.Integer)
					return new Value(true);
				else if (match.IsDate && param.IsDate && match.Date == param.Date)
					return new Value(true);
				else if (match.IsFloat && param.IsFloat && match.Float == param.Float)
					return new Value(true);
				else if (match.IsBoolean && param.IsBoolean && match.Boolean == param.Boolean)
					return new Value(true);
				else if (match.IsDecimal && param.IsBoolean && match.Decimal == param.Decimal)
					return new Value(true);
			}

			return new Value(false);
		}

		public static Value IsBetween(IList<Value> parameters)
		{
			if (parameters.Count != 3)
				return Value.CreateErrorValue("Between() missing parameter");

			var param = parameters[0];
			var a = parameters[1];
			var b = parameters[2];

			if (param.ValueType != a.ValueType || param.ValueType != b.ValueType)
				return Value.CreateErrorValue("Between() contains invalid parameter types");

			if (param.IsInteger)
				return new Value((param.Integer >= a.Integer && param.Integer <= b.Integer) || (param.Integer <= a.Integer && param.Integer >= b.Integer));
			else if (param.IsDate)
				return new Value((param.Date >= a.Date && param.Date <= b.Date) || (param.Date <= a.Date && param.Date >= b.Date));
			else if (param.IsFloat)
				return new Value((param.Float >= a.Float && param.Float <= b.Float) || (param.Float <= a.Float && param.Float >= b.Float));
			else if (param.IsDecimal)
				return new Value((param.Decimal >= a.Decimal && param.Decimal <= b.Decimal) || (param.Decimal <= a.Decimal && param.Decimal >= b.Decimal));

			return Value.CreateErrorValue("Between() contains invalid parameter types");
		}

		public static Value Max(IList<Value> parameters)
		{
			if (parameters.Count != 2)
				return Value.CreateErrorValue("Max() missing parameter");

			var a = parameters[0];
			var b = parameters[1];

			if (a.ValueType != b.ValueType)
				return Value.CreateErrorValue("Max() contains invalid parameter types");

			if (a.IsInteger) return (a.Integer >= b.Integer) ? a : b;
			if (a.IsDate) return (a.Date >= b.Date) ? a : b;
			if (a.IsFloat) return (a.Float >= b.Float) ? a : b;
			if (a.IsDecimal) return (a.Decimal >= b.Decimal) ? a : b;

			return Value.CreateErrorValue("Max() contains invalid parameter types");
		}

		public static Value Min(IList<Value> parameters)
		{
			if (parameters.Count != 2)
				return Value.CreateErrorValue("Min() missing parameter");

			var a = parameters[0];
			var b = parameters[1];

			if (a.ValueType != b.ValueType)
				return Value.CreateErrorValue("Min() contains invalid parameter types");

			if (a.IsInteger) return (a.Integer < b.Integer) ? a : b;
			if (a.IsDate) return (a.Date < b.Date) ? a : b;
			if (a.IsFloat) return (a.Float < b.Float) ? a : b;
			if (a.IsDecimal) return (a.Decimal < b.Decimal) ? a : b;

			return Value.CreateErrorValue("Min() contains invalid parameter types");
		}
	}
}