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
		string ReadFile(string fileName, string directory);
		bool DeleteFile(string path);
		bool DoesFileExist(string path);
		void CreateDirectory(string name);
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

		public void CreateDirectory(string name)
		{
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");

			Directory.CreateDirectory($"{path}\\{name}\\");
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

		public string ReadFile(string fileName, string directory)
		{
			string text;

			try
			{
				var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");

				text = File.ReadAllText($"{path}\\{directory}\\{fileName}");
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());

				return null;
			}

			return text;
		}

		public bool DeleteFile(string path)
		{
			try
			{
				File.Delete(path);

				return true;
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());

				return false;
			}
		}

		public bool DoesFileExist(string path)
		{
			try
			{
				return File.Exists(path);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());

				return false;
			}
		}
	}
}
