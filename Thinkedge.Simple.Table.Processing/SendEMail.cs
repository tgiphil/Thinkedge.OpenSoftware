using System;
using System.Net.Mail;
using Thinkedge.Common;

namespace Thinkedge.Simple.Table.Processing
{
	internal class SendEMail : BaseStandardResult
	{
		internal bool Execute(MailMessage message)
		{
			if (!MailHelper.SendMailMessage(message, out Exception e))
			{
				SetError("unable to send e-mail to: " + message.To, e);
				return false;
			}

			return true;
		}
	}
}