using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
    public class OmniRFIDDetectionMessage : OmniAPIMessage
    {
        /// <summary>
        /// Device name (typically a GUID)
        /// </summary>
		public string RFIDDeviceID { get; set; }

        /// <summary>
        /// Device address is an IP address or multicast DNS address.
        /// </summary>
        public string RFIDDeviceAddress { get; set; }
		public int RFIDDeviceAntennaID { get; set; }
		public string RFIDTagEPC { get; set; }
		public string RFIDTagUserMemory { get; set; }
		public DateTime TimeStamp { get; set; }

        protected override void InitMessageType()
        {
            MessageType = OmniAPIMessageType.OmniRFIDDetectionMessage;
        }

		public override string ToString()
		{
            return String.Format("RFIDDeviceID: {0}, RFIDDeviceAddress {1}, RFIDDeviceAntennaID: {2}, RFIDTagEPC: {3}, RFIDTagUserMemory: {4}, TimeStamp: {5}",
                RFIDDeviceID, RFIDDeviceAddress, RFIDDeviceAntennaID, RFIDTagEPC, RFIDTagUserMemory, TimeStamp);
		}
    }
}
