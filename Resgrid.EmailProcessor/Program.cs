using Consolas2.Core;
using Consolas2.ViewEngines;
using Newtonsoft.Json;
using Resgrid.EmailProcessor.Core;
using Resgrid.EmailProcessor.Core.Model;
using Resgrid.EmailProcessor.Models;
using Serilog;
using Serilog.Core;
using SimpleInjector;
using System;
using System.IO;

namespace Resgrid.EmailProcessor
{
	public class Program : ConsoleApp<Program>
	{
		static void Main(string[] args)
		{
			Match(args);

			Console.Write("Processor has finished, press Enter to exit...");
			Console.ReadLine();
		}

		public override void Configure(Container container)
		{
			Logger log;

			/* Ugly I know, but trying to keep this as low level as possible. If the config
			 * doesn't load just log errors. Won't use the actual config service. 
			 */
			try
			{
				Config config = LoadSettingsFromFile();

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
			}
			catch (Exception ex)
			{
				log = new LoggerConfiguration()
					.MinimumLevel.Error()
					.WriteTo.Console()
					.CreateLogger();
			}


			container.Register<IConsole, SystemConsole>();
			container.Register<IThreadService, ThreadService>();

			container.Register<Logger>(() => log, Lifestyle.Singleton);
			container.Register<IConfigService, ConfigService>();
			container.Register<IFileService, FileService>();
			container.Register<INetworkService, NetworkService>();
			container.Register<IEmailService, EmailService>();
			container.Register<IImportService, ImportService>();
			container.Register<IMontiorService, MonitorService>();

			ViewEngines.Add<StubbleViewEngine>();
		}

		private static Config LoadSettingsFromFile()
		{
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");

			try
			{
				Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText($"{path}\\settings.json"));

				return config;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}
