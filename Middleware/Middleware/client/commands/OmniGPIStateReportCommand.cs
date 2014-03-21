using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client.messages;
using RFIDWebApiService.Models;

namespace Middleware.client.commands
{
	public class OmniGPIStateReportCommand : OmniAPICommand
	{
		public int GPIPortNumber { get; set; }
		public string RFIDDeviceID { get; set; }

		protected override void InitCommandType()
		{
			CommandType = OmniAPICommandType.OmniGPIStateReportCommand;
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
			else if(GPIPortNumber <= 0 || GPIPortNumber > 4)
			{
				error = OmniCommandErrorResultEvent.Invalid_Field_Error;
				error.TransactionID = TransactionID;

				return false;
			}

			return base.IsValid(out error);
		}

#if REVOLUTION_SERVICE

		/**
		 * Queries the state of a GPI port on an Impinj Reader using the
		 * Revolution Service and posts the result to the client.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			List<string> gpiQueryParams = new List<string>() { RFIDDeviceID, GPIPortNumber.ToString() };
			var response = model.Clients.ReaderGetClient.Get(gpiQueryParams);

			responseMessages = new List<OmniAPIMessage>();
			OmniAPIMessage error;

			if(model.CheckWebResponse(response, this, out error))
			{
				GPIOState state = response.responseData;

				responseMessages.Add(new OmniGPIStateReportCommandResultEvent()
				{
					TransactionID = TransactionID,
					GPIPortNumber = GPIPortNumber,
					PortState = model.ConvertGPIOState(state),
					RFIDDeviceID = RFIDDeviceID
				});

				return true;
			}
			else
			{
				responseMessages.Add(error);

				return false;
			}
		}

#else

		/**
		 * Not implemented.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			throw new NotImplementedException();
		}

#endif
	}
}
