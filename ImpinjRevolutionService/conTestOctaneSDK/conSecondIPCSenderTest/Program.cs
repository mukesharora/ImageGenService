using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATRemoteObjectsLib;
using System.Threading;
using System.Windows.Forms;

namespace conSecondIPCSenderTest
{

    public static class Globals
    {
        public static ATRemoteAnnounceServerProxy IPCProxy { get; set; }
    }

    class Program
    {
        static void CreateGlobalObjects()
        {
            Globals.IPCProxy = new ATRemoteAnnounceServerProxy();  
        }


        static void Main(string[] args)
        {
            CreateGlobalObjects();

            Globals.IPCProxy.Connect();

            if (!Globals.IPCProxy.IsConnected)
            {
                Console.WriteLine("IPC Proxy Unable to Connect");
                //TODO : May want to terminate here.
            }

            DateTime timeCheck = DateTime.Now;

            Console.WriteLine("Press Escape key to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(250);
                    Application.DoEvents();

                    if (timeCheck + TimeSpan.FromSeconds(3) < DateTime.Now)
                    {
                        timeCheck = DateTime.Now;
                        List<RFIDMessage> msgList = new List<RFIDMessage>();
                        msgList.Add(new RFIDMessage()
                        {
                             MessageType = RFIDMessage.RFIDMessageType.Exception,
                             Message = "**********HI THERE FROM SECOND SENDER**********"
                        });
                        Globals.IPCProxy.PostRFIDMessages(msgList);
                    }
                }

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
