using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevolutionServiceIPC
{
    [Serializable]
    public class RFIDMessage
    {
        public enum RFIDMessageType
        {
            GPIEvent = 0,
            GPIStateReport = 1,
            GPOStateReport = 2,
            GPOStateChange = 3,
            RFIDDetection = 4,
            Exception = 5,
        }

        public RFIDMessageType MessageType { get; set; }
        public string RFIDDeviceID {get;set;}
        public string RFIDDeviceAntennaID { get; set; }
        public bool PortState { get; set; } //high is 'true' , low is 'false' (RFID companies like Impinj use this convention)
        public int GPIPortNumber { get; set; }
        public int GPOPortNumber { get; set; }
        public string RFIDTagEPC { get; set; }
       
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string TransactionID { get; set; }
        public string RFIDTagUSER { get; set; }
    }
}
