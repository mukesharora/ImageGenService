using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane2ReaderBLL;
using ATRemoteObjectsLib;


namespace conTestBLL_TO_IPC
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
