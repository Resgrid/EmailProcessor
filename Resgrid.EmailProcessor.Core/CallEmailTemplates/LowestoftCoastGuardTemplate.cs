using Resgrid.ApiClient.Common;
using Resgrid.ApiClient.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Resgrid.EmailProcessor.Core.CallEmailTemplates
{
	public class LowestoftCoastGuardTemplate : ICallEmailTemplate
	{
		public Call GenerateCall(InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			if (email == null)
				return null;

			if (String.IsNullOrEmpty(email.HtmlBody))
				return null;

			if (String.IsNullOrEmpty(email.Subject))
				return null;

			Call c = new Call();
			c.Notes = email.Subject + " " + email.HtmlBody;
			c.Name = email.Subject;

			string[] data = email.HtmlBody.Split(char.Parse(","));

			if (data.Length >= 1)
			{
				int tryPriority;

				if (int.TryParse(data[0], out tryPriority))
					c.Priority = tryPriority;
			}
			else
			{
				c.Priority = (int)ApiClient.Common.CallPriority.High;
			}

			if (data.Length >= 2)
			{
				if (data[1].Length > 3)
					c.NatureOfCall = data[1];
			}
			else
			{
				c.NatureOfCall = email.HtmlBody;
			}

			if (data.Length >= 3)
			{
				if (data[2].Length > 3)
				{
					string address = String.Empty;

					if (!data[2].Contains("United Kingdom"))
					{
						address = data[2] + "United Kingdom";
					}

					c.Address = address;
				}
			}

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
