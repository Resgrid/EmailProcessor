using Newtonsoft.Json;
using Resgrid.EmailProcessor.Core.Model;
using System;
using System.IO;

namespace Resgrid.EmailProcessor.Core
{
	public interface IConfigService
	{
		Config LoadSettingsFromFile();
	}

	public class ConfigService: IConfigService
	{
		public Config LoadSettingsFromFile()
		{
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");

			try
			{
				Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText($"{path}\\settings.json"));

				return config;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return null;
			}
		}
	}
}
