using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.Helpers;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class PortJervisTemplate : ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			//if (String.IsNullOrEmpty(email.Subject))
			//	return null;

			Call c = new Call();
			c.Notes = email.Subject + " " + email.HtmlBody;
			c.NatureOfCall = email.HtmlBody;
			c.Name = "Call Email Import";
			c.LoggedOn = DateTime.UtcNow;
			c.Priority = priority;
			c.ReportingUserId = managingUser;
			c.Dispatches = new List<CallDispatch>();
			c.CallSource = (int)CallSources.EmailImport;
			c.SourceIdentifier = email.MessageID;

			string[] rawData = email.HtmlBody.Split(new string[] { "\r\n", "\r\n\r\n" }, StringSplitOptions.None);

			if (rawData != null && rawData.Any())
			{
				foreach (string s in rawData)
				{
					if (!string.IsNullOrWhiteSpace(s) && s.StartsWith("La", StringComparison.InvariantCultureIgnoreCase))
					{
						string[] geoData = s.Split(new string[] { "  " }, StringSplitOptions.None);

						if (geoData != null && geoData.Length == 2)
						{
							var latDMS = geoData[0].Replace(" grader ", ",").Replace("La = ", "").Replace("'N", "").Trim();
							var lonDMS = geoData[1].Replace(" grader ", ",").Replace("Lo = ", "").Replace("'E", "").Trim();

							if (!String.IsNullOrWhiteSpace(latDMS) && !String.IsNullOrWhiteSpace(lonDMS))
							{
								string[] latValues = latDMS.Split(new string[] { "," }, StringSplitOptions.None);
								string[] lonValues = lonDMS.Split(new string[] { "," }, StringSplitOptions.None);

								double lat = 0;
								if (latValues != null && latValues.Length == 3)
								{
									lat = LocationHelpers.ConvertDegreesToDecimal(latValues[0].Trim(), latValues[1].Trim(), latValues[2].Trim());
								}

								double lon = 0;
								if (lonValues != null && lonValues.Length == 3)
								{
									lon = LocationHelpers.ConvertDegreesToDecimal(lonValues[0].Trim(), lonValues[1].Trim(), lonValues[2].Trim());
								}


								if (lat != 0 && lon != 0)
								{
									c.GeoLocationData = $"{lat},{lon}";
								}
							}
						}
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
