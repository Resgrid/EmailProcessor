using Serilog.Core;
using SmtpServer;
using System.Threading;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public interface IEmailService
	{
		Task<bool> Run(CancellationToken token, Logger log);
	}

	public class EmailService: IEmailService
	{
		public async Task<bool> Run(CancellationToken token, Logger log)
		{
			var options = new SmtpServerOptionsBuilder()
							.ServerName("localhost")
							.Port(25, 587)
							.MessageStore(new MessageStore())
							.Build();

			var smtpServer = new SmtpServer.SmtpServer(options);
			await smtpServer.StartAsync(token);

			return true;
		}
	}
}
