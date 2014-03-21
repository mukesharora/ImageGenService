using System;
using System.Collections.Generic;
using Octane2ReaderBLL;
using RevolutionServiceIPC;

namespace OmniRevolutionRFIDService
{
    public class ReaderEventSink : IReaderEventReportSink 
    {
        public void ReportReaderEvent(string readerID, string eventDescription, string errorCode)
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

			if(!Globals.RFIDServer.IsConnected)
			{
				Globals.ConnectSignalRServer();
			}

			try
			{
				Globals.RFIDServer.ServerDefinition.PostRFIDMessages(msgList);
			}
			catch(Exception e)
			{
				//Black hole Exception handling
			}
        }
    }
}
