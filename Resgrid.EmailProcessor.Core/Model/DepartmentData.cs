using Microsoft.AspNet.SignalR.Client;
using Resgrid.ApiClient.V3.Models;
using System.Collections.Generic;

namespace Resgrid.EmailProcessor.Core.Model
{
	public class DepartmentData
	{
		public Department DepartmentInfo { get; set; }
		public DepartmentOption Options { get; set; }
		public List<CallPriority> CallPriorities { get; set; }
		public List<Unit> Units { get; set; }

		public void Setup(Config config)
		{
			var connection = new HubConnection(config.ApiUrl + config.SignalRChannel);
			var hub = connection.CreateHubProxy(config.SignalRHub);

			connection.Start().ContinueWith(task =>
			{
				if (task.IsFaulted)
				{
					//Console.WriteLine("There was an error opening the connection:{0}",
					//				  task.Exception.GetBaseException());
				}
				else
				{
					//Console.WriteLine("Connected");
				}

			}).Wait();

			hub.Invoke<string>("connect", DepartmentInfo.Id).ContinueWith(task =>
			{
				if (task.IsFaulted)
				{
					//Console.WriteLine("There was an error calling send: {0}",
					//				  task.Exception.GetBaseException());
				}
				else
				{
					//Console.WriteLine(task.Result);
				}
			}).Wait();

			hub.On<string>("departmentUpdated", param =>
			{
				//Console.WriteLine(param);
			});

			//myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();


			//Console.Read();
			connection.Stop();
		}
	}
}
