using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client.messages;

namespace Middleware.client.commands
{
	public class OmniGPOStateReportCommand : OmniAPICommand
	{
		public int GPOPortNumber { get; set; }
		public string RFIDDeviceID { get; set; }

		protected override void InitCommandType()
		{
			CommandType = OmniAPICommandType.OmniGPOStateReportCommand;
			HasCALCResponse = false;
		}

		internal override bool IsValid(out OmniCommandErrorResultEvent error)
		{
			if(RFIDDeviceID == null)
			{
				error = OmniCommandErrorResultEvent.Missing_Field_Error;
				error.TransactionID = TransactionID;

				return false;
			}
			else if(GPOPortNumber <= 0 || GPOPortNumber > 4)
			{
				error = OmniCommandErrorResultEvent.Invalid_Field_Error;
				error.TransactionID = TransactionID;

				return false;
			}

			return base.IsValid(out error);
		}

		/**
		 * Not implemented.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			throw new NotImplementedException();
		}
	}
}
