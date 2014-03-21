using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATRemoteObjectsLib;
using Octane2ReaderBLL;

namespace OmniRFIDRevolutionService.ReportSink
{
    public class GPIReportSink : IGPIReportSink
    {
        public void ReportGPIEvent(string readerID, int port, bool newState)
        {
            List<RFIDMessage> msgList = new List<RFIDMessage>();
            msgList.Add(new RFIDMessage()
            {
                MessageType = RFIDMessage.RFIDMessageType.GPIEvent,
                RFIDDeviceID = readerID,
                RFIDDeviceAntennaID = "",
                RFIDTagEPC = "",
                GPIPortNumber = port,
                GPOPortNumber = -1,
                PortState = newState
            });

            Globals.IPCProxy.PostRFIDMessages(msgList);
        }
    }
}
