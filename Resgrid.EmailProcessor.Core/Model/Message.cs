using MimeKit;
using Resgrid.ApiClient.V3.Models;
using System;

namespace Resgrid.EmailProcessor.Core.Model
{
	public class Message
	{
		public Guid Id { get; set; }
		public int RetryCount { get; set; }
		public string ErrorReason { get; set; }
		public DateTime Timestamp { get; set; }
		public InboundMessage InboundMessage { get; set; }
	}
}
