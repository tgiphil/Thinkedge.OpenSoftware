using System.Net.Mail;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Processing
{
	internal class SendEMails : BaseStandardResult
	{
		internal void Execute(SimpleTable sourceTable)
		{
			foreach (var row in sourceTable)
			{
				var message = new MailMessage(row["Mail-From"], row["Mail-To"])
				{
					Subject = row["Mail-Subject"],
					Body = row["Mail-Body"]
				};
				
				message.IsBodyHtml = true;

				var result = Process.SendEMail(message);

				if (result.HasError)
				{
					SetError(result.ErrorMessage, result.Exception);
					return;
				}
			}
		}
	}
}