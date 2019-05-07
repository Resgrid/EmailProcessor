using Consolas.Core;
using Resgrid.EmailProcessor.Args;
using Resgrid.EmailProcessor.Core;
using Resgrid.EmailProcessor.Models;
using System;

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
			Console.WriteLine("Resgrid Email Processor");
			Console.WriteLine("-----------------------------------------");

			Console.WriteLine("Testing Enviorment...");

			var model = new TestViewModel();

			Console.WriteLine("Checking Config...");
			var config = _configService.LoadSettingsFromFile();

			if (config != null)
				model.CanLoadConfig = false;
			else
				model.CanLoadConfig = true;

			Console.WriteLine("Checking Directory...");
			model.DirectoryAvailable = _fileService.DoesDirectoryExist("emails");

			Console.WriteLine("Checking Directory Permissions...");

			var guid = Guid.NewGuid().ToString();
			var filePath = _fileService.CreateFile($"{guid}.test", "emails", guid);

			if (String.IsNullOrWhiteSpace(filePath))
				model.CanCreateFile = false;
			else
				model.CanCreateFile = true;


			var fileText = _fileService.ReadFile($"{guid}.test");

			if (!String.IsNullOrWhiteSpace(fileText) && fileText.Trim() == guid)
				model.CanReadFile = true;
			else
				model.CanReadFile = false;

			model.CanDeleteFile = _fileService.DeleteFile(filePath);
			var doesFileExist = _fileService.DoesFileExist(filePath);

			if (doesFileExist)
				model.CanDeleteFile = false;

			return View("Help", model);
		}
	}
}
