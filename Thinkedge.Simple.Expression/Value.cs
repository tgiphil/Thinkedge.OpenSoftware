﻿using System;
using System.Text;

namespace Thinkedge.Simple.Expression
{
	public class Value
	{
		public ValueType ValueType { get; protected set; }

		public int Integer { get; protected set; }
		public bool Boolean { get; protected set; }
		public decimal Decimal { get; protected set; }
		public String String { get; protected set; }
		public DateTime Date { get; protected set; }
		public double Float { get; protected set; }
		public object Object { get; protected set; }
		public Exception Exception { get; protected set; }

		public Value(int i)
		{
			ValueType = ValueType.Integer; Integer = i;
		}

		public Value(bool b)
		{
			ValueType = ValueType.Boolean; Boolean = b;
		}

		public Value(decimal d)
		{
			ValueType = ValueType.Decimal; Decimal = d;
		}

		public Value(String s)
		{
			ValueType = ValueType.String; String = s;
		}

		public Value(DateTime d)
		{
			ValueType = ValueType.Date; Date = d;
		}

		public Value(double f)
		{
			ValueType = ValueType.Float; Float = f;
		}

		public Value(object u)
		{
			ValueType = ValueType.Object; Object = u;
		}

		public Value(Exception exception, String error)
		{
			ValueType = ValueType.Error;
			String = error;
			Exception = exception;
		}

		public static Value CreateErrorValue(string error, Exception exception = null)
		{
			return new Value(exception, error);
		}

		public bool IsInteger { get { return ValueType == ValueType.Integer; } }
		public bool IsBoolean { get { return ValueType == ValueType.Boolean; } }
		public bool IsDecimal { get { return ValueType == ValueType.Decimal; } }
		public bool IsString { get { return ValueType == ValueType.String; } }
		public bool IsDate { get { return ValueType == ValueType.Date; } }
		public bool IsFloat { get { return ValueType == ValueType.Float; } }
		public bool IsObject { get { return ValueType == ValueType.Object; } }
		public bool IsError { get { return ValueType == ValueType.Error; } }

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.Append(ValueType.ToString());
			sb.Append(" = ");

			if (IsInteger)
				sb.Append(Integer.ToString());
			else if (IsDate)
				sb.Append(Date.ToShortDateString());
			else if (IsBoolean)
			{
				if (Boolean)
					sb.Append("true");
				else
					sb.Append("false");
			}
			else if (IsDecimal)
				sb.Append(Decimal.ToString());
			else if (IsString)
				sb.Append(String);
			else if (IsFloat)
				sb.Append(Float.ToString());
			else if (IsObject)
				sb.Append(Object.ToString());
			else if (IsError)
				sb.Append(String);

			return sb.ToString();
		}
	}
}