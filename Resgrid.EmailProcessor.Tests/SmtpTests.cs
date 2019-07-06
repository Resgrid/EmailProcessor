using NUnit.Framework;
using System;
using System.Net.Mail;

namespace Resgrid.EmailProcessor.Tests
{
	[TestFixture]
	public class SmtpTests
	{
		[Ignore("")]
		[Test]
		public void SendTestEmail()
		{
			SmtpClient client = new SmtpClient();
			client.Host = "";
			client.Port = 81;

			//create sender address
			MailAddress from = new MailAddress("cad@dispatch.com", "Dispatch");

			//create receiver address
			MailAddress receiver = new MailAddress("xxxxx@testdispatch.resgrid.com", "Resgrid");

			MailMessage message = new MailMessage(from, receiver);
			message.Subject = "";
			message.Body = "";

			client.Send(message);
		}
	}
}
