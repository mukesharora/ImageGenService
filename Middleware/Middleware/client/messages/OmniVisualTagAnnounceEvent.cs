using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
    public class OmniVisualTagAnnounceEvent : OmniAPIMessage
    {
        public string VisualTagUID { get; set; }
        
        protected override void InitMessageType()
        {
            MessageType = OmniAPIMessageType.OmniVisualTagAnnounceEvent;
        }

        public override string ToString()
        {
            return String.Format("VisualTagUID: {0}",
                VisualTagUID);
        }
    }
}
