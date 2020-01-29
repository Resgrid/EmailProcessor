using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;

namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class CalFireEccTemplate: ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			Call c = new Call();
			c.Notes = email.HtmlBody;

			string[] data = email.HtmlBody.Split(char.Parse(";"));
			c.IncidentNumber = data[0].Replace("Inc# ", "").Trim();
			c.Type = data[1].Trim();

			try
			{
				int end = data[6].IndexOf(">Map");
				c.GeoLocationData = data[6].Substring(37, end - 38);
			}
			catch { }

			c.NatureOfCall = data[1].Trim() + "   " + data[7];
			c.Address = data[2].Trim();
			c.Name = data[1].Trim() + " " + data[7];
			c.LoggedOn = DateTime.UtcNow;
			c.Priority = priority;
			c.ReportingUserId = managingUser;
			c.Dispatches = new List<CallDispatch>();
			c.CallSource = (int) CallSources.EmailImport;
			c.SourceIdentifier = email.MessageID;

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
