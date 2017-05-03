using System.Collections.Generic;
using System.Net.Mail;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Processing
{
	public class Process
	{
		public static StandardResult<SimpleTable> Filter(SimpleTable sourceTable, string includeExpression)
		{
			var filter = new FilterTable();

			var result = filter.Execute(sourceTable, includeExpression);

			return new StandardResult<SimpleTable>(result, filter);
		}

		public static StandardResult<SimpleTable> LookupUpdate(SimpleTable sourceTable, string sourceKeyField, string mergeField, SimpleTable lookupTable, string lookupKeyField, string lookupDataField, bool overwrite, bool caseInsensitive = true)
		{
			var update = new LookupUpdateTable();

			var result = update.Execute(sourceTable, sourceKeyField, mergeField, lookupTable, lookupKeyField, lookupDataField, overwrite, caseInsensitive);

			return new StandardResult<SimpleTable>(result, update);
		}

		public static StandardResult<SimpleTable> Transform(SimpleTable sourceTable, SimpleTable mapTable)
		{
			var transform = new TransformTable();

			var result = transform.Execute(sourceTable, mapTable);

			return new StandardResult<SimpleTable>(result, transform);
		}

		public static StandardResult<SimpleTable> Validate(SimpleTable sourceTable, SimpleTable validationRules, SimpleTable validationMap)
		{
			var validate = new ValidateTable();

			var result = validate.Execute(sourceTable, validationRules, validationMap);

			return new StandardResult<SimpleTable>(result, validate);
		}

		public static StandardResult<string> LoadFromFile(string filename)
		{
			var load = new LoadFromFile();

			var result = load.Execute(filename);

			return new StandardResult<string>(result, load);
		}

		public static StandardResult<SimpleTable> ParseTabDelimited(string data, bool containsHeader = true, bool mapHeaderNames = true, bool unespace = true)
		{
			var load = new ParseTabDelimited();

			var result = load.Execute(data, containsHeader, mapHeaderNames, unespace);

			return new StandardResult<SimpleTable>(result, load);
		}

		public static StandardResult<SimpleTable> ParseValuePairs(string data, string destination = "destination", string source = "source", char delimiter = '=')
		{
			var load = new ParseValuePairs();

			var result = load.Execute(data, destination, source, delimiter);

			return new StandardResult<SimpleTable>(result, load);
		}

		public static StandardResult<SimpleTable> ParseCustomINI(string data, string newRow, char delimiter = '=', IList<string> columns = null)
		{
			var parse = new ParseCustomINI();

			var result = parse.Execute(data, newRow, delimiter, columns);

			return new StandardResult<SimpleTable>(result, parse);
		}

		public static StandardResult<SimpleTable> CreateEMails(SimpleTable sourceTable, string template, string fromMail, string toField, string groupExpression = null)
		{
			var emails = new CreateEMails();

			var result = emails.Execute(sourceTable, template, fromMail, toField, groupExpression);

			return new StandardResult<SimpleTable>(result, emails);
		}

		public static StandardResult<bool> SaveToFile(string filename, string data)
		{
			var save = new SaveToFile();

			save.Execute(filename, data);

			return new StandardResult<bool>(!save.HasError, save);
		}

		public static StandardResult<string> FormatToTabDelimited(SimpleTable table, bool escape = true)
		{
			var format = new FormatToTabDelimited();

			var result = format.Execute(table, escape);

			return new StandardResult<string>(result, format);
		}

		public static StandardResult<bool> SendEMails(SimpleTable sourceTable)
		{
			var emails = new SendEMails();

			emails.Execute(sourceTable);

			return new StandardResult<bool>(!emails.HasError, emails);
		}

		public static StandardResult<bool> SendEMail(MailMessage message)
		{
			var email = new SendEMail();

			var result = email.Execute(message);

			return new StandardResult<bool>(result, email);
		}
	}
}