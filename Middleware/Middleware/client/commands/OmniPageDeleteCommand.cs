using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client.messages;
using CALCManager.Models;

namespace Middleware.client.commands
{
	public class OmniPageDeleteCommand : OmniAPICommand
	{
		public int PageNumber { get; set; }
		public string VisualTagUID { get; set; }
		public string GatewayID { get; set; }
        public bool IsTransactional { get; set; }

		protected override void InitCommandType()
		{
			CommandType = OmniAPICommandType.OmniPageDeleteCommand;
			HasCALCResponse = true;
		}

		internal override bool IsValid(out OmniCommandErrorResultEvent error)
		{
			if(VisualTagUID == null)
			{
				error = OmniCommandErrorResultEvent.Missing_Field_Error;
				error.TransactionID = TransactionID;

				return false;
			}
			else if(PageNumber <= 0)
			{
				error = OmniCommandErrorResultEvent.Invalid_Field_Error;
				error.TransactionID = TransactionID;

				return false;
			}
			
			return base.IsValid(out error);
		}

		/**
		 * Sends a Delete Page command to an Active Tag using the CALC and posts
		 * status messages to the client as needed.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			Coral coral = new Coral()
			{
				Uid = VisualTagUID,
				CurrentPage = (uint)PageNumber,
				ImageId = Coral.ID_NO_IMAGE,
				CalcId = GatewayID,
                IsTransactionalCommand = IsTransactional
			};

			var status = model.Clients.CoralsPostClient.Post(coral);

			responseMessages = new List<OmniAPIMessage>();
			OmniAPIMessage error;

			if(model.CheckWebResponse(status, this, out error))
			{
				CALCResponseHandle = status.responseData.Handle;

				responseMessages.Add(new OmniPageDeleteCommandResultEvent()
				{
					TransactionID = TransactionID,
					VisualTagUID = VisualTagUID
				}.SetCommandQueued());

				return true;
			}
			else
			{
				responseMessages.Add(new OmniPageDeleteCommandResultEvent()
				{
					TransactionID = TransactionID,
					VisualTagUID = coral.Uid
				}.SetCommandQueuedFailed());

				responseMessages.Add(error);

				return false;
			}
		}
	}
}
