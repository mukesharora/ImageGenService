using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	class OmniRFIDReaderCommandResultEvent : OmniAPIMessage
	{
		public Guid TransactionID { get; set; }
		public string RFIDDeviceID { get; set; }
		public bool Success { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniRFIDReaderCommandResultEvent;
		}

		public override string ToString()
		{
			return String.Format("TransactionID: {0}, RFIDDeviceID: {1}, Success: {2}",
				TransactionID, RFIDDeviceID, Success);
		}
	}
}
