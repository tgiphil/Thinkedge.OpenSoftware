using System;
using System.Net.Mail;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Process
{
	public class SendEMail : BaseStandardResult
	{
		public static StandardResult<bool> Execute(MailMessage message)
		{
			return new SendEMail().ExecuteEx(message);
		}

		internal StandardResult<bool> ExecuteEx(MailMessage message)
		{
			if (!MailHelper.SendMailMessage(message, out Exception e))
			{
				return ReturnError<bool>("unable to send e-mail to: " + message.To + "\nResult: " + e.ToString(), e);
			}

			return ReturnResult<bool>(true);
		}
	}
}