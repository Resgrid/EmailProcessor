using Microsoft.AspNet.SignalR.Client;
using Resgrid.ApiClient.V3;
using Resgrid.ApiClient.V3.Models;
using Serilog.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core.Model
{
	public class DepartmentDataProvider
	{
		private HubConnection connection;
		private IHubProxy hub;
		public Department DepartmentInfo { get; set; }
		public DepartmentOption Options { get; set; }
		public List<CallPriority> CallPriorities { get; set; }
		public List<Unit> Units { get; set; }

		public DepartmentDataProvider(Department department)
		{
			DepartmentInfo = department;
		}

		public async Task<bool> GetData(bool getInfoToo)
		{
			var options = await DepartmentApi.GetOptions(DepartmentInfo.Id);
			Options = options.FirstOrDefault();

			if (getInfoToo)
			{
				var info = await DepartmentApi.Get(DepartmentInfo.Id);
				DepartmentInfo = info.FirstOrDefault();
			}

			CallPriorities = await CallPrioritiesApi.GetAllCallPriorites(DepartmentInfo.Id);
			Units = await UnitsApi.GetUnitsForDepartment(DepartmentInfo.Id);

			return true;
		}

		public void Start(Config config, Logger log)
		{
			connection = new HubConnection(config.ApiUrl + config.SignalRChannel);
			hub = connection.CreateHubProxy(config.SignalRHub);

			connection.Start().ContinueWith(task =>
			{
				if (task.IsFaulted)
				{
					log.Error(task.Exception.GetBaseException(), $"Error Starting Signalr Monitoring for department: {DepartmentInfo.Id}");
				}
				else
				{
					log.Information($"Started Monitoring for Department: {DepartmentInfo.Id}");
				}

			}).Wait();

			hub.Invoke<string>("connect", DepartmentInfo.Id).ContinueWith(task =>
			{
				if (task.IsFaulted)
				{
					log.Error(task.Exception.GetBaseException(), $"Error Connecting to Resgrid Signalr Hub for department: {DepartmentInfo.Id}");
				}
				else
				{
					log.Information($"Connected to Resgrid Signalr hub for Department: {DepartmentInfo.Id}");
				}
			}).Wait();

			hub.On<string>("departmentUpdated", async param =>
			{
				log.Information($"Received update event for Department: {DepartmentInfo.Id}");
				await GetData(true);
			});

			//myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();


			connection.Stop();
		}

		public void Stop()
		{
			if (connection != null)
				connection.Stop();

			hub = null;
			connection = null;
		}
	}
}
