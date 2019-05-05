using Consolas.Core;
using Resgrid.EmailProcessor.Args;
using Resgrid.EmailProcessor.Core;
using Resgrid.EmailProcessor.Models;

namespace Resgrid.EmailProcessor.Commands
{

	public class TestCommand : Command
	{
		private readonly IConfigService _configService;
		private readonly IFileService _fileService;

		public TestCommand(IConfigService configService, IFileService fileService)
		{
			_configService = configService;
			_fileService = fileService;
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
			model.DirectoryAvailable = _fileService.DoesDirectoryExist("emails");



			return View("Help", model);
		}
	}
}
