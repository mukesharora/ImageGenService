using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniPageChangeCommandResultEvent : OmniCalcCommandResultEvent
	{
		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniPageChangeCommandResultEvent;
		}
	}
}
