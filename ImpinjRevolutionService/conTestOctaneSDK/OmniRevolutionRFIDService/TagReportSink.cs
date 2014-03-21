using System;
using System.Collections.Generic;
using Octane2ReaderBLL;
using RevolutionServiceIPC;

namespace OmniRevolutionRFIDService
{
    public class TagReportSink : ITagReportSink 
    {
        public void ReportTagRead(string readerID, string readerAddress, string EPC, int AntennaPortNumber = -1, string tagUSER = "")
        {            
            List<RFIDMessage> msgList = new List<RFIDMessage>();
            msgList.Add( new RFIDMessage() 
            { 
                MessageType = RFIDMessage.RFIDMessageType.RFIDDetection,
                RFIDDeviceID = readerID,
                RFIDDeviceAntennaID = AntennaPortNumber.ToString(),
                RFIDTagEPC = EPC,
                GPIPortNumber = -1,
                GPOPortNumber = -1,
                PortState = false,
                RFIDTagUSER = tagUSER
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
