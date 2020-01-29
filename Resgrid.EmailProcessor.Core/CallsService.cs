using Resgrid.ApiClient.V3.Models;
using Resgrid.EmailProcessor.Core.CallEmailTemplates;
using Resgrid.EmailProcessor.Core.Model;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Resgrid.EmailProcessor.Core
{
	public interface ICallsService
	{
		Call GenerateCallFromEmail(CallEmailTypes type, ApiClient.V3.Models.InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority);
	}

	public interface ICallEmailTemplate
	{
		Call GenerateCall(ApiClient.V3.Models.InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority);
	}

	public class CallsService : ICallsService
	{
		private Logger _log;
		private Dictionary<int, ICallEmailTemplate> _templates;

		public CallsService(Logger log)
		{
			_log = log;

			_templates = new Dictionary<int, ICallEmailTemplate>();
			_templates.Add((int)CallEmailTypes.CalFireECC, new CalFireEccTemplate());
			_templates.Add((int)CallEmailTypes.CarencroFire, new CarencroFireTemplate());
			_templates.Add((int)CallEmailTypes.Resgrid, new ResgridEmailTemplate());
			_templates.Add((int)CallEmailTypes.GrandBlanc, new GrandBlancTemplate());
			_templates.Add((int)CallEmailTypes.Generic, new GenericTemplate());
			_templates.Add((int)CallEmailTypes.LowestoftCoastGuard, new LowestoftCoastGuardTemplate());
			_templates.Add((int)CallEmailTypes.UnionFire, new UnionFireTemplate());
			_templates.Add((int)CallEmailTypes.ParklandCounty, new ParklandCountyTemplate());
			_templates.Add((int)CallEmailTypes.GenericPage, new GenericPageTemplate());
			_templates.Add((int)CallEmailTypes.Brockport, new BrockportTemplate());
			_templates.Add((int)CallEmailTypes.HancockCounty, new HancockTemplate());
			_templates.Add((int)CallEmailTypes.CalFireSCU, new CalFireSCUTemplate());
			_templates.Add((int)CallEmailTypes.Connect, new ConnectTemplate());
			_templates.Add((int)CallEmailTypes.SpottedDog, new SpottedDogTemplate());
			_templates.Add((int)CallEmailTypes.PortJervis, new PortJervisTemplate());
			_templates.Add((int)CallEmailTypes.Yellowhead, new YellowHeadTemplate());
			_templates.Add((int)CallEmailTypes.ParklandCounty2, new ParklandCounty2Template());
			_templates.Add((int)CallEmailTypes.FourPartPipe, new FourPartPipeTemplate());
		}

		public Call GenerateCallFromEmail(CallEmailTypes type, ApiClient.V3.Models.InboundMessage email, string managingUser, List<string> users, Department department, List<Call> activeCalls, List<Unit> units, int priority)
		{
			Call call = null;

			try
			{
				call = _templates[(int)type].GenerateCall(email, managingUser, users, department, activeCalls, units, priority);
			}
			catch (Exception ex)
			{
				_log.Error(ex, $"CallsService::GenerateCallFromEmail {email.HtmlBody}");
			}

			return call;
		}
	}
}
