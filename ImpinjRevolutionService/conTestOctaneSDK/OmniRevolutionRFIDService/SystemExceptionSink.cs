using System;
using System.Collections.Generic;
using Octane2ReaderBLL;
using RevolutionServiceIPC;

namespace OmniRevolutionRFIDService
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
