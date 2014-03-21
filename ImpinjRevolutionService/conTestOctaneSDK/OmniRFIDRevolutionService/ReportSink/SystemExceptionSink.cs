using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATRemoteObjectsLib;
using Octane2ReaderBLL;

namespace OmniRFIDRevolutionService.ReportSink
{
    public class SystemExceptionSink : ISystemExceptionSink
    {
        public void LogSystemException(string description)
        {
            List<RFIDMessage> msgList = new List<RFIDMessage>();
            msgList.Add(new RFIDMessage()
            {
                MessageType = RFIDMessage.RFIDMessageType.Exception,
                RFIDDeviceID = "",
                RFIDDeviceAntennaID = "",
                RFIDTagEPC = "",
                GPIPortNumber = -1,
                GPOPortNumber = -1,
                PortState = false,
                ErrorCode = "",
                Message = description
            });

            Globals.IPCProxy.PostRFIDMessages(msgList);
        }
    }
}
