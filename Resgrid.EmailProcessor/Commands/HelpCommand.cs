using Consolas.Core;
using Resgrid.EmailProcessor.Args;

namespace Resgrid.EmailProcessor.Commands
{
	public class HelpCommand : Command
	{
		public object Execute(HelpArgs args)
		{
			return View("Help");
		}
	}
}