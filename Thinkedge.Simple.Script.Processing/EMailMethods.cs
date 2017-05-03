using System.Collections.Generic;
using Thinkedge.Simple.Expression;
using Thinkedge.Simple.Table;
using Thinkedge.Simple.Table.Processing;

namespace Thinkedge.Simple.Script.Processing
{
	public class EMailMethods : IMethodSource
	{
		Value IMethodSource.Evaluate(string name, IList<Value> parameters)
		{
			switch (name)
			{
				//short versions
				case "CreateEMails": return CreateEMails(parameters);
				case "SendEMails": return SendEMails(parameters);
				default: break;
			}

			return null;
		}

		public static Value CreateEMails(IList<Value> parameters)
		{
			var validate = Evaluator.ValidateHelper("CreateEMails", parameters, 5, new List<ValueType>() { ValueType.Object, ValueType.String, ValueType.String, ValueType.String, ValueType.String });

			if (validate != null)
				return validate;

			if (parameters.Count == 5 && !parameters[4].IsString)
			{
				return Value.CreateErrorValue("parameter #5 is not a string");
			}
			else if (!(parameters[0].Object is SimpleTable))
			{
				return Value.CreateErrorValue("parameter #1 is not a table");
			}

			var sourceTable = parameters[0].Object as SimpleTable;
			var template = parameters[1].String;
			var fromMail = parameters[2].String;
			var toField = parameters[3].String;
			var groupExpression = parameters.Count != 5 ? null : parameters[4].String;

			try
			{
				var result = Process.CreateEMails(sourceTable, template, fromMail, toField, groupExpression);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("unable to perform lookup update", e);
			}
		}

		public static Value SendEMails(IList<Value> parameters)
		{
			var validate = Evaluator.ValidateHelper("SendEMails", parameters, 1, new List<ValueType>() { ValueType.Object, ValueType.String });

			if (validate != null)
				return validate;

			if (!parameters[0].IsObject)
			{
				return Value.CreateErrorValue("parameter #1 is not a table");
			}

			var sourceTable = parameters[0].Object as SimpleTable;

			try
			{
				var result = Process.SendEMails(sourceTable);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("unable to read file: " + parameters[0].String, e);
			}
		}
	}
}