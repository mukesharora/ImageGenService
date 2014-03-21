using System.Collections.Generic;
using SignalRLib;

namespace RevolutionServiceIPC.SignalR
{
	public class RFIDServerDef : ServerDefinition
	{
		/*
		 * Server-to-Client functions
		 */

		public void PostRFIDMessages(List<RFIDMessage> messages)
		{
			HubContext.Clients.All.PostRFIDMessages(messages);
		}
	}
}