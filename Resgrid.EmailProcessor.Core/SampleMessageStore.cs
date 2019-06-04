using MimeKit;
using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public class SampleMessageStore : MessageStore
	{
		/// <summary>
		/// Save the given message to the underlying storage system.
		/// </summary>
		/// <param name="context">The session context.</param>
		/// <param name="transaction">The SMTP message transaction to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A unique identifier that represents this message in the underlying message store.</returns>
		public override Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, CancellationToken cancellationToken)
		{
			var textMessage = (ITextMessage)transaction.Message;
			MimeMessage mailMessage = MimeMessage.Load(textMessage.Content);

			

			return Task.FromResult(SmtpResponse.Ok);
		}
	}
}
