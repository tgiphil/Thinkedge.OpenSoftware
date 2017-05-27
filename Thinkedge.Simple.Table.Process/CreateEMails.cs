﻿using System.Collections.Generic;
using Thinkedge.Common;
using Thinkedge.Simple.Evaluator;

namespace Thinkedge.Simple.Table.Process
{
	public class CreateEMails : BaseStandardResult
	{
		public static StandardResult<SimpleTable> Execute(SimpleTable sourceTable, string template, string sendFromMail, string sendToFieldExpression, string groupExpression = null)
		{
			return new CreateEMails().ExecuteEx(sourceTable, template, sendFromMail, sendToFieldExpression, groupExpression);
		}

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

		protected string EvaulateExpression(SimpleTableRow row, string expressionText)
		{
			if (expressionText == null)
				return null;

			var fieldSource = new TableDataSource()
			{
				Row = row
			};

			var expression = ExpressionCache.Compile(expressionText);

			var result = expression.Evaluate(new Context() { FieldSource = fieldSource });

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