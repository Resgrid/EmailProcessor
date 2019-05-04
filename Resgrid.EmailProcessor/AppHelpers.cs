using Newtonsoft.Json;
using Resgrid.EmailProcessor.Core.Model;
using System.IO;

namespace Resgrid.EmailProcessor
{
	public static class AppHelpers
	{
		public static Core.Model.Config LoadSettingsFromFile()
		{
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");
			Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText($"{path}\\settings.json"));

			return config;
		}

		public static void CreateMessagesDirectory()
		{
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");
			Directory.CreateDirectory($"{path}\\Messages\\");
		}
	}
}
