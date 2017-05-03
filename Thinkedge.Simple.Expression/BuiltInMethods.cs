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
				case "ToInteger": return ToInteger(parameters);
				case "ToDate": return ToDate(parameters);
				case "IsDate": return IsDate(parameters);
				case "IsEmpty": return IsEmpty(parameters);
				case "IsWhiteSpace": return IsWhiteSpace(parameters);
				case "Length": return Length(parameters);
				case "Contain": return Length(parameters);
				case "Trim": return Trim(parameters);
				case "Today": return Today(parameters);

				default: break;
			}

			return null;
		}

		public static Value ToInteger(IList<Value> parameters)
		{
			var param = parameters[0];

			if (param.IsString)
				return new Value(Convert.ToInt32(param.String));
			else if (param.IsFloat)
				return new Value(Convert.ToInt32(param.Float));
			else if (param.IsInteger)
				return new Value(param.Integer);

			return Value.CreateErrorValue("invalid parameter type for ToInteger() method");
		}

		public static Value ToDate(IList<Value> parameters)
		{
			var param = parameters[0];

			if (param.IsString)
				return new Value(Convert.ToDateTime(param.String));
			else if (param.IsDate)
				return new Value(param.Date);

			return Value.CreateErrorValue("invalid parameter type for ToDate() method");
		}

		public static Value IsDate(IList<Value> parameters)
		{
			var param = parameters[0];

			if (param.IsString)
			{
				return new Value(DateTime.TryParse(param.String, out DateTime date));
			}
			else if (param.IsDate)
				return new Value(true);

			return Value.CreateErrorValue("invalid parameter type for ToDate() method");
		}

		public static Value IsEmpty(IList<Value> parameters)
		{
			var param = parameters[0];

			if (param.IsString)
				return new Value(param.String.Length == 0);

			return Value.CreateErrorValue("invalid parameter type for IsEmpty() method");
		}

		public static Value IsWhiteSpace(IList<Value> parameters)
		{
			var param = parameters[0];

			if (param.IsString)
				return new Value(string.IsNullOrWhiteSpace(param.String));

			return Value.CreateErrorValue("invalid parameter type for IsWhiteSpace() method");
		}

		public static Value Length(IList<Value> parameters)
		{
			var param = parameters[0];

			if (param.IsString)
				return new Value(param.String.Length);

			return Value.CreateErrorValue("invalid parameter type for Length() method");
		}

		public static Value Trim(IList<Value> parameters)
		{
			var param = parameters[0];

			if (param.IsString)
				return new Value(param.String.Trim(' '));

			return Value.CreateErrorValue("invalid parameter type for Trim() method");
		}

		public static Value Contains(IList<Value> parameters)
		{
			var param = parameters[0];
			var param2 = parameters[1];

			if (param.IsString && param2.IsString)
				return new Value(param.String.Contains(param2.String));

			return Value.CreateErrorValue("invalid parameter type for Contains() method");
		}

		public static Value Today(IList<Value> parameters)
		{
			return new Value(DateTime.Now.Date);
		}
	}
}