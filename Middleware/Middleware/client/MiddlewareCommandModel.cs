using System.IO;
using System.Net;
using System.Xml.Serialization;
using CALCManager.Models;
using CALCManager.Models.OtherClientModels;
using ImageGenModels;
using Middleware.client.commands;
using Middleware.client.messages;
using RESTClients.General;
using RFIDWebApiService.Models;
using System;

#if !REVOLUTION_SERVICE
using SignalRLib;
using OmniImpinjReaderIPC;
using OmniImpinjReaderIPC.SignalR;
#endif

namespace Middleware.client
{
	/**
	 * Exposes data and helper functions to commands during execution.
	 */
	internal class MiddlewareCommandModel
	{
		public RestClients Clients { get; private set; }
		public string SystemMetadataFile { get; private set; }

#if !REVOLUTION_SERVICE
		public SignalRClient<RFIDClientDef> rfidReaderClient { private get; set; }
#endif

		/**
		 * Creates a new MiddlewareCommandModel which will expose the given
		 * RestClients and SystemMetadataFile to executing commands.
		 * 
		 * @param restClients	RestClients structure with instances of all
		 *						REST web clients needed by executing commands.
		 * @param metadataFile	The location of the SystemMetadataFile.
		 */
		public MiddlewareCommandModel(RestClients restClients, string metadataFile)
		{
			Clients = restClients;
			SystemMetadataFile = metadataFile;
		}

		/**
		 * Checks the given web response for errors and posts any errorMsg
		 * messages to the client. Returns whether the response was good or bad.
		 * 
		 * @return	True iff there were no errors in the web response, false
		 *			otherwise.
		 */
		public bool CheckWebResponse<T>(WebResponseDataEventArgs<T> response,
			OmniAPICommand command, out OmniAPIMessage errorMessage)
		{
			errorMessage = null;

			if(response.Error != null)
			{
				if(response.webReponse == null)
				{
					errorMessage = OmniSystemErrorEvent.Server_Unreachable_Error;
				}
				else
				{
					switch(response.webReponse.StatusCode)
					{
						case HttpStatusCode.BadGateway:
						case HttpStatusCode.ServiceUnavailable:
						case HttpStatusCode.ProxyAuthenticationRequired:
							errorMessage = OmniSystemErrorEvent.Server_Unreachable_Error;
							break;
						case HttpStatusCode.InternalServerError:
							errorMessage = OmniSystemErrorEvent.Internal_Server_Error;
							(errorMessage as OmniSystemErrorEvent).Information += response.Error.Message;
							break;
						case HttpStatusCode.ExpectationFailed:
						case HttpStatusCode.BadRequest:
						case HttpStatusCode.NotFound:
							errorMessage = OmniCommandErrorResultEvent.Rejected_Command_Error;
							(errorMessage as OmniCommandErrorResultEvent).Information += response.Error.Message;
							(errorMessage as OmniCommandErrorResultEvent).TransactionID = command.TransactionID;
							break;
						default:
							errorMessage = OmniSystemErrorEvent.Web_Error;
							(errorMessage as OmniSystemErrorEvent).Information += response.Error.Message;
							break;
					}
				}

				return false;
			}

			return true;
		}

#if REVOLUTION_SERVICE
		
		/**
		 * Converts the given GPIOState from the Revolution Service to a
		 * Middleware GPIOPortState enumeration and returns it.
		 */
		public GPIOPortState ConvertGPIOState(GPIOState portState)
		{
			return portState.State ? GPIOPortState.High : GPIOPortState.Low;
		}

#else

		/**
		 * 
		 */
		public void SendReaderStateChange(ReaderStateRequest request)
		{
			rfidReaderClient.ClientDefinition.ChangeReaderState(request);
		}

		/**
		 * 
		 */
		public void SendGPOStateChange(GPOStateRequest request)
		{
			rfidReaderClient.ClientDefinition.ChangeGPOState(request);
		}

#endif

		/**
		 * Returns an XML string version of the given array of Corals,
		 * including only entries for which CurrentPage is non-null and showing
		 * only the following fields: Uid, CurrentPage, AwakeDwell, SleepDwell,
		 * Battery, and WakeupReason.
		 */
		public string ToVisualTagXML(Coral[] corals)
		{
			string tagXml = "";

			foreach(Coral c in corals)
			{
				if(c.DisplayType != 0)
				{
					tagXml += c.ToShortXml() + "\n";
				}
			}

			return "<VisualTags>\n" + tagXml.TabIn() + "</VisualTags>";
		}

