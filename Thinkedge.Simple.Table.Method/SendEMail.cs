using System;
using System.Net.Mail;
using Thinkedge.Common;
using Thinkedge.Common.Result;

namespace Thinkedge.Simple.Table.Method
{
	public static class SendEMail
	{
		public static StandardResult<bool> Execute(MailMessage message)
		{
			if (!MailHelper.SendMailMessage(message, out Exception e))
				return StandardResult<bool>.ReturnError("SendEMail() error: unable to send e-mail to: " + message.To, "Reason: " + e.ToString(), e);

			return StandardResult<bool>.ReturnResult(true);
		}
	}
}