using SmtpServer;
using System.Threading;
using System.Threading.Tasks;

namespace Resgrid.EmailProcessor.Core
{
	public interface IEmailService
	{
		Task<bool> Run(CancellationToken token);
	}

	public class EmailService: IEmailService
	{
		public async Task<bool> Run(CancellationToken token)
		{
			var options = new SmtpServerOptionsBuilder()
							.ServerName("localhost")
							.Port(25, 587)
							.MessageStore(new SampleMessageStore())
							.Build();

			var smtpServer = new SmtpServer.SmtpServer(options);
			await smtpServer.StartAsync(token);

			return true;
		}
	}
}
