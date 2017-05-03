using System;
using System.Net.Mail;
using System.Net.Mime;

namespace Thinkedge.Common
{
	public static class MailHelper
	{
		public static MailMessage CreateMailMessage(
			string from,
			string to,
			string subject,
			string body,
			FilePackage filePackage)
		{
			var message = new MailMessage(from, to)
			{
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			foreach (var file in filePackage.Files)
			{
				var attachment = new Attachment(file.Stream, file.ShortFilename, MediaTypeNames.Application.Octet);

				message.Attachments.Add(attachment);
			}

			return message;
		}

		public static bool SendMailMessage(MailMessage message)
		{
			try
			{
				var client = new SmtpClient()
				{
					Timeout = 200000 // 200 seconds
				};

				client.Send(message);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool SendMailMessage(MailMessage message, out Exception exception)
		{
			try
			{
				exception = null;

				var client = new SmtpClient()
				{
					Timeout = 200000 // 200 seconds
				};

				client.Send(message);

				return true;
			}
			catch (Exception e)
			{
				exception = e;
				return false;
			}
		}
	}
}