using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client.messages;
using RFIDWebApiService.Models;

namespace Middleware.client.commands
{
	public class OmniRFIDReaderCommand : OmniAPICommand
	{
		public enum ReaderState
		{
			StartReading,
			StopReading
		}

		public string RFIDDeviceID { get; set; }
		public ReaderState RequestedReaderState { get; set; }

		protected override void InitCommandType()
		{
			CommandType = OmniAPICommandType.OmniRFIDReaderCommand;
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

			return base.IsValid(out error);
		}

#if REVOLUTION_SERVICE

		/**
		 * Starts or stops an Impinj Reader using the Revolution Service and
		 * posts a response message to the client.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			ReaderRequest readerReq = new ReaderRequest()
			{
				ReaderName = RFIDDeviceID,
				ReaderStateChange = RequestedReaderState == OmniRFIDReaderCommand.ReaderState.StartReading
			};

			var status = model.Clients.ReaderPostClient.Post(readerReq);

			responseMessages = new List<OmniAPIMessage>();
			OmniAPIMessage error;

			if(model.CheckWebResponse(status, this, out error))
			{
				responseMessages.Add(new OmniRFIDReaderCommandResultEvent()
				{
					TransactionID = TransactionID,
					RFIDDeviceID = RFIDDeviceID,
					Success = true
				});

				return true;
			}
			else
			{
				responseMessages.Add(new OmniRFIDReaderCommandResultEvent()
				{
					TransactionID = TransactionID,
					RFIDDeviceID = RFIDDeviceID,
					Success = false
				});

				responseMessages.Add(error);

				return false;
			}
		}

#else

		/**
		 * Starts or stops an Impinj Reader using the OmniImpinjReader
		 * application and posts a response message to the client.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			//TODO: Implement a way of getting error messages returned from the requests

			responseMessages = new List<OmniAPIMessage>();
			OmniRFIDReaderCommandResultEvent result = new OmniRFIDReaderCommandResultEvent()
			{
				TransactionID = TransactionID,
				RFIDDeviceID = RFIDDeviceID
			};

			try
			{
				model.SendReaderStateChange(new OmniImpinjReaderIPC.ReaderStateRequest()
				{
					RFIDReaderID = RFIDDeviceID,
					RequestedState = RequestedReaderState == ReaderState.StartReading
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
