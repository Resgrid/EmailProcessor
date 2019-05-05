using Consolas.Core;
using Consolas.Mustache;
using Resgrid.EmailProcessor.Core;
using Resgrid.EmailProcessor.Models;
using Serilog;
using Serilog.Core;
using SimpleInjector;

namespace Resgrid.EmailProcessor
{
	public class Program : ConsoleApp<Program>
	{
		static void Main(string[] args)
		{
			Match(args);
		}

		public override void Configure(Container container)
		{
			Logger log = new LoggerConfiguration()
					.MinimumLevel.Error()
					.WriteTo.Console()
					.CreateLogger();

			container.Register<IConsole, SystemConsole>();
			container.Register<IThreadService, ThreadService>();

			container.Register<Logger>(() => log, Lifestyle.Singleton);
			container.Register<IConfigService, ConfigService>();
			container.Register<IFileService, FileService>();

			ViewEngines.Add<MustacheViewEngine>();
		}
	}
}
