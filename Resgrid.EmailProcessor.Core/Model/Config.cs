using System.Collections.Generic;

namespace Resgrid.EmailProcessor.Core.Model
{
	public class Config
	{
		public string ApiUrl { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool Debug { get; set; }
		public string DebugKey { get; set; }
		public string Ports { get; set; }
		public string DispatchDomain { get; set; }
		public string DispatchTestDomain { get; set; }
		public string ListsDomain { get; set; }
		public string ListsTestDomain { get; set; }
		public string GroupsDomain { get; set; }
		public string GroupsTestDomain { get; set; }
		public string GroupMessageDomain { get; set; }
		public string GroupTestMessageDomain { get; set; }
	}
}
