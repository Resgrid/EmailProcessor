using MimeKit;
using System;

namespace Resgrid.EmailProcessor.Core.Model
{
	public class Message
	{
		public Guid Id { get; set; }
		public int RetryCount { get; set; }
		public string ErrorReason { get; set; }
		public DateTime Timestamp { get; set; }
		public MimeMessage MailMessage { get; set; }
	}
}
