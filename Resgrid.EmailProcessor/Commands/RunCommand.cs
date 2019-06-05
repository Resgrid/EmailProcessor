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
		private readonly IMontiorService _montiorService;

		public RunCommand(IConfigService configService, IEmailService emailService, IMontiorService montiorService)
		{
			_configService = configService;
			_emailService = emailService;
			_montiorService = montiorService;
		}

		public object Execute(RunArgs args)
		{
			var _running = false;
			var model = new RunViewModel();

			// Define the cancellation token.
			CancellationTokenSource source = new CancellationTokenSource();
			CancellationToken token = source.Token;
			
			// Create the specified number of clients, to carry out test operations, each on their own threads
			Thread emailThread = new Thread(() => _emailService.Run(token));
			emailThread.Name = $"Email Service Thread";
			emailThread.Start();

			Thread importThread = new Thread(() => _montiorService.Run(token));
			importThread.Name = $"Import Service Thread";
			importThread.Start();


			while (_running)
			{
				var line = Console.ReadLine();
				source.Cancel();
				_running = false;
			}

			return View("Run", model);
		}
	}
}
