using Consolas.Core;
using Resgrid.EmailProcessor.Args;
using Resgrid.EmailProcessor.Core;
using Resgrid.EmailProcessor.Models;
using System;
using System.Threading;

namespace Resgrid.EmailProcessor.Commands
{
	public class RunCommand : Command
	{
		private readonly IConfigService _configService;
		private readonly IEmailService _emailService;
		private readonly IImportService _importService;

		public RunCommand(IConfigService configService, IEmailService emailService, IImportService importService)
		{
			_configService = configService;
			_emailService = emailService;
			_importService = importService;
		}

		public object Execute(RunArgs args)
		{
			var model = new RunViewModel();
			var barrier = new Barrier(3);

			// Create the specified number of clients, to carry out test operations, each on their own threads
			Thread emailThread = new Thread(_emailService.Run);
			emailThread.Name = $"Email Service Thread";
			emailThread.Start();

			Thread importThread = new Thread(_importService.Run);
			importThread.Name = $"Import Service Thread";
			importThread.Start();
			

			barrier.SignalAndWait();

			return View("Run", model);
		}
	}
}
