using System.Collections.Generic;
using Thinkedge.Simple.ExpressionEngine;

namespace Thinkedge.Simple.ScriptEngine.Method
{
	public class FileMethods : IMethodSource
	{
		Value IMethodSource.Evaluate(string name, IList<Value> parameters, Context context)
		{
			switch (name)
			{
				//short versions
				case "LoadFromFile": return LoadFromFile(parameters);
				case "SaveToFile": return SaveToFile(parameters);
				//long versions
				case "File.Load": return LoadFromFile(parameters);
				case "File.Save": return SaveToFile(parameters);
				default: break;
			}

			return null;
		}

		public static Value LoadFromFile(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("LoadFromFile", parameters, 1, new List<ValueType>() { ValueType.String });

			if (validate != null)
				return validate;

			var result = Method.LoadFromFile.Execute(parameters[0].String);

			if (result.HasError)
				return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

			return new Value(result.Result);
		}

		public static Value SaveToFile(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("SaveToFile", parameters, 1, new List<ValueType>() { ValueType.String, ValueType.String });

			if (validate != null)
				return validate;

			var result = Method.SaveToFile.Execute(parameters[0].String, parameters[1].String);

			if (result.HasError)
				return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

			return new Value(result.Result);
		}
	}
}