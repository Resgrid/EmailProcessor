using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;

namespace Resgrid.EmailProcessor.Core
{
	public interface IMontiorService
	{
		void Run(CancellationToken token);
	}

	public class MonitorService: IMontiorService
	{
		private readonly IFileService _fileService;
		private readonly IImportService _importService;

		public MonitorService(IFileService fileService, IImportService importService)
		{
			_fileService = fileService;
			_importService = importService;
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

		private void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (new FileInfo(e.FullPath).Length > 0)
			{
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
}
