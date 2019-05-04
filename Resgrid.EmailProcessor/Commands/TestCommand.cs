using Consolas.Core;
using Resgrid.EmailProcessor.Args;
using Resgrid.EmailProcessor.Core;
using Resgrid.EmailProcessor.Models;

namespace Resgrid.EmailProcessor.Commands
{

	public class TestCommand : Command
	{
		private readonly IConfigService _configService;

		public TestCommand(IConfigService configService)
		{

		}

		public object Execute(TestArgs args)
		{
			System.Console.WriteLine("Resgrid Email Processor");
			System.Console.WriteLine("-----------------------------------------");

			System.Console.WriteLine("Testing Enviorment...");



			var model = new TestViewModel();

			return View("Help", model);
		}
	}
}
