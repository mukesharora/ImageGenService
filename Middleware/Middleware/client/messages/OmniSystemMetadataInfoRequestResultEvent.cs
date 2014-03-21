using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniSystemMetadataInfoRequestResultEvent : OmniAPIMessage
	{
		public Guid TransactionID { get; set; }
		public string SystemMetadataXMLDocument { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniSystemMetadataInfoRequestResultEvent;
		}

		public override string ToString()
		{
			return String.Format("TransactionID: {0}, SystemMetadataXMLDocument: {1}",
				TransactionID, SystemMetadataXMLDocument);
		}
	}
}
