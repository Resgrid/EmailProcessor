using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class ParklandCountyTemplate : ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			if (!email.Subject.Contains("Parkland County Incident") && !email.Subject.Contains("Incident Message"))
				return null;

			var c = new Call();
			c.Notes = email.HtmlBody;

			try
			{
				var data = new List<string>();
				string[] rawData = email.HtmlBody.Split(new string[] { "\r\n", "\r\n\r\n" }, StringSplitOptions.None);

				if (rawData != null && rawData.Any())
				{
					foreach (string s in rawData)
					{
						if (!string.IsNullOrWhiteSpace(s))
							data.Add(s);
					}

					if (data.Count > 0)
					{
						c.Name = data[1].Trim().Replace("Type: ", "");

						try
						{
							var location = data.FirstOrDefault(x => x.StartsWith("Location:"));

							if (!String.IsNullOrWhiteSpace(location))
								c.Address = location.Replace("//", "").Replace("Business Name:", "");

							var coordinates = data.FirstOrDefault(x => x.Contains("(") && x.Contains(")"));

							if (!String.IsNullOrWhiteSpace(coordinates))
							{
								coordinates = coordinates.Replace("Common Place:", "");
								coordinates = coordinates.Trim();
								c.GeoLocationData = coordinates.Replace("(", "").Replace(")", "");
							}
						}
						catch
						{ }
					}
				}
				else
				{
					c.Name = "Call Import Type: Unknown";
				}
			}
			catch
			{
				c.Name = "Call Import Type: Unknown";
			}

			c.NatureOfCall = email.HtmlBody;
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
