using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class GenericPageTemplate : ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			if (String.IsNullOrEmpty(email.Subject))
				return null;

			if (!email.Subject.ToLower().Contains("page"))
				return null;

			Call c = new Call();
			c.Notes = email.Subject + " " + email.HtmlBody;
			c.NatureOfCall = email.Subject + " " + email.HtmlBody;
			c.Name = email.Subject;
			c.LoggedOn = DateTime.UtcNow;
			c.Priority = priority;
			c.ReportingUserId = managingUser;
			c.Dispatches = new List<CallDispatch>();
			c.CallSource = (int)CallSources.EmailImport;
			c.SourceIdentifier = email.MessageID;

			//if (email.DispatchAudio != null)
			//{
			//	c.Attachments = new Collection<CallAttachment>();

			//	CallAttachment ca = new CallAttachment();
			//	ca.FileName = email.DispatchAudioFileName;
			//	ca.Data = email.DispatchAudio;
			//	ca.CallAttachmentType = (int)CallAttachmentTypes.DispatchAudio;

			//	c.Attachments.Add(ca);
			//}

			foreach (var u in users)
			{
				var cd = new CallDispatch();
				cd.UserId = u;

				c.Dispatches.Add(cd);
			}

			return c;
		}
	}
}
