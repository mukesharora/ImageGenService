using System.Collections.Generic;
using SignalRLib;

namespace RevolutionServiceIPC.SignalR
{
	public delegate void ReceivedRFIDMessageDelegate(List<RFIDMessage> messages);

	public class RFIDClientDef : ClientDefinition
	{
		public event ReceivedRFIDMessageDelegate ReceivedRFIDMessagesEvent;

		/**
		 * Registers a function to fire off an event when the server sends
		 * RFIDMessages.
		 */
		public override void RegisterEvents()
		{
			RegisterFunc<List<RFIDMessage>>(PostRFIDMessages);
		}

		/*
		 * Server-to-Client functions
		 */

		/**
		 * Fires off the ReceivedRFIDMessagesEvent upon receipt of RFID
		 * messages from the server.
		 */
		private void PostRFIDMessages(List<RFIDMessage> messages)
		{
			if(ReceivedRFIDMessagesEvent != null)
			{
				ReceivedRFIDMessagesEvent(messages);
			}
		}
	}
}