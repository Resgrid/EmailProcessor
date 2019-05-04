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
			_configService = configService;
		}

		public object Execute(TestArgs args)
		{
			System.Console.WriteLine("Resgrid Email Processor");
			System.Console.WriteLine("-----------------------------------------");

			System.Console.WriteLine("Testing Enviorment...");

			var model = new TestViewModel();

			System.Console.WriteLine("Checking Config...");
			var config = _configService.LoadSettingsFromFile();

			if (config != null)
				model.CanLoadConfig = false;
			else
				model.CanLoadConfig = true;

			System.Console.WriteLine("Checking Directory...");
			model.DirectoryAvailable = FileHelper.DoesDirectoryExist("emails");

			return View("Help", model);
		}
	}
}
