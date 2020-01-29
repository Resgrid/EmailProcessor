using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;

namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class YellowHeadTemplate : ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			if (String.IsNullOrEmpty(email.Subject))
				return null;

			string[] data = email.HtmlBody.Split(char.Parse("/"));

			Call c = new Call();
			c.Notes = email.Subject + " " + email.HtmlBody;
			c.NatureOfCall = email.HtmlBody;
			c.Name = email.Subject;
			c.LoggedOn = DateTime.UtcNow;
			c.Priority = priority;
			c.ReportingUserId = managingUser;
			c.Dispatches = new List<CallDispatch>();
			c.CallSource = (int)CallSources.EmailImport;
			c.SourceIdentifier = email.MessageID;

			if (data != null && data.Length > 0)
			{
				c.Name = data[2];
				c.Address = data[3];
			}

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
