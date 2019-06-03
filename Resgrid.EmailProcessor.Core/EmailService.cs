using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public interface IEmailService
	{
		void Run();
	}

	public class EmailService: IEmailService
	{
		public void Run()
		{

		}
	}
}
