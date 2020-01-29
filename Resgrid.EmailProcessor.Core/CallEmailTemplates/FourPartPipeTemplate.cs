using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class FourPartPipeTemplate : ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			if (String.IsNullOrEmpty(email.Subject))
				return null;

			string[] data = email.HtmlBody.Split(char.Parse("|"));

			Call c = new Call();
			c.Notes = email.Subject + " " + email.HtmlBody;

			if (data != null && data.Length >= 4)
				c.NatureOfCall = data[3];
			else
				c.NatureOfCall = email.Subject + " " + email.HtmlBody;

			if (data != null && data.Length >= 1)
				c.Name = data[0];

			c.LoggedOn = DateTime.UtcNow;
			c.Priority = priority;
			c.ReportingUserId = managingUser;
			c.Dispatches = new List<CallDispatch>();
			c.CallSource = (int)CallSources.EmailImport;
			c.SourceIdentifier = email.MessageID;

			if (data != null && data.Length > 3)
			{
				var geoData = data[2].Replace("[LONGLAT:","").Replace("] - 1:", "");

				if (!String.IsNullOrWhiteSpace(geoData))
				{
					var geoDataArray = geoData.Split(char.Parse(","));

					if (geoDataArray != null && geoDataArray.Length == 2)
					{
						c.GeoLocationData = $"{geoDataArray[1]}.{geoDataArray[0]}";
					}
				}
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
