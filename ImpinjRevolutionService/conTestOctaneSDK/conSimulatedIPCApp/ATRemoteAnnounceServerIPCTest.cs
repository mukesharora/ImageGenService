using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATRemoteObjectsLib;

namespace conSimulatedIPCApp
{
    public class ATRemoteAnnounceServerIPCTest : ATRemoteAnnounceServer
    {
        public override void PostRFIDMessages(List<RFIDMessage> RFIDMsgs)
        {
            //base.PostRFIDMessages(RFIDMsgs);
            foreach (RFIDMessage msg in RFIDMsgs)
            {
                switch (msg.MessageType)
                {
                    case RFIDMessage.RFIDMessageType.Exception:
                        {
                            Console.WriteLine(string.Format("EXCEPTION: RDR:{0} MSG:{1}", msg.RFIDDeviceID, msg.Message));
                        }
                        break;
                    case RFIDMessage.RFIDMessageType.GPIEvent:
                        {
                            Console.WriteLine(string.Format("GPI: RDR:{0} PORT:{1} STATE:{2}", msg.RFIDDeviceID, msg.GPIPortNumber, msg.PortState));
                        }
                        break;
                    case RFIDMessage.RFIDMessageType.GPIStateReport:
                        {
                            Console.WriteLine(string.Format("GPISTATERPT: RDR:{0} PORT:{1} STATE:{2}", msg.RFIDDeviceID, msg.GPIPortNumber, msg.PortState));
                        }
                        break;
                    case RFIDMessage.RFIDMessageType.GPOStateChange:
                        {
                            Console.WriteLine("GPO STATE CHANGE");
                        }
                        break;
                    case RFIDMessage.RFIDMessageType.GPOStateReport:
                        {
                            Console.WriteLine("GPO STATE REPORT");
                        }
                        break;
                    case RFIDMessage.RFIDMessageType.RFIDDetection:
                        {
                            Console.WriteLine(string.Format("RFID DETECT: RDR:{0} ANT:{1} EPC:{2} USR:{3}", msg.RFIDDeviceID, msg.RFIDDeviceAntennaID, msg.RFIDTagEPC,msg.RFIDTagUSER));
                        }
                        break;

                }
            }
        }
    }
}
