using System;
using System.Text;

namespace Thinkedge.Simple.Table.Process
{
	public class MessageTemplate
	{
		protected string InProcessBody;
		protected StringBuilder InProcessRows;

		public string To { get; set; }
		public string Subject { get; set; }

		protected string TemplateFull;
		protected string TemplateBody;
		protected string TemplateRow;
		protected string TemplateSubject;

		protected const string RowTag = "{Row}";
		protected const string RowStartTag = "{Row}";
		protected const string RowEndTag = "{/Row}";

		protected const string SubjectTag = "{Subject}";
		protected const string SubjectEndTag = "{/Subject}";

		public string Body
		{
			get
			{
				var final = InProcessBody.Replace(RowTag, InProcessRows.ToString());
				return final;
			}
		}

		public MessageTemplate(string template)
		{
			SetMessageTemplate(template);

			Subject = TemplateSubject;
			InProcessBody = TemplateBody;
			InProcessRows = new StringBuilder();
		}

		protected void SetMessageTemplate(string template)
		{
			TemplateFull = template;
			TemplateRow = string.Empty;
			TemplateSubject = null;

			int subjectStartTag = template.IndexOf(SubjectTag);
			int subjectEndTag = template.IndexOf(SubjectEndTag);

			if (subjectStartTag < 0 || subjectEndTag < 0)
			{
				throw new Exception("Invalid message template - missing subject");
			}

			TemplateSubject = template.Substring(subjectStartTag + SubjectTag.Length, subjectEndTag - subjectStartTag - SubjectTag.Length).Trim().Trim('\n').Trim('\r');

			TemplateBody = template.Substring(subjectEndTag + SubjectEndTag.Length + 1).TrimStart('\n').TrimStart('\r');

			int start = TemplateBody.IndexOf(RowStartTag);

			if (start >= 0)
			{
				int end = TemplateBody.IndexOf(RowEndTag);

				if (end > 0)
				{
					TemplateRow = TemplateBody.Substring(start + RowStartTag.Length, end - start - RowStartTag.Length);
					TemplateBody = TemplateBody.Substring(0, start) + RowTag + TemplateBody.Substring(end + RowEndTag.Length);
				}
			}
		}

		public void SetBodyFields(string name, string value)
		{
			InProcessBody = InProcessBody.Replace("[" + name + "]", value);
		}

		public void SetBodyFields(SimpleTableRow row)
		{
			for (int i = 0; i < row.ColumnCount; i++)
			{
				var columnName = row.Table.GetColumnName(i);
				var value = row[i];

				InProcessBody = InProcessBody.Replace("[" + columnName + "]", value);
			}
		}

		public void SetRowFields(SimpleTableRow row)
		{
			var sb = new StringBuilder(TemplateRow);

			foreach (var column in row.Table.ColumnNames)
			{
				var value = row[column];

				sb.Replace("[" + column + "]", value);
			}

			InProcessRows.Append(sb);
		}
	}
}