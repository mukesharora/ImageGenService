using System;
using System.Collections.Generic;
using System.Linq;
//using ATRemoteObjectsLib;
//using Middleware.client.calcman;

namespace CoralDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //ATAnnounceReceiver.ReceivedAnnouncesEvent += new ReceivedAnnouncesDelegate(ATAnnounceReceiver_ReceivedAnnouncesEvent);
            //ATAnnounceReceiver.ReceivedStatusMessagesEvent += new ReceivedStatusMessageDelegate(ATAnnounceReceiver_ReceivedStatusMessagesEvent);
            //ATAnnounceReceiver.Start();

            CoralDemo.demo.CoralDemo cd = new CoralDemo.demo.CoralDemo();
            cd.Start();
        }

        //static void ATAnnounceReceiver_ReceivedStatusMessagesEvent(List<StatusMessage> msgs)
        //{
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    Console.BackgroundColor = ConsoleColor.Green;

        //    foreach(StatusMessage msg in msgs)
        //    {
        //        if (msg.ErrorCode == -1)
        //        {
        //            Console.Write(string.Format("DestinationUid {0} command failed", msg.DestinationUid));
        //            //Console.WriteLine(string.Format("CalcErrorCode {0}", msg.CalcErrorCode));
        //        }
        //        else if (msg.ErrorCode == 0)
        //        {
        //            Console.Write(string.Format("DestinationUid {0} command success", msg.DestinationUid));
        //        }
        //    }
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.BackgroundColor = ConsoleColor.Black;
        //}

        //static void ATAnnounceReceiver_ReceivedAnnouncesEvent(List<ATRemoteObjectsLib.AtlasAnnounce> announces)
        //{
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    Console.BackgroundColor = ConsoleColor.Green;

        //    foreach (AtlasAnnounce ann in announces)
        //    {
        //        //Console.WriteLine(string.Format("Current Time : {0}", DateTime.Now));
        //        Console.WriteLine(string.Format("UID : {0}", ann.UID));
        //        //Console.WriteLine(string.Format("Rssi : {0}", ann.Rssi));
        //        //Console.WriteLine(string.Format("ReaderUID : {0}", ann.ReaderUID));
        //        //Console.WriteLine(string.Format("BatteryLevel : {0}", ann.BatteryLevel));
        //        //Console.WriteLine(string.Format("Temperature : {0}", ann.Temperature));
        //    }
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.BackgroundColor = ConsoleColor.Black;
        //}
    }
}
