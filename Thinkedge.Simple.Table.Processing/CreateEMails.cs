using System.Collections.Generic;
using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Table.Processing
{
	internal class CreateEMails : BaseStandardResult
	{
		protected string Template;
		protected string GroupExpression = null;
		protected EvaluatorCache Cache = new EvaluatorCache();

		internal SimpleTable Execute(SimpleTable sourceTable, string template, string fromMail, string toField, string groupExpression = null)
		{
			ClearError();
			Template = template;

			var templates = new List<MessageTemplate>();
			var lookup = (groupExpression != null) ? new Dictionary<string, MessageTemplate>() : null;

			GroupExpression = string.IsNullOrWhiteSpace(groupExpression) ? null : groupExpression;

			foreach (var row in sourceTable)
			{
				var sendTo = row[toField];
				var groupName = GetGroupingName(row);

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

				if (HasError) return null;
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
				row["Mail-From"] = fromMail;
			}

			return messageTable;
		}

		protected string GetGroupingName(SimpleTableRow row)
		{
			if (GroupExpression == null)
			{
				return null;
			}

			var tableSource = new TableDataSource()
			{
				Row = row
			};

			var evaluation = Cache.Compile(GroupExpression);

			var result = evaluation.Evaluate(tableSource);

			if (result.IsError)
			{
				SetError(result.String);
				return null;
			}

			if (!result.IsString)
			{
				SetError("message processing error - group evaulation result not a string");
				return null;
			}

			string group = result.String;

			//if (string.IsNullOrWhiteSpace(group))
			//{
			//	ErrorMessage = "message process error - group evaulation result is empty";
			//	return null;
			//}

			return group;
		}
	}
}