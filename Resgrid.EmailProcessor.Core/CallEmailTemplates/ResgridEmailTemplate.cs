using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class ResgridEmailTemplate : ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			//ID | TYPE | PRIORITY | ADDRESS | MAPPAGE | NATURE | NOTES

			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			Call c = new Call();
			c.Notes = email.HtmlBody;

			string[] data = email.HtmlBody.Split(char.Parse("|"));

			if (data.Any() && data.Length >= 5)
			{
				if (!string.IsNullOrEmpty(data[0]))
					c.IncidentNumber = data[0].Trim();

				if (!string.IsNullOrEmpty(data[1]))
					c.Type = data[1].Trim();

				if (string.IsNullOrEmpty(data[2]))
				{
					int prio;
					if (int.TryParse(data[2], out prio))
					{
						c.Priority = prio;
					}
					else
					{
						c.Priority = priority;
					}
				}
				else
				{
					c.Priority = priority;
				}

				if (!string.IsNullOrEmpty(data[4]))
					c.MapPage = data[4];

				c.NatureOfCall = data[5];
				c.Address = data[3];

				StringBuilder title = new StringBuilder();

				title.Append("Email Call ");
				title.Append(((ApiClient.Common.CallPriority)c.Priority).ToString());
				title.Append(" ");

				if (!string.IsNullOrEmpty(c.Type))
				{
					title.Append(c.Type);
					title.Append(" ");
				}

				if (!string.IsNullOrEmpty(c.IncidentNumber))
				{
					title.Append(c.IncidentNumber);
					title.Append(" ");
				}

				c.Name = title.ToString();
			}
			else
			{
				c.Name = email.Subject;
				c.NatureOfCall = email.HtmlBody;
				c.Notes = "WARNING: FALLBACK RESGRID EMAIL IMPORT! THIS EMAIL MAY NOT BE THE CORRECT FORMAT FOR THE RESGRID EMAIL TEMPLATE. CONTACT SUPPORT IF THE EMAIL AND TEMPLATE ARE CORRECT." + email.HtmlBody;
			}

			c.LoggedOn = DateTime.UtcNow;
			c.ReportingUserId = managingUser;
			c.Dispatches = new List<CallDispatch>();
			c.CallSource = (int)CallSources.EmailImport;
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
