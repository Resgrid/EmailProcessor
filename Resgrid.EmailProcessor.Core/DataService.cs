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
		Task<bool> Run(CancellationToken token, Logger log, Config config);
		DepartmentDataProvider GetDataProviderByEmailCode(string emailCode);
	}

	public class DataService: IDataService
	{
		private Config _config;
		private Logger _log;
		private Dictionary<int, DepartmentDataProvider> _data;

		public DataService(Config config)
		{
			_data = new Dictionary<int, DepartmentDataProvider>();
		}

		public async Task<bool> Run(CancellationToken token, Logger log, Config config)
		{
			_log = log;
			_config = config;

			ResgridV3ApiClient.Init(_config.ApiUrl, _config.Username, _config.Password);

			var departments = await DepartmentApi.Get(_config.CacheDepartmentId);

			foreach (var department in departments)
			{
				var departmentDataProvider = new DepartmentDataProvider(department);
				await departmentDataProvider.GetData(false);
				departmentDataProvider.Start(_config, log);
				_data.Add(department.Id, departmentDataProvider);
			}
			
			while (!token.IsCancellationRequested)
			{
				Thread.Sleep(500);
			}

			foreach (var item in _data)
			{
				item.Value.Stop();
			}

			return true;
		}

		public DepartmentDataProvider GetDataProviderByEmailCode(string emailCode)
		{
			foreach (var items in _data)
			{
				if (items.Value != null && items.Value.DepartmentInfo != null)
				{
					if (items.Value.DepartmentInfo.EmailCode == emailCode)
						return items.Value;
				}
			}

			return null;
		}
	}
}
