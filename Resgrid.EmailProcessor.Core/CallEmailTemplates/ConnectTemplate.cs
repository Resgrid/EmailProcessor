using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class ConnectTemplate : ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			Call c = new Call();
			c.Notes = email.HtmlBody;
			c.Name = email.Subject;

			int nonEmptyLineCount = 0;
			string[] rawData = email.HtmlBody.Split(new string[] { "\r\n", "\r\n\r\n" }, StringSplitOptions.None);

			foreach (var line in rawData)
			{
				if (line != null && !String.IsNullOrWhiteSpace(line))
				{
					nonEmptyLineCount++;

					switch (nonEmptyLineCount)
					{
						case 1:
							//c.Name = line.Trim();
							c.NatureOfCall = line.Trim();
							break;
						case 2: // Address
							//c.Name = $"{c.Name} {line}";
							c.NatureOfCall = c.NatureOfCall + " " + line;
							break;
						case 3: // Map and Radio Channel
							break;
						case 4: // Cross Street
							break;
						case 5: // Cross Street 2
							break;

					}
				}
			}

			c.LoggedOn = DateTime.UtcNow;
			c.Priority = priority;
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
