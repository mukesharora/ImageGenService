using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageGenModels;
using Middleware.client;
using Middleware.client.messages;
using Middleware.client.commands;

namespace Middleware
{
	class conTestOmniMiddlewareTag
	{
		private static OmniMiddlewareClient middlewareClient;

		static void Main(string[] args)
		{
            middlewareClient = new OmniMiddlewareClient("localhost:3000", "localhost:30525", "localhost:4000", "F33B072B647448d2BA48230903A2C565");
            
            List<string> commandList = new List<string>();
			string command = null;

			Console.Write("UID: ");
			string uid = Console.ReadLine();

			Console.Write("Calc ID: ");
			string calcId = Console.ReadLine();

			if(calcId.Length == 0)
			{
				calcId = null;
			}

			Console.WriteLine("Input commands, one per line, and terminate the sequence with 'start'");

			do
			{
				command = Console.ReadLine();
				commandList.Add(command);
			}
			while(!command.ToLower().Equals("start"));

            middlewareClient.OmniIDMiddlewareEvent += new OmniMiddlewareClient.OmniAPIEventCallbackHandler(MiddlewareEvent);

			foreach(string commandStr in commandList)
			{
				command = commandStr.Split(' ')[0].ToLower();
				List<string> commandParams = commandStr.Split(' ').Skip(1).ToList();

				Console.WriteLine("\nExecuting command: {0}", commandStr);

				if(command.Equals("image"))
				{
					int pageNum = -1;
					
					if(int.TryParse(commandParams[0], out pageNum))
					{
						if(commandParams.Count > 2)
						{
                            int templateNum = int.Parse(commandParams[1]);                            
							SendImage(uid, calcId, pageNum, templateNum, commandParams.Skip(2).ToList());
						}
						else
						{
							SendImage(uid, calcId, pageNum, commandParams[1]);
						}
					}
				}
				else if(command.Equals("page"))
				{
					int pageNum = -1;

					if(int.TryParse(commandParams[0], out pageNum))
					{
						SendPageChange(uid, calcId, pageNum);
					}
				}
				else if(command.Equals("delete"))
				{
					int pageNum = -1;

					if(int.TryParse(commandParams[0], out pageNum))
					{
						SendPageDelete(uid, calcId, pageNum);
					}
				}
				else if(command.Equals("info"))
				{
					SendInfoRequest(uid);
				}
				else if(command.Equals("metadata"))
				{
					SendMetadataRequest();
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
            middlewareClient.Dispose();
		}

        /// <summary>
        /// When OmniMiddlewareClient receives a message from CALC Man this method is called.
        /// </summary>
        /// <param name="apiMessages">the response from CALC Man</param>
		public static void MiddlewareEvent(OmniAPIMessage[] apiMessages)
		{
			foreach(OmniAPIMessage message in apiMessages)
			{
				switch(message.MessageType)
				{
					case OmniAPIMessageType.OmniVisualTagAnnounceEvent:
                    case OmniAPIMessageType.OmniVisualTagHealthReportMessage:
					case OmniAPIMessageType.OmniVisualTagInfoRequestResultEvent:
					case OmniAPIMessageType.OmniPageChangeCommandResultEvent:
					case OmniAPIMessageType.OmniImageUpdateCommandResultEvent:
					case OmniAPIMessageType.OmniPageDeleteCommandResultEvent:
					case OmniAPIMessageType.OmniSystemErrorEvent:
					case OmniAPIMessageType.OmniCommandErrorResultEvent:
					case OmniAPIMessageType.OmniSystemMetadataInfoRequestResultEvent:                    
						Console.WriteLine(message);

						break;
					case OmniAPIMessageType.OmniSystemInformationEvent:
						OmniSystemInformationEvent systemInfo = message as OmniSystemInformationEvent;

						if(systemInfo.RFIDDeviceID == null)
						{
							Console.WriteLine("\nInformation: {0}", systemInfo.Information);
						}
						
						break;
					default:
						break;
				}
			}
		}

		#region Private helper functions

		private static void SendImage(string uid, string calcId, int pageNum, int templateNum, List<string> fields)
		{
            int counter = 0;
            Dictionary<string, string> fieldDictionary = new Dictionary<string, string>();
            foreach (string fieldValue in fields)
            {
                string fieldName = string.Format("field{0}", counter++);
                fieldDictionary.Add(fieldName, fieldValue);
            }

			middlewareClient.PostOmniAPICommand(new OmniImageUpdateCommand()
			{
				TransactionID = Guid.NewGuid(),
				VisualTagUID = uid,
				PageNumber = pageNum,
                ImageTemplateID = templateNum,    
                CoralType = CoralTypes.P4,
				ImageTemplateParameters = fieldDictionary,
				GatewayID = calcId
			});
		}

		private static void SendImage(string uid, string calcId, int pageNum, string url)
		{
			middlewareClient.PostOmniAPICommand(new OmniImageUrlUpdateCommand()
			{
				TransactionID = Guid.NewGuid(),
				VisualTagUID = uid,
				PageNumber = pageNum,
				ImageURL = url,
				GatewayID = calcId
			});
		}

		private static void SendPageChange(string uid, string calcId, int pageNum)
		{
			middlewareClient.PostOmniAPICommand(new OmniPageChangeCommand()
			{
				PageNumber = pageNum,
				TransactionID = Guid.NewGuid(),
				VisualTagUID = uid,
				GatewayID = calcId
			});
		}

		private static void SendPageDelete(string uid, string calcId, int pageNum)
		{
			middlewareClient.PostOmniAPICommand(new OmniPageDeleteCommand()
			{
				PageNumber = pageNum,
				TransactionID = Guid.NewGuid(),
				VisualTagUID = uid,
				GatewayID = calcId
			});
		}

		private static void SendInfoRequest(string uid)
		{
			middlewareClient.PostOmniAPICommand(new OmniVisualTagInfoRequestCommand()
			{
				TransactionID = Guid.NewGuid(),
				VisualTagUID = uid
			});
		}

		private static void SendMetadataRequest()
		{
			middlewareClient.PostOmniAPICommand(new OmniSystemMetadataInfoRequestCommand()
			{
				TransactionID = Guid.NewGuid()
			});
		}

		#endregion
	}
}
