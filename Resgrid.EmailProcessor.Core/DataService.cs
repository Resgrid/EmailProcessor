using Resgrid.ApiClient.V3;
using Resgrid.EmailProcessor.Core.Model;
using Serilog.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public interface IDataService
	{
		Task<bool> Run(CancellationToken token, Logger log);
	}

	public class DataService: IDataService
	{
		private Config _config;
		private Logger _log;
		private Dictionary<int, DepartmentData> _data;

		DataService(Config config)
		{
			_data = new Dictionary<int, DepartmentData>();
			_config = config;
		}

		public async Task<bool> Run(CancellationToken token, Logger log)
		{
			_log = log;

			ResgridV3ApiClient.Init(_config.ApiUrl, _config.Username, _config.Password);

			var departments = await DepartmentApi.Get(_config.CacheDepartmentId);




			while (!token.IsCancellationRequested)
			{
				Thread.Sleep(500);
			}

			return true;
		}
	}
}
