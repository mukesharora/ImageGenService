using System;
using System.Reflection;
using ATRemoteObjectsLib;
using RevolutionServiceIPC.SignalR;
using SignalRLib;

namespace Octane2ReaderBLL
{
	public class Globals
	{
		public static string DBPath { get; set; }
		public static ITagReportSink TagReportSinkInterface { get; set; }
		public static IGPIReportSink GPIReportSinkInterface { get; set; }
		public static IReaderEventReportSink ReaderEventSinkInterface { get; set; }
		public static ISystemExceptionSink SystemExceptionReportSinkInterface { get; set; }
		public static IReaderInterface ReaderInterface { get; set; }
		public static ATRemoteAnnounceServerProxy IPCProxy { get; set; }
		public static SignalRServer<RFIDServerDef> RFIDServer { get; set; }

		/**
		 * Binds the SignalRServer to 'http://localhost:<port>/<name>' and
		 * attempts to connect it.
		 */
		public static void InitSignalRServer(int port, string name)
		{
			RFIDServer = new SignalRServer<RFIDServerDef>(new RFIDServerDef(),
				String.Format("http://localhost:{0}/{1}", port, name));

			ConnectSignalRServer();
		}

		/**
		 * This ensures that multiple threads don't try to connect the server
		 * at the same time (which causes errors), and catches errors that do
		 * occur.
		 */
		public static void ConnectSignalRServer()
		{
			lock(RFIDServer)
			{
				try
				{
					RFIDServer.Connect();
				}
				catch(TargetInvocationException tie)
				{
					//Black hole exception handling
				}
			}
		}

		//Gives other packages the ability to disconnect the SignalRServer
		//without referencing the SignalRLib dll
		public static void StopSignalRServer()
		{
			RFIDServer.Disconnect();
		}
	}
}