		/**
		 * Returns an XML string version of the given array of Readers, with
		 * the namespace and XML version header stripped and indenting redone.
		 */
		public string ToReaderXML(Reader[] readers)
		{
			string readerXml = "";

			foreach(Reader r in readers)
			{
				XmlSerializer xmlCereal = new XmlSerializer(typeof(Reader));
				StringWriter writer = new StringWriter();

				xmlCereal.Serialize(writer, r);

				string xml = writer.ToString().Replace("  ", "\t");
				readerXml += "<Reader>\n\t" + xml.Remove(0, xml.IndexOf("<Name>")) + "\n";
			}

			return "<RFIDReaders>\n" + readerXml.TabIn() + "</RFIDReaders>";
		}

		/**
		 * Holds a set of RESTClients needed by the Middleware so that they only
		 * need to be created once.
		 */
		public struct RestClients
		{
			public PostRestClient<Coral, CmdStatus> CoralsPostClient;
			public GetRestClient<Coral> CoralsGetClient;
			public GetRestClient<Coral[]> CoralsListGetClient;

			public PostRestClient<ImageData, string> ImageGenPostClient;

			public PostRestClient<ReaderRequest, string> ReaderPostClient;
			public GetRestClient<GPIOState> ReaderGetClient;
			public GetRestClient<Reader[]> ReaderListGetClient;

			public static RestClients CreateInstance(string calcManHost, string imageGenHost, string rfidHost)
			{
				string invalidHost = "nohost";
				int invalidPort = 0;

				string[] calcManHostPair = calcManHost.Split(':');
				string[] imageGenHostPair = imageGenHost.Split(':');
				string[] rfidHostPair = rfidHost.Split(':');

				RestClients clients = new RestClients();

				try
				{
					clients.CreateCoralsClients(calcManHostPair[0], int.Parse(calcManHostPair[1]));
				}
				catch(Exception)
				{
					clients.CreateCoralsClients(invalidHost, invalidPort);
				}

				try
				{
					clients.CreateImageGenClients(imageGenHostPair[0], int.Parse(imageGenHostPair[1]));
				}
				catch(Exception)
				{
					clients.CreateImageGenClients(invalidHost, invalidPort);
				}

				try
				{
					clients.CreateReaderClients(rfidHostPair[0], int.Parse(rfidHostPair[1]));
				}
				catch(Exception)
				{
					clients.CreateReaderClients(invalidHost, invalidPort);
				}

				return clients;
			}

			private void CreateCoralsClients(string hostIp, int hostPort)
			{
				CoralsPostClient = new PostRestClient<Coral, CmdStatus>("api/corals",
						hostIp, hostPort);
				CoralsGetClient = new GetRestClient<Coral>("api/corals", "SomeGarbage");
				CoralsListGetClient = new GetRestClient<Coral[]>("api/corals", "SomeGarbage");

				CoralsGetClient.Host = hostIp + ":" + hostPort;
				CoralsListGetClient.Host = hostIp + ":" + hostPort;
			}

			private void CreateImageGenClients(string hostIp, int hostPort)
			{
				ImageGenPostClient = new PostRestClient<ImageData, string>("api/image",
						hostIp, hostPort);
			}

			private void CreateReaderClients(string hostIp, int hostPort)
			{
				ReaderPostClient = new PostRestClient<ReaderRequest, string>("api/reader",
						hostIp, hostPort);
				ReaderGetClient = new GetRestClient<GPIOState>("api/reader", "SomeGarbage");
				ReaderListGetClient = new GetRestClient<Reader[]>("api/reader", "SomeGarbage");

				ReaderGetClient.Host = hostIp + ":" + hostPort;
				ReaderListGetClient.Host = hostIp + ":" + hostPort;
			}
		}
	}

	/**
	 * String class extensions for convenience.
	 */
	internal static class StringExtensions
	{
		/**
		 * Returns a copy of this string with every line tabbed in once.
		 */
		public static string TabIn(this string str)
		{
			return "\t" + str.Replace("\n", "\n\t").TrimEnd('\t');
		}
	}
}
