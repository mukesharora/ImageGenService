using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniGPIStateReportCommandResultEvent : OmniAPIMessage
	{
		public Guid TransactionID { get; set; }
		public string RFIDDeviceID { get; set; }
		public int GPIPortNumber { get; set; }
		public GPIOPortState PortState { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniGPIStateReportCommandResultEvent;
		}

		public override string ToString()
		{
			return String.Format("TransactionID: {0}, RFIDDeviceID: {1}, GPIPortNumber: {2}, PortState: {3}",
				TransactionID, RFIDDeviceID, GPIPortNumber, PortState);
		}
	}
}
