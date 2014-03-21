using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniVisualTagHealthReportMessage : OmniAPIMessage
	{
		public string VisualTagUID { get; set; }
		public int BatteryLevel { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniVisualTagHealthReportMessage;
		}

		public override string ToString()
		{
			return String.Format("VisualTagUID: {0}, BatteryLevel: {1}",
				VisualTagUID, BatteryLevel);
		}
	}
}
