using System.Threading;

namespace Resgrid.EmailProcessor.Core
{
	public interface IImportService
	{
		void Run(CancellationToken token);
	}

	public class ImportService : IImportService
	{
		public void Run(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{

			}
		}
	}
}
