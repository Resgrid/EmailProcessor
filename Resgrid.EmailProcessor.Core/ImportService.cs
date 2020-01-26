using Resgrid.ApiClient.V3;
using Resgrid.EmailProcessor.Core.Helpers;
using Resgrid.EmailProcessor.Core.Model;
using Serilog.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public interface IImportService
	{
		Tuple<int, string> DetermineEmailTypeAndEmail(Message message);
		Task<bool> CreateCall(Message message);
		Task<bool> ImportMessage(Message message);
	}

	public class ImportService : IImportService
	{
		private Config _config;
		private readonly IConfigService _configService;
		private readonly Logger _logger;

		public ImportService(IConfigService configService, Logger logger)
		{
			_configService = configService;
			_logger = logger;
		}

		private void Init()
		{
			if (_config == null)
			{
				_config = _configService.LoadSettingsFromFile();

				ResgridV3ApiClient.Init(_config.ApiUrl, _config.Username, _config.Password);
			}
		}

		public async Task<bool> ImportMessage(Message message)
		{
			var emailType = DetermineEmailTypeAndEmail(message);

			if (emailType.Item1 == 1) // Call
			{
				return await CreateCall(message);
			}
			else if (emailType.Item1 == 2) // Email List
			{

			}
			else if (emailType.Item1 == 3)  // Group Dispatch
			{

			}
			else if (emailType.Item1 == 4)  // Group Message
			{

			}

			return false;
		}

		public async Task<bool> CreateCall(Message message)
		{
			var inboundMessage = new InboundMessage();

			

			

			return false;
		}

		public Tuple<int, string> DetermineEmailTypeAndEmail(Message message)
		{
			foreach (var email in message.InboundMessage.ToFull)
			{
				if (StringHelpers.ValidateEmail(email.Email))
				{
					var proccedEmailInfo = ProcessEmailAddress(email.Email);

					if (proccedEmailInfo != null && proccedEmailInfo.Item1 > 0)
						return proccedEmailInfo;
				}
			}

			foreach (var email in message.InboundMessage.CcFull)
			{
				if (StringHelpers.ValidateEmail(email.Email))
				{
					var proccedEmailInfo = ProcessEmailAddress(email.Email);

					if (proccedEmailInfo != null && proccedEmailInfo.Item1 > 0)
						return proccedEmailInfo;
				}
			}

			if (message.InboundMessage.Headers != null && message.InboundMessage.Headers.Count > 0)
			{
				var header = message.InboundMessage.Headers.FirstOrDefault(x => x.Name == "Received-SPF");

				if (header != null)
				{
					var lastValue = header.Value.LastIndexOf(char.Parse("="));
					var newEmail = header.Value.Substring(lastValue + 1, (header.Value.Length - (lastValue + 1)));

					newEmail = newEmail.Trim();

					if (StringHelpers.ValidateEmail(newEmail))
					{
						var proccedEmailInfo = ProcessEmailAddress(newEmail);

						if (proccedEmailInfo != null && proccedEmailInfo.Item1 > 0)
							return proccedEmailInfo;
					}
				}
			}

			return null;
		}

		private Tuple<int, string> ProcessEmailAddress(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				return new Tuple<int, string>(0, String.Empty);

			int type = 0;
			string emailAddress = String.Empty;

			if (email.Contains($"@{_config.DispatchDomain}") || email.Contains($"@{_config.DispatchTestDomain}"))
			{
				type = 1;

				if (email.Contains($"@{_config.DispatchDomain}"))
					emailAddress = email.Replace($"@{_config.DispatchDomain}", "").Trim();
				else
					emailAddress = email.Replace($"@{_config.DispatchTestDomain}", "").Trim();
			}
			else if (email.Contains($"@{_config.ListsDomain}"))
			{
				type = 2;
				emailAddress = email.Replace($"@{_config.ListsDomain}", "").Trim();
			}
			else if (email.Contains($"@{_config.GroupsDomain}") || email.Contains($"@{_config.GroupsTestDomain}"))
			{
				type = 3;

				if (email.Contains($"@{_config.GroupsDomain}"))
					emailAddress = email.Replace($"@{_config.GroupsDomain}", "").Trim();
				else
					emailAddress = email.Replace($"@{_config.GroupsTestDomain}", "").Trim();
			}

			return new Tuple<int, string>(type, emailAddress);
		}
	}
}
