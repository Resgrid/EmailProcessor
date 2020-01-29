using System.Collections.Generic;

namespace Resgrid.EmailProcessor.Core.Model
{

	public static class CallTypesThatNeedActiveCalls
	{
		public static HashSet<int> CallTypes = new HashSet<int>()
		{
			(int)CallEmailTypes.Resgrid,
			(int)CallEmailTypes.ParklandCounty2
		};
	}
}
