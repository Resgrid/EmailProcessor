using System.IO;

namespace Resgrid.EmailProcessor.Core
{
	public static class FileHelper
	{
		public static bool DoesDirectoryExist(string name)
		{
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");

			return Directory.Exists($"{path}\\{name}\\");
		}
	}
}
