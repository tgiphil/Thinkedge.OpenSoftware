using System.Net.Mail;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Process
{
	public static class SendEMails
	{
		public static StandardResult<bool> Execute(SimpleTable sourceTable)
		{
			foreach (var row in sourceTable)
			{
				var message = new MailMessage(row["Mail-From"], row["Mail-To"])
				{
					Subject = row["Mail-Subject"],
					Body = row["Mail-Body"]
				};

				if (sourceTable.ContainColumn("Mail-CC"))
				{
					var cc = row["Mail-CC"];

					if (!string.IsNullOrWhiteSpace(cc))
					{
						message.CC.Add(cc);
					}
				}

				message.IsBodyHtml = true;

				var result = SendEMail.Execute(message);

				if (result.HasError)
				{
					return result;
				}
			}

			return StandardResult<bool>.ReturnResult(true);
		}
	}
}