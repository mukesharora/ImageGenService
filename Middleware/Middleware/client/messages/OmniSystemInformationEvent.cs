using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniSystemInformationEvent : OmniAPIMessage
	{
        /// <summary>
        /// Device name (typically a GUID)
        /// </summary>
		public string RFIDDeviceID { get; set; }

        /// <summary>
        /// Device address is an IP address or multicast DNS address.
        /// </summary>
        public string RFIDDeviceAddress { get; set; }
		public string Information { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniSystemInformationEvent;
		}

		public override string ToString()
		{
			return String.Format("RFIDDeviceID: {0}, DeviceAddress: {1} Information: {2}",
				RFIDDeviceID, RFIDDeviceAddress, Information);
		}
	}
}
