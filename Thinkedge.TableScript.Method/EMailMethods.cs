using System.Collections.Generic;
using Thinkedge.Simple.ExpressionEngine;
using Thinkedge.Simple.Table;

namespace Thinkedge.TableScript.Method
{
	public class EMailMethods : IMethodSource
	{
		Value IMethodSource.Evaluate(string name, IList<Value> parameters, Context context)
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
			var validate = Expression.ValidateHelper("CreateEMails", parameters, 5, new List<ValueType>() { ValueType.Object, ValueType.String, ValueType.String, ValueType.String, ValueType.String });

			if (validate != null)
				return validate;

			if (parameters.Count == 5 && !parameters[4].IsString)
				return Value.CreateErrorValue("CreateEMails: parameter #5 is not a string");
			else if (!(parameters[0].Object is SimpleTable))
				return Value.CreateErrorValue("CreateEMails: parameter #1 is not a table");

			var sourceTable = parameters[0].Object as SimpleTable;
			var template = parameters[1].String;
			var fromMail = parameters[2].String;
			var toField = parameters[3].String;
			var groupExpression = parameters.Count != 5 ? null : parameters[4].String;

			try
			{
				var result = Simple.Table.Method.CreateEMails.Execute(sourceTable, template, fromMail, toField, string.Empty, groupExpression);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(result.Result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("CreateEMails: unable to perform lookup update", e);
			}
		}

		public static Value SendEMails(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("SendEMails", parameters, 1, new List<ValueType>() { ValueType.Object, ValueType.String });

			if (validate != null)
				return validate;

			if (!parameters[0].IsObject)
				return Value.CreateErrorValue("SendEMails: parameter #1 is not a table");

			var sourceTable = parameters[0].Object as SimpleTable;

			try
			{
				var result = Simple.Table.Method.SendEMails.Execute(sourceTable);

				if (result.HasError)
					return Value.CreateErrorValue(result.ErrorMessage, result.Exception);

				return new Value(true);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("SendEMails: unable to send emails: " + e.ToString(), e);
			}
		}
	}
}