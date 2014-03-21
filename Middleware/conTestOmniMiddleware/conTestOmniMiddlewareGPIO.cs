using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client;
using Middleware.client.messages;
using Middleware.client.commands;

namespace Middleware
{
	class conTestOmniMiddlewareGPIO
	{
		private static OmniMiddlewareClient middlewareClient;

		static void Main(string[] args)
		{
            // Zero parameter constructor. Client sets configuration via their app.config file:
            //
            //      <add key="CALCManServiceHostName" value="localhost:3000" />
            //      <add key="ImageGenServiceHostName" value="localhost:30525" />
            //      <add key="RFIDServiceHostName" value="localhost:3300" />
            //      <add key="MiddlewareClientGuid" value="F33B072B647448d2BA48230903A2C565" />    
            //
            middlewareClient = new OmniMiddlewareClient();
            
            
            Console.Write("RFID Device ID: ");
			
			List<string> commandList = new List<string>();
			string command = null;
			string deviceID = Console.ReadLine();

			Console.WriteLine("Input commands, one per line, and terminate the sequence with 'start'");

			do
			{
				command = Console.ReadLine().ToLower();
				commandList.Add(command);
			}
			while(!command.Equals("start"));

            middlewareClient.OmniIDMiddlewareEvent += new OmniMiddlewareClient.OmniAPIEventCallbackHandler(MiddlewareEvent);

			foreach(string commandStr in commandList)
			{
				command = commandStr.Split(new char[] { ' ' })[0];
				List<string> commandParams = commandStr.Split(new char[] { ' ' }).Skip(1).ToList();

				Console.WriteLine("\nExecuting command: {0}", commandStr);

				if(command.Equals("gpistate"))
				{
					int portNum = -1;
					
					if(int.TryParse(commandParams[0], out portNum))
					{
						RequestGPIPortState(deviceID, portNum);
					}
				}
				else if(command.Equals("gpochange"))
				{
					int portNum = -1;
					int portState = -1;

					if(int.TryParse(commandParams[0], out portNum) &&
						int.TryParse(commandParams[1], out portState))
					{
						ChangeGPOPortState(deviceID, portNum, portState);
					}
				}
				else if(command.Equals("gpostate"))
				{
					int portNum = -1;

					if(int.TryParse(commandParams[0], out portNum))
					{
						RequestGPOPortState(deviceID, portNum);
					}
				}
				else if(command.Equals("startreader"))
				{
					StartStopReader(deviceID, true);
				}
				else if(command.Equals("stopreader"))
				{
					StartStopReader(deviceID, false);
				}
				else if(command.Equals("listen"))
				{
					//Just listen until the user hits enter
				}
				else
				{
					break;
				}

				Console.ReadLine();
			}

            middlewareClient.OmniIDMiddlewareEvent += new OmniMiddlewareClient.OmniAPIEventCallbackHandler(MiddlewareEvent);
            middlewareClient.Dispose();
            middlewareClient = null;
		}

		public static void MiddlewareEvent(OmniAPIMessage[] apiMessages)
		{
			foreach(OmniAPIMessage message in apiMessages)
			{
				switch(message.MessageType)
				{
					case OmniAPIMessageType.OmniGPIStateReportCommandResultEvent:
					case OmniAPIMessageType.OmniGPOStateReportCommandResultEvent:
					case OmniAPIMessageType.OmniGPOStateChangeCommandResultEvent:
					case OmniAPIMessageType.OmniSystemErrorEvent:
					case OmniAPIMessageType.OmniCommandErrorResultEvent:
					case OmniAPIMessageType.OmniGPIEventMessage:
					case OmniAPIMessageType.OmniRFIDDetectionMessage:
					case OmniAPIMessageType.OmniSystemInformationEvent:
						Console.WriteLine(message);

						break;
					default:
						break;
				}
			}
		}

		#region Private helper functions

		private static void RequestGPIPortState(string deviceID, int portNum)
		{
			middlewareClient.PostOmniAPICommand(new OmniGPIStateReportCommand()
			{
				TransactionID = Guid.NewGuid(),
				RFIDDeviceID = deviceID,
				GPIPortNumber = portNum
			});
		}

		private static void RequestGPOPortState(string deviceID, int portNum)
		{
			middlewareClient.PostOmniAPICommand(new OmniGPOStateReportCommand()
			{
				TransactionID = Guid.NewGuid(),
				RFIDDeviceID = deviceID,
				GPOPortNumber = portNum
			});
		}

		private static void ChangeGPOPortState(string deviceId, int portNum, int portState)
		{
			middlewareClient.PostOmniAPICommand(new OmniGPOStateChangeCommand()
			{
				TransactionID = Guid.NewGuid(),
				GPOPortNumber = portNum,
				RequestedState = portState == 1 ? GPIOPortState.High : GPIOPortState.Low,
				RFIDDeviceID = deviceId
			});
		}

		private static void StartStopReader(string deviceId, bool start)
		{
			middlewareClient.PostOmniAPICommand(new OmniRFIDReaderCommand()
			{
				TransactionID = Guid.NewGuid(),
				RFIDDeviceID = deviceId,
				RequestedReaderState = start ? OmniRFIDReaderCommand.ReaderState.StartReading : OmniRFIDReaderCommand.ReaderState.StopReading
			});
		}

		#endregion
	}
}
