using System.Collections.Generic;
using Thinkedge.Common;
using Thinkedge.Simple.Evaluator;

namespace Thinkedge.Simple.Table.Process
{
	public static class CreateEMails
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, string template, string sendFromMail, string sendToFieldExpression, string groupExpression = null)
		{
			var templates = new List<MessageTemplate>();
			var lookup = (groupExpression != null) ? new Dictionary<string, MessageTemplate>() : null;

			groupExpression = string.IsNullOrWhiteSpace(groupExpression) ? null : groupExpression;

			foreach (var row in sourceTable)
			{
				var sendTo = EvaulateExpression(row, sendToFieldExpression);

				if (sendTo.HasError)
					return StandardResult<SimpleTable>.ReturnError(sendTo.ErrorMessage); // fixme

				var groupName = EvaulateExpression(row, groupExpression);

				if (groupName.HasError)
					return StandardResult<SimpleTable>.ReturnError(groupName.ErrorMessage); // fixme

				if (lookup == null || groupName.Result == null || !lookup.TryGetValue(groupName.Result, out MessageTemplate message))
				{
					message = new MessageTemplate(template);
					message.SetBodyFields(row);

					message.To = sendTo.Result;

					templates.Add(message);

					if (lookup != null || groupName != null)
					{
						lookup.Add(groupName.Result, message);
					}
				}

				message.SetRowFields(row);
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

			return StandardResult<SimpleTable>.ReturnResult(messageTable);
		}

		private static StandardResult<string> EvaulateExpression(SimpleTableRow row, string expressionText)
		{
			if (expressionText == null)
				return StandardResult<string>.ReturnResult(null);

			var fieldSource = new TableDataSource()
			{
				Row = row
			};

			var expression = ExpressionCache.Compile(expressionText);

			var result = expression.Evaluate(new Context() { FieldSource = fieldSource });

			if (result.IsError)
				StandardResult<string>.ReturnError(result.String);

			if (!result.IsString)
				StandardResult<string>.ReturnError("message processing error - expression evaulation did not result in a string");

			return StandardResult<string>.ReturnResult(result.String);
		}
	}
}