using Consolas2.Core;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Resgrid.EmailProcessor.Args;
using Resgrid.EmailProcessor.Core;
using Resgrid.EmailProcessor.Models;
using Serilog;
using Serilog.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Commands
{
	public class RunCommand : Command
	{
		private readonly IConfigService _configService;
		private readonly IEmailService _emailService;
		private readonly IMontiorService _montiorService;

		public RunCommand(IConfigService configService, IEmailService emailService, IMontiorService montiorService)
		{
			_configService = configService;
			_emailService = emailService;
			_montiorService = montiorService;
		}

		public object Execute(RunArgs args)
		{
			Console.WriteLine("Resgrid Email Processor");
			Console.WriteLine("-----------------------------------------");

			Console.WriteLine("Starting Up");

			var _running = true;
			var model = new RunViewModel();

			var config = _configService.LoadSettingsFromFile();

			TelemetryConfiguration configuration = null;
			TelemetryClient telemetryClient = null;

			if (!String.IsNullOrWhiteSpace(config.DebugKey))
			{
				try
				{
					configuration = TelemetryConfiguration.Active;
					configuration.InstrumentationKey = config.DebugKey;
					configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
					configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());
					telemetryClient = new TelemetryClient();

					System.Console.WriteLine("Application Insights Debug Key Detected and AppInsights Initialized");
				}
				catch { }
			}

			Logger log;

			if (config.Debug)
			{
				log = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.WriteTo.Console()
					.CreateLogger();
			}
			else
			{
				log = new LoggerConfiguration()
					.MinimumLevel.Error()
					.WriteTo.Console()
					.CreateLogger();
			}

			using (InitializeDependencyTracking(configuration))
			{
				// Define the cancellation token.
				CancellationTokenSource source = new CancellationTokenSource();
				CancellationToken token = source.Token;

				// Create the specified number of clients, to carry out test operations, each on their own threads
				Thread emailThread = new Thread(() => _emailService.Run(token, log));
				emailThread.Name = $"Email Service Thread";
				emailThread.Start();

				Thread importThread = new Thread(() => _montiorService.Run(token, log));
				importThread.Name = $"Import Service Thread";
				importThread.Start();

				System.Console.WriteLine("Email Processor is Running...");
				System.Console.WriteLine("Press any key to stop");

				var line = Console.ReadLine();
				source.Cancel();
				System.Console.WriteLine("Shutting down, please wait...");
				Task.Delay(2000).Wait();

				_running = false;

			}

			if (telemetryClient != null)
			{
				telemetryClient.Flush();
				Task.Delay(5000).Wait();
			}

			return View("Run", model);
		}

		private static DependencyTrackingTelemetryModule InitializeDependencyTracking(TelemetryConfiguration configuration)
		{
			var module = new DependencyTrackingTelemetryModule();

			if (configuration != null)
			{
				// prevent Correlation Id to be sent to certain endpoints. You may add other domains as needed.
				module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.windows.net");
				module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.chinacloudapi.cn");
				module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.cloudapi.de");
				module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.usgovcloudapi.net");
				module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("localhost");
				module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("127.0.0.1");

				// enable known dependency tracking, note that in future versions, we will extend this list. 
				// please check default settings in https://github.com/Microsoft/ApplicationInsights-dotnet-server/blob/develop/Src/DependencyCollector/NuGet/ApplicationInsights.config.install.xdt#L20
				module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.ServiceBus");
				module.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.EventHubs");

				// initialize the module
				module.Initialize(configuration);
			}

			return module;
		}
	}
}
