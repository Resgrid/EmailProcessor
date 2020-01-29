using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class ParklandCounty2Template : ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			if (!email.Subject.Contains("Parkland County Incident") && !email.Subject.Contains("Incident Message"))
				return null;

			bool is2ndPage = false;
			string priorityChar = null;
			DateTime callTimeUtc = DateTime.UtcNow;

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

						var priorityString = c.Name.Substring(0, c.Name.IndexOf(char.Parse("-"))).Trim();
						priorityChar = Regex.Replace(priorityString, @"\d", "").Trim();

						TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(department.TimeZone);
						callTimeUtc = new DateTimeOffset(DateTime.Parse(data[0].Replace("Date:", "").Trim()), timeZone.BaseUtcOffset).UtcDateTime;

						is2ndPage = c.Notes.Contains("WCT2ndPage");

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
			c.LoggedOn = callTimeUtc;
			c.Priority = PriorityMapping(priorityChar, is2ndPage, priority);
			c.ReportingUserId = managingUser;
			c.Dispatches = new List<CallDispatch>();
			c.UnitDispatches = new List<CallDispatchUnit>();
			c.CallSource = (int)CallSources.EmailImport;
			c.SourceIdentifier = email.MessageID;

			foreach (var u in users)
			{
				var cd = new CallDispatch();
				cd.UserId = u;

				c.Dispatches.Add(cd);
			}

			if (units != null && units.Any())
			{
				foreach (var unit in units)
				{
					var ud = new CallDispatchUnit();
					ud.UnitId = unit.Id;
					ud.LastDispatchedOn = DateTime.UtcNow;
					
					c.UnitDispatches.Add(ud);
				}
			}

			// Search for an active call
			if (activeCalls != null && activeCalls.Any())
			{
				var activeCall = activeCalls.FirstOrDefault(x => x.Name == c.Name && x.LoggedOn == c.LoggedOn);

				if (activeCall != null)
				{
					activeCall.Priority = c.Priority;
					activeCall.Notes = c.Notes;
					activeCall.LastDispatchedOn = DateTime.UtcNow;
					activeCall.DispatchCount++;

					return activeCall;
				}
			}
			
			return c;
		}

		private int PriorityMapping(string priority, bool is2ndPage, int priorityDefault)
		{
			if (priority.Equals("E", StringComparison.CurrentCultureIgnoreCase))    // Emergency
			{
				return (int)ApiClient.Common.CallPriority.Emergency;
			}
			else if (priority.Equals("C", StringComparison.CurrentCultureIgnoreCase)) // High
			{
				if (is2ndPage)
					return (int)ApiClient.Common.CallPriority.Emergency;
				else
					return (int)ApiClient.Common.CallPriority.High;
			}
			else if (priority.Equals("D", StringComparison.CurrentCultureIgnoreCase)) // High
			{
				if (is2ndPage)
					return (int)ApiClient.Common.CallPriority.Emergency;
				else
					return (int)ApiClient.Common.CallPriority.High;
			}
			else if (priority.Equals("B", StringComparison.CurrentCultureIgnoreCase)) // Medium
			{
				if (is2ndPage)
					return (int)ApiClient.Common.CallPriority.High;
				else
					return (int)ApiClient.Common.CallPriority.Medium;
			}
			else if (priority.Equals("O", StringComparison.CurrentCultureIgnoreCase)) // Low
			{
				if (is2ndPage)
					return (int)ApiClient.Common.CallPriority.Medium;
				else
					return (int)ApiClient.Common.CallPriority.Low;
			}
			else if (priority.Equals("A", StringComparison.CurrentCultureIgnoreCase)) // Low
			{
				if (is2ndPage)
					return (int)ApiClient.Common.CallPriority.Medium;
				else
					return (int)ApiClient.Common.CallPriority.Low;
			}

			return priorityDefault;
		}
	}
}
