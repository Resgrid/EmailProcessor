using System.Collections.Generic;

namespace Resgrid.EmailProcessor.Core.Model
{
	public class Config
	{
		public string ApiUrl { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool Debug { get; set; }
		public string Ports { get; set; }
	}
}
