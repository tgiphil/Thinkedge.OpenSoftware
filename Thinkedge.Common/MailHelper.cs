using System;
using System.Net.Mail;

namespace Thinkedge.Common
{
	public static class MailHelper
	{
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