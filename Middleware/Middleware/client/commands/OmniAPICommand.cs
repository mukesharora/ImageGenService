using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client.messages;

namespace Middleware.client.commands
{
	public enum OmniAPICommandType
	{
		OmniGPIStateReportCommand,
		OmniGPOStateChangeCommand,
		OmniGPOStateReportCommand,
		OmniPageChangeCommand,
		OmniImageUpdateCommand,
		OmniSystemMetadataInfoRequestCommand,
		OmniVisualTagInfoRequestCommand,
		OmniRFIDReaderCommand,
		OmniImageUrlUpdateCommand,
		OmniPageDeleteCommand
	}

	public abstract class OmniAPICommand
	{
		public OmniAPICommandType CommandType { get; protected set; }
		public Guid TransactionID { get; set; }

		public OmniAPICommand()
		{
			InitCommandType();
		}

		protected abstract void InitCommandType();

		protected internal bool HasCALCResponse { get; protected set; }
		protected internal int? CALCResponseHandle { get; protected set; }

		internal virtual bool IsValid(out OmniCommandErrorResultEvent error)
		{
			if(TransactionID == null)
			{
				error = OmniCommandErrorResultEvent.No_TID_Error;
				return false;
			}

			error = null;
			return true;
		}

		abstract internal bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages);
	}
}
