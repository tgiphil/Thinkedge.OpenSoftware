using System.Collections.Generic;
using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Process
{
	public class CreateEMails : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, string template, string sendFromMail, string sendToFieldExpression, string groupExpression = null)
		{
			return new CreateEMails().ExecuteEx(sourceTable, template, sendFromMail, sendToFieldExpression, groupExpression);
		}

		protected EvaluatorCache Cache = new EvaluatorCache();

		internal StandardResult<SimpleTable> ExecuteEx(SimpleTable sourceTable, string template, string sendFromMail, string sendToFieldExpression, string groupExpression = null)
		{
			var templates = new List<MessageTemplate>();
			var lookup = (groupExpression != null) ? new Dictionary<string, MessageTemplate>() : null;

			groupExpression = string.IsNullOrWhiteSpace(groupExpression) ? null : groupExpression;

			foreach (var row in sourceTable)
			{
				var sendTo = EvaulateExpression(row, sendToFieldExpression);
				var groupName = EvaulateExpression(row, groupExpression);

				if (lookup == null || groupName == null || !lookup.TryGetValue(groupName, out MessageTemplate message))
				{
					message = new MessageTemplate(template);
					message.SetBodyFields(row);

					message.To = sendTo;

					templates.Add(message);

					if (lookup != null || groupName != null)
					{
						lookup.Add(groupName, message);
					}
				}

				message.SetRowFields(row);

				if (HasError)
					return ReturnError<SimpleTable>();
			}

			var messageTable = new SimpleTable();
			messageTable.AddColumnName("Mail-To");
			messageTable.AddColumnName("Mail-From");
			messageTable.AddColumnName("Mail-Subject");
			messageTable.AddColumnName("Mail-Body");

			foreach (var message in templates)
			{
				var row = messageTable.CreateRow();

				row["Mail-Body"] = message.Body;
				row["Mail-Subject"] = message.Subject;
				row["Mail-To"] = message.To;
				row["Mail-From"] = sendFromMail;
			}

			return ReturnResult<SimpleTable>(messageTable);
		}

		protected string EvaulateExpression(SimpleTableRow row, string expression)
		{
			if (expression == null)
				return null;

			var tableSource = new TableDataSource()
			{
				Row = row
			};

			var evaluation = Cache.Compile(expression);

			var result = evaluation.Evaluate(tableSource);

			if (result.IsError)
			{
				SetError(result.String);
				return null;
			}

			if (!result.IsString)
			{
				SetError("message processing error - expression evaulation did not result in a string");
				return null;
			}

			return result.String;
		}
	}
}