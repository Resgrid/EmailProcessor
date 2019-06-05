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
			message.MailMessage = mailMessage;

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
