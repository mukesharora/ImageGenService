using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniGPIEventMessage : OmniAPIMessage
	{
        /// <summary>
        /// Device name (typically a GUID)
        /// </summary>
		public string RFIDDeviceID { get; set; }
		public int GPIPortNumber { get; set; }
		public GPIOPortState PortState { get; set; }

        /// <summary>
        /// Device address is an IP address or multicast DNS address.
        /// </summary>
        public string RFIDDeviceAddress { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniGPIEventMessage;
		}

		public override string ToString()
		{
            return String.Format("RFIDDeviceID: {0}, RFIDDeviceAddress {1}, GPIPortNumber: {2}, PortState: {3}",
                RFIDDeviceID, RFIDDeviceAddress, GPIPortNumber, PortState);
		}
	}
}
