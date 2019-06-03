using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public interface IImportService
	{
		void Run();
	}

	public class ImportService: IImportService
	{
		public void Run()
		{

		}
	}
}
