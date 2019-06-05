using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace Resgrid.EmailProcessor.Core
{
	public interface IImportService
	{
		bool CreateCall(Model.Message message);
	}

	public class ImportService : IImportService
	{
		public bool CreateCall(Model.Message message)
		{

			return false;
		}
	}
}
