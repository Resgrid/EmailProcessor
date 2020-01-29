using Resgrid.ApiClient.V3;
using Resgrid.ApiClient.V3.Models;
using Resgrid.EmailProcessor.Core.Helpers;
using Resgrid.EmailProcessor.Core.Model;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public interface IImportService
	{
		Tuple<int, string> DetermineEmailTypeAndEmail(Message message);
		Task<bool> CreateCall(Tuple<int, string> emailType, Message message);
		Task<bool> ImportMessage(Message message);
	}

	public class ImportService : IImportService
	{
		private Config _config;
		private readonly IConfigService _configService;
		private readonly Logger _log;
		private readonly IDataService _dataService;
		private readonly ICallsService _callsService;

		public ImportService(IConfigService configService, IDataService dataService, Logger log, ICallsService callsService)
		{
			_configService = configService;
			_log = log;
			_dataService = dataService;
			_callsService = callsService;
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
			_log.Information($"ImportService::Determining email type for: {message.Id}||{message.InboundMessage.MessageID}");

			var emailType = DetermineEmailTypeAndEmail(message);
			_log.Information($"ImportService::File Type for email: {message.Id}||{message.InboundMessage.MessageID} type:{emailType.Item1} code:{emailType.Item2}");

			if (emailType.Item1 == 1) // Call
			{
				return await CreateCall(emailType, message);
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

		public async Task<bool> CreateCall(Tuple<int, string> emailType, Message message)
		{
			_log.Information($"ImportService::Creating call for email: {message.Id}||{message.InboundMessage.MessageID}");

			var departmentData = _dataService.GetDataProviderByEmailCode(emailType.Item2);

			if (departmentData != null)
			{
				try
				{
					int emailFormatType = 0;

					if (departmentData.Options == null || departmentData.Options.EmailFormatType < 0)
					{
						departmentData.Options.EmailFormatType = (int)CallEmailTypes.Generic;
					}
					else
					{
						emailFormatType = departmentData.Options.EmailFormatType;
					}

					List<Call> activeCalls = new List<Call>();
					if (CallTypesThatNeedActiveCalls.CallTypes.Contains(emailFormatType))
					{
						activeCalls = await CallsApi.GetActiveCalls(departmentData.DepartmentInfo.Id);
					}

					int defaultPriority = (int)ApiClient.Common.CallPriority.High;

					if (departmentData.CallPriorities != null && departmentData.CallPriorities.Any())
					{
						var defaultPrio = departmentData.CallPriorities.FirstOrDefault(x => x.IsDefault && x.IsDeleted == false);

						if (defaultPrio != null)
							defaultPriority = defaultPrio.Id;
					}

					var call = _callsService.GenerateCallFromEmail((CallEmailTypes)emailFormatType, message.InboundMessage,
																	 departmentData.DepartmentInfo.ManagingUserId,
																	 departmentData.DepartmentInfo.Members, departmentData.DepartmentInfo, activeCalls, departmentData.Units, defaultPriority);

					if (call != null)
					{
						call.DepartmentId = departmentData.DepartmentInfo.Id;

						var savedCall = await CallsApi.AddNewCall(call);
					}
				}
				catch (Exception ex)
				{
					_log.Error(ex, $"ImportService::Exception attempting to create call for email: {message.Id}||{message.InboundMessage.MessageID}");
				}
			}

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
