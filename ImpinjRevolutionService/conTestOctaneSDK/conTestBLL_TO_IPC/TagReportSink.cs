﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane2ReaderBLL;
using ATRemoteObjectsLib;


namespace conTestBLL_TO_IPC
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
                PortState = false
            });

            Globals.IPCProxy.PostRFIDMessages(msgList);
        }
    }
}
