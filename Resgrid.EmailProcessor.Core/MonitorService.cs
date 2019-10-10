using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Resgrid.EmailProcessor.Core
{
	public interface IMontiorService
	{
		void Run(CancellationToken token, Logger log);
	}

	public class MonitorService : IMontiorService
	{
		private readonly IFileService _fileService;
		private readonly IImportService _importService;

		private Logger _log;
		private System.Timers.Timer _timer;
		private HashSet<string> _files;

		public MonitorService(IFileService fileService, IImportService importService)
		{
			_fileService = fileService;
			_importService = importService;

			_files = new HashSet<string>();
		}

		public void Run(CancellationToken token, Logger log)
		{
			_log = log;

			CreateTimer();

			while (!token.IsCancellationRequested)
			{
				Thread.Sleep(500);
			}
		}

		private void CreateTimer()
		{
			_timer = new System.Timers.Timer(250);
			_timer.Elapsed += OnTimedEvent;
			_timer.AutoReset = true;
			_timer.Enabled = true;
		}

		private void OnTimedEvent(Object source, ElapsedEventArgs e)
		{
			var path = _fileService.GetFullPath("emails");
			var files = Directory.GetFiles(path, "*.rgm", SearchOption.AllDirectories);

			Parallel.ForEach(files, file =>
			{
				var newPath = Path.ChangeExtension(file, ".rgi");
				File.Move(file, newPath);
				var message = JsonConvert.DeserializeObject<Model.Message>(File.ReadAllText(newPath));

				try
				{
					var result = _importService.CreateCall(message).Result;

					if (result)
						File.Move(newPath, Path.ChangeExtension(file, ".rgc"));
				}
				catch (Exception ex)
				{

				}

				File.Move(newPath, Path.ChangeExtension(file, ".rgm"));
			});


			var importFiles = Directory.GetFiles(path, "*.rgi", SearchOption.AllDirectories);

			foreach (var file in importFiles)
			{
				if (!_fileService.IsFileLocked(new FileInfo(file)))
				{
					var message = JsonConvert.DeserializeObject<Model.Message>(File.ReadAllText(file));

					try
					{
						var result = _importService.CreateCall(message).Result;

						if (result)
							File.Move(file, Path.ChangeExtension(file, ".rgc"));
					}
					catch (Exception ex)
					{

					}

					File.Move(file, Path.ChangeExtension(file, ".rgm"));
				}
			}
		}
	}
}
