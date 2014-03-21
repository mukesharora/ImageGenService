using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public enum OmniAPIMessageType
	{
		OmniRFIDDetectionMessage,
		OmniCommandErrorResultEvent,
		OmniSystemErrorEvent,
		OmniVisualTagHealthReportMessage,
		OmniPageChangeCommandResultEvent,
		OmniGPIEventMessage,
		OmniSystemMetadataInfoRequestResultEvent,
		OmniGPIStateReportCommandResultEvent,
		OmniGPOStateChangeCommandResultEvent,
		OmniImageUpdateCommandResultEvent,
		OmniGPOStateReportCommandResultEvent,
		OmniVisualTagInfoRequestResultEvent,
		OmniSystemInformationEvent,
		OmniRFIDReaderCommandResultEvent,
        OmniVisualTagAnnounceEvent,
		OmniPageDeleteCommandResultEvent
	}

    public abstract class OmniAPIMessage : EventArgs
    {
        public OmniAPIMessageType MessageType { get; protected set; }

		public OmniAPIMessage()
		{
			InitMessageType();
		}

        protected abstract void InitMessageType();
    }
}
