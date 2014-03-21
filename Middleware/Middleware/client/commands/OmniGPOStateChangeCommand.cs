using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client.messages;
using RFIDWebApiService.Models;

namespace Middleware.client.commands
{
	public class OmniGPOStateChangeCommand : OmniAPICommand
	{
		public int GPOPortNumber { get; set; }
		public GPIOPortState RequestedState { get; set; }
		public string RFIDDeviceID { get; set; }

		protected override void InitCommandType()
		{
			CommandType = OmniAPICommandType.OmniGPOStateChangeCommand;
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

#if REVOLUTION_SERVICE

		/**
		 * Changes the state of a GPO port on an Impinj Reader using the
		 * Revolution Service and posts a response message to the client.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			ReaderRequest readerReq = new ReaderRequest()
			{
				ReaderName = RFIDDeviceID,
				GpoStateChange = new GPIOState()
				{
					PortNum = GPOPortNumber,
					State = RequestedState == GPIOPortState.High
				}
			};

			var status = model.Clients.ReaderPostClient.Post(readerReq);

			responseMessages = new List<OmniAPIMessage>();
			OmniAPIMessage error;

			if(model.CheckWebResponse(status, this, out error))
			{
				responseMessages.Add(new OmniGPOStateChangeCommandResultEvent()
				{
					TransactionID = TransactionID,
					RFIDDeviceID = RFIDDeviceID,
					GPOPortNumber = GPOPortNumber,
					Success = true
				});

				return true;
			}
			else
			{
				responseMessages.Add(new OmniGPOStateChangeCommandResultEvent()
				{
					TransactionID = TransactionID,
					RFIDDeviceID = RFIDDeviceID,
					GPOPortNumber = GPOPortNumber,
					Success = false
				});

				responseMessages.Add(error);

				return false;
			}
		}

#else

		/**
		 * Changes the state of a GPO Port on an Impinj Reader using the
		 * OmniImpinjReader application and posts a response message to the
		 * client.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			//TODO: Implement a way of getting error messages returned from the requests

			responseMessages = new List<OmniAPIMessage>();
			OmniGPOStateChangeCommandResultEvent result = new OmniGPOStateChangeCommandResultEvent()
			{
				TransactionID = TransactionID,
				RFIDDeviceID = RFIDDeviceID,
				GPOPortNumber = GPOPortNumber
			};

			try
			{
				model.SendGPOStateChange(new OmniImpinjReaderIPC.GPOStateRequest()
				{
					RFIDReaderID = RFIDDeviceID,
					RequestedState = RequestedState == GPIOPortState.High,
					PortNumber = GPOPortNumber
				});

				result.Success = true;
			}
			catch(Exception e)
			{
				result.Success = false;
			}

			responseMessages.Add(result);

			return result.Success;
		}

#endif
	}
}
