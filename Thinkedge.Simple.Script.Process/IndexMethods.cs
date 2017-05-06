using System.Collections.Generic;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Script.Process
{
	public class IndexMethods : IMethodSource
	{
		Value IMethodSource.Evaluate(string name, IList<Value> parameters)
		{
			switch (name)
			{
				//short versions
				case "GetIndex": return GetIndexOf(parameters);
				case "GetIndexValue": return GetIndexValue(parameters);
				//long versions
				case "Index.Get": return GetIndexOf(parameters);
				case "Index.GetValue": return GetIndexValue(parameters);
				default: break;
			}

			return null;
		}

		public static Value GetIndexOf(IList<Value> parameters)
		{
			if (parameters.Count <= 2)
				return Value.CreateErrorValue("GetIndexOf() error too few parameters");

			var match = parameters[0];

			if (match.IsError)
				return match;

			if (match.IsObject)
				return Value.CreateErrorValue("GetIndexOf() can not find index of type object");

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
				return Value.CreateErrorValue("GetIndexOf() error too few parameters");

			var indexParam = parameters[0];

			if (indexParam.IsError)
				return indexParam;

			if (indexParam.IsInteger || indexParam.IsBoolean)
				return Value.CreateErrorValue("GetIndexOf() index parameter is not not an integer or boolean");

			int index = indexParam.Integer;

			if (indexParam.IsBoolean)
				index = indexParam.Boolean ? 1 : 2;

			if (index <= 1)
				return Value.CreateErrorValue("GetIndexOf() index must be greater than zero");

			if (parameters.Count > index)
				return Value.CreateErrorValue("GetIndexOf() indexed a non-existing parameter");

			return parameters[index];
		}
	}
}