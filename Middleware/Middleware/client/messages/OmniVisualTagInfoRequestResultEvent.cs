using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniVisualTagInfoRequestResultEvent : OmniAPIMessage
	{
		public Guid TransactionID { get; set; }
		public string VisualTagUID { get; set; }
		public string VisualTagModel { get; set; }
		public int BatteryLevel { get; set; }
		public int CurrentDisplayPage { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniVisualTagInfoRequestResultEvent;
		}

		public override string ToString()
		{
			return String.Format("TransactionID: {0}, VisualTagUID: {1}, VisualTagModel: {2}, BatteryLevel: {3}, CurrentDisplayPage: {4}",
				TransactionID, VisualTagUID, VisualTagModel, BatteryLevel, CurrentDisplayPage);
		}
	}
}
