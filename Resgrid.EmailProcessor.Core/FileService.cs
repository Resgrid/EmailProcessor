using Serilog.Core;
using System;
using System.IO;
using System.Text;

namespace Resgrid.EmailProcessor.Core
{
	public interface IFileService
	{
		bool DoesDirectoryExist(string name);
		string CreateFile(string fileName, string directory, string text);
	}

	public class FileService: IFileService
	{
		private readonly Logger _logger;

		public FileService(Logger logger)
		{
			_logger = logger;
		}

		public bool DoesDirectoryExist(string name)
		{
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");

			return Directory.Exists($"{path}\\{name}\\");
		}

		public string CreateFile(string fileName, string directory, string text)
		{
			try
			{
				var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");

				if (File.Exists($"{path}\\{directory}\\{fileName}"))
				{
					return null;
				}
   
				using (FileStream fs = File.Create($"{path}\\{directory}\\{fileName}"))
				{
					Byte[] body = new UTF8Encoding(true).GetBytes(text);
					fs.Write(body, 0, body.Length);
				}

				return $"{path}\\{directory}\\{fileName}";
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());

				return null;
			}
		}
	}
}
