using Newtonsoft.Json;
using Resgrid.ApiClient.V3;
using Resgrid.ApiClient.V3.Models;
using Resgrid.EmailProcessor.Core.Model;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public interface IImportService
	{
		Task<bool> CreateCall(Model.Message message);
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

		public async Task<bool> CreateCall(Model.Message message)
		{
			var inboundMessage = new InboundMessage();


			return false;
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
