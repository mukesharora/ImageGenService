using System;
using System.Collections.Generic;
using Octane2ReaderBLL;
using RevolutionServiceIPC;

namespace OmniRevolutionRFIDService
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
