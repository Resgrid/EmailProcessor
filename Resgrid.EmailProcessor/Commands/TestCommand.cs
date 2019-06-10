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
		private readonly INetworkService _networkService;

		public TestCommand(IConfigService configService, IFileService fileService, INetworkService networkService)
		{
			_configService = configService;
			_fileService = fileService;
			_networkService = networkService;
		}

		public object Execute(TestArgs args)
		{
			Console.WriteLine("Resgrid Email Processor");
			Console.WriteLine("-----------------------------------------");

			Console.WriteLine("Testing Environment...");

			var model = new TestViewModel();

			Console.WriteLine("Checking Config...");
			var config = _configService.LoadSettingsFromFile();

			if (config != null)
				model.CanLoadConfig = true;
			else
				model.CanLoadConfig = false;

			Console.WriteLine("Checking Directory...");
			model.DirectoryAvailable = _fileService.DoesDirectoryExist("emails");

			if (!model.DirectoryAvailable)
			{
				_fileService.CreateDirectory("emails");
			}

			Console.WriteLine("Checking Directory Permissions...");

			var guid = Guid.NewGuid().ToString();
			var filePath = _fileService.CreateFile($"{guid}.test", "emails", guid);

			if (String.IsNullOrWhiteSpace(filePath))
				model.CanCreateFile = false;
			else
				model.CanCreateFile = true;


			var fileText = _fileService.ReadFile($"{guid}.test", "emails");

			if (!String.IsNullOrWhiteSpace(fileText) && fileText.Trim() == guid)
				model.CanReadFile = true;
			else
				model.CanReadFile = false;

			model.CanDeleteFile = _fileService.DeleteFile(filePath);
			var doesFileExist = _fileService.DoesFileExist(filePath);

			if (doesFileExist)
				model.CanDeleteFile = false;

			Console.WriteLine("Checking Network Ports...");

			model.PortsAvailable = new System.Collections.Generic.List<Tuple<int, bool>>();
			string standardPorts;

			if (config != null)
				standardPorts = config.Ports;
			else
				standardPorts = "25, 587";

			var ports = standardPorts.Split(char.Parse(","));

			foreach (var port in ports)
			{
				var portConverted = int.Parse(port.Trim());
				var available = _networkService.IsPortAvailable(portConverted);

				var result = new Tuple<int, bool>(portConverted, available);
				model.PortsAvailable.Add(result);
			}

			Console.WriteLine("Results:");

			return View("Test", model);
		}
	}
}
