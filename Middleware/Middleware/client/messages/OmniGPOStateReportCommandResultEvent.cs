using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniGPOStateReportCommandResultEvent : OmniAPIMessage
	{
		public Guid TransactionID { get; set; }
		public string RFIDDeviceID { get; set; }
		public int GPOPortNumber { get; set; }
		public GPIOPortState PortState { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniGPOStateReportCommandResultEvent;
		}

		public override string ToString()
		{
			return String.Format("TransactionID: {0}, RFIDDeviceID: {1}, GPOPortNumber: {2}, PortState: {3}",
				TransactionID, RFIDDeviceID, GPOPortNumber, PortState);
		}
	}
}
