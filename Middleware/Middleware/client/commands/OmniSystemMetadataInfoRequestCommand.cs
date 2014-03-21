using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Middleware.client.messages;

namespace Middleware.client.commands
{
	public class OmniSystemMetadataInfoRequestCommand : OmniAPICommand
	{
		protected override void InitCommandType()
		{
			CommandType = OmniAPICommandType.OmniSystemMetadataInfoRequestCommand;
			HasCALCResponse = false;
		}

		/**
		 * Reads in the SystemMetadata.xml file and posts a response to the
		 * client with the contents of said file.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			StreamReader fileIn = new StreamReader(File.Open(
				model.SystemMetadataFile, FileMode.Open, FileAccess.Read));
			string xmlDoc = fileIn.ReadToEnd();
			string generatedXml = "";

			fileIn.Close();

			var coralsListResponse = model.Clients.CoralsListGetClient.Get();

			responseMessages = new List<messages.OmniAPIMessage>();
			OmniAPIMessage error;

			if(model.CheckWebResponse(coralsListResponse, this, out error))
			{
				generatedXml += "\n" + model.ToVisualTagXML(coralsListResponse.responseData).TabIn();
			}
			else
			{
				responseMessages.Add(error);
			}

#if REVOLUTION_SERVICE

			var readerListResponse = model.Clients.ReaderListGetClient.Get();

			if(model.CheckWebResponse(readerListResponse, this, out error))
			{
				generatedXml += "\n" + model.ToReaderXML(readerListResponse.responseData).TabIn();
			}
			else
			{
				responseMessages.Add(error);
			}

#endif

			int lastNewline = xmlDoc.LastIndexOf("\r\n");
			xmlDoc = xmlDoc.Substring(0, lastNewline) + generatedXml +
				xmlDoc.Substring(lastNewline);

			responseMessages.Add(new OmniSystemMetadataInfoRequestResultEvent()
			{
				TransactionID = TransactionID,
				SystemMetadataXMLDocument = xmlDoc
			});

			return true;
		}
	}
}
