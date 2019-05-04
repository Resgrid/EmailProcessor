using System.Reflection;
using Consolas.Core;
using Resgrid.EmailProcessor.Args;
using Resgrid.EmailProcessor.Models;

namespace Resgrid.EmailProcessor.Commands
{
	public class VersionCommand : Command
	{
		public object Execute(VersionArgs args)
		{
			var model = new VersionViewModel
			{
				Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
			};

			return View("Version", model);
		}
	}
}
