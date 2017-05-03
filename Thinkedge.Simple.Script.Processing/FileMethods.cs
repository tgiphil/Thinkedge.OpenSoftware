using System.Collections.Generic;
using System.IO;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Script.Processing
{
	public class FileMethods : IMethodSource
	{
		Value IMethodSource.Evaluate(string name, IList<Value> parameters)
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
			var validate = Evaluator.ValidateHelper("LoadFromFile",parameters, 1, new List<ValueType>() { ValueType.String });

			if (validate != null)
				return validate;

			try
			{
				string data = File.ReadAllText(parameters[0].String);
				return new Value(data);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("unable to read file: " + parameters[0].String, e);
			}
		}

		public static Value SaveToFile(IList<Value> parameters)
		{
			var validate = Evaluator.ValidateHelper("SaveToFile",parameters, 1, new List<ValueType>() { ValueType.String, ValueType.String });

			if (validate != null)
				return validate;

			try
			{
				File.WriteAllText(parameters[0].String, parameters[1].String);
				return new Value(true);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("unable to write file: " + parameters[0].String, e);
			}
		}
	}
}