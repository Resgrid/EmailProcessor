using Consolas.Core;
using Consolas.Mustache;
using Resgrid.EmailProcessor.Core;
using Resgrid.EmailProcessor.Models;
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
			container.Register<IConsole, SystemConsole>();
			container.Register<IThreadService, ThreadService>();

			container.Register<IConfigService, ConfigService>();


			ViewEngines.Add<MustacheViewEngine>();
		}
	}
}
