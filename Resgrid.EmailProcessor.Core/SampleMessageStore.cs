using MimeKit;
using Newtonsoft.Json;
using Resgrid.EmailProcessor.Core.Model;
using Serilog;
using Serilog.Core;
using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public class SampleMessageStore : MessageStore
	{
		private static IConfigService _configService;
		private static IFileService _fileService;
		private static Logger _logger;
		private static Config _config;

		/// <summary>
		/// Save the given message to the underlying storage system.
		/// </summary>
		/// <param name="context">The session context.</param>
		/// <param name="transaction">The SMTP message transaction to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A unique identifier that represents this message in the underlying message store.</returns>
		public override Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, CancellationToken cancellationToken)
		{
			Init();

			var textMessage = (ITextMessage)transaction.Message;
			MimeMessage mailMessage = MimeMessage.Load(textMessage.Content);

			Message message = new Message();
			message.Id = Guid.NewGuid();
			message.RetryCount = 0;
			message.ErrorReason = "";
			message.Timestamp = DateTime.UtcNow;
			
			var inboundMessage = new InboundMessage();

			if (mailMessage.From != null && mailMessage.From.Count > 0)
			{
				var from = ((MailboxAddress)mailMessage.From[0]);
				inboundMessage.From = from.Address;
				inboundMessage.FromFull = new FromFull() { Email = from.Address, Name = from.Name };
			}

			if (mailMessage.To != null && mailMessage.To.Count > 0)
			{
				foreach (var to in mailMessage.To)
				{
					var toAddress = (MailboxAddress)to;

					if (String.IsNullOrWhiteSpace(inboundMessage.To))
						inboundMessage.To = toAddress.Address;

					inboundMessage.ToFull.Add(new ToFull() { Email = toAddress.Address, Name = toAddress.Name });
				}
			}

			var attachments = new List<MimePart>();
			var multiparts = new List<Multipart>();
			var iter = new MimeIterator(mailMessage);

			// collect our list of attachments and their parent multiparts
			while (iter.MoveNext())
			{
				var multipart = iter.Parent as Multipart;
				var part = iter.Current as MimePart;

				if (multipart != null && part != null && part.IsAttachment)
				{
					// keep track of each attachment's parent multipart
					multiparts.Add(multipart);
					attachments.Add(part);
				}
			}

			// now remove each attachment from its parent multipart...
			for (int i = 0; i < attachments.Count; i++)
				multiparts[i].Remove(attachments[i]);

			inboundMessage.TextBody = mailMessage.GetTextBody(MimeKit.Text.TextFormat.Plain);
			inboundMessage.HtmlBody = mailMessage.HtmlBody;
			inboundMessage.Subject = mailMessage.Subject;
			inboundMessage.MessageID = mailMessage.MessageId;

			inboundMessage.Attachments = new List<Attachment>();
			foreach (var attachment in attachments)
			{
				var att = new Attachment();

				StreamReader reader = new StreamReader(attachment.Content.Stream);
				att.Content = reader.ReadToEnd();

				att.Name = attachment.FileName;
				att.ContentID = attachment.ContentId;
				att.ContentType = attachment.ContentType.MimeType;

				inboundMessage.Attachments.Add(att);
			}

			message.InboundMessage = inboundMessage;

			var fileText = JsonConvert.SerializeObject(message);
			var filePath = _fileService.CreateFile($"{message.Id.ToString()}.rgm", "emails", fileText);

			if (!String.IsNullOrWhiteSpace(filePath))
				return Task.FromResult(SmtpResponse.Ok);
			else
				return Task.FromResult(new SmtpResponse(SmtpReplyCode.MessageTimeout));
		}

		private void Init()
		{
			if (_configService == null)
				_configService = new ConfigService();

			if (_config == null)
				_config = _configService.LoadSettingsFromFile();

			if (_logger != null)
			{
				if (_config.Debug)
				{
					_logger = new LoggerConfiguration()
						.MinimumLevel.Debug()
						.WriteTo.Console()
						.CreateLogger();
				}
				else
				{
					_logger = new LoggerConfiguration()
						.MinimumLevel.Error()
						.WriteTo.Console()
						.CreateLogger();
				}
			}

			if (_fileService == null)
				_fileService = new FileService(_logger);
		}
	}
}
