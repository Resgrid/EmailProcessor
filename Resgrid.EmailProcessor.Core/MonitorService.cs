using Newtonsoft.Json;
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
		void Run(CancellationToken token);
	}

	public class MonitorService : IMontiorService
	{
		private readonly IFileService _fileService;
		private readonly IImportService _importService;

		private System.Timers.Timer _timer;
		private HashSet<string> _files;

		public MonitorService(IFileService fileService, IImportService importService)
		{
			_fileService = fileService;
			_importService = importService;

			_files = new HashSet<string>();
		}

		public void Run(CancellationToken token)
		{
			var watcher = new System.IO.FileSystemWatcher();
			watcher.Path = _fileService.GetFullPath("emails");
			watcher.Filter = "*.rgm";
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Changed += Watcher_Changed;

			watcher.EnableRaisingEvents = true;

			while (!token.IsCancellationRequested)
			{
				Thread.Sleep(500);
			}

			watcher.EnableRaisingEvents = false;
			watcher.Changed -= Watcher_Changed;
			watcher.Dispose();
			watcher = null;
		}

		private void CreateTimer()
		{
			_timer = new System.Timers.Timer(250);
			_timer.Elapsed += OnTimedEvent;
			_timer.AutoReset = true;
			_timer.Enabled = true;
		}

		private void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (new FileInfo(e.FullPath).Length > 0)
			{
				if (!_files.Contains(Path.GetFileName(e.FullPath)))
				{
					_files.Add(Path.GetFileName(e.FullPath));

					var newPath = Path.ChangeExtension(e.FullPath, ".rgi");
					File.Move(e.FullPath, newPath);
					var message = JsonConvert.DeserializeObject<Model.Message>(File.ReadAllText(newPath));

					try
					{
						var result = _importService.CreateCall(message).Result;

						if (result)
							File.Move(newPath, Path.ChangeExtension(e.FullPath, ".rgc"));
					}
					catch (Exception ex)
					{

					}

					File.Move(newPath, Path.ChangeExtension(e.FullPath, ".rgm"));
					return;
				}
			}
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
