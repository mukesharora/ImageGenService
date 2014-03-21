using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ATRemoteObjectsLib;
using Octane2ReaderBLL;

namespace OmniRFIDRevolutionService.ReportSink
{
    public class ReaderEventSink : IReaderEventReportSink
    {
        public void ReportReaderEvent(string readerID, string eventDescription, string errorCode = "")
        {
            List<RFIDMessage> msgList = new List<RFIDMessage>();
            msgList.Add(new RFIDMessage()
            {
                MessageType = RFIDMessage.RFIDMessageType.Exception,
                RFIDDeviceID = readerID,
                RFIDDeviceAntennaID = "",
                RFIDTagEPC = "",
                GPIPortNumber = -1,
                GPOPortNumber = -1,
                PortState = false,
                ErrorCode = errorCode,
                Message = eventDescription
            });

            Globals.IPCProxy.PostRFIDMessages(msgList);
        }

       
    }
}
