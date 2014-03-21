using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client.messages;
using CALCManager.Models;

namespace Middleware.client.commands
{
	public class OmniVisualTagInfoRequestCommand : OmniAPICommand
	{
		public string VisualTagUID { get; set; }

		protected override void InitCommandType()
		{
			CommandType = OmniAPICommandType.OmniVisualTagInfoRequestCommand;
			HasCALCResponse = false;
		}

		internal override bool IsValid(out OmniCommandErrorResultEvent error)
		{
			if(VisualTagUID == null)
			{
				error = OmniCommandErrorResultEvent.Missing_Field_Error;
				error.TransactionID = TransactionID;

				return false;
			}
			
			return base.IsValid(out error);
		}

		/**
		 * Requests information about a Visual Tag from the CALCMan Service and
		 * posts a message to the client with the requested information.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			var response = model.Clients.CoralsGetClient.Get(VisualTagUID);

			responseMessages = new List<OmniAPIMessage>();
			OmniAPIMessage error;

			if(model.CheckWebResponse(response, this, out error))
			{
				Coral coral = response.responseData;

				responseMessages.Add(new OmniVisualTagInfoRequestResultEvent()
				{
					BatteryLevel = coral.Battery ?? -1,
					CurrentDisplayPage = (int?)coral.CurrentPage ?? -1,
					TransactionID = TransactionID,
					VisualTagModel = coral.DisplayTypeName,
					VisualTagUID = coral.Uid
				});

				return true;
			}
			else
			{
				responseMessages.Add(error);

				return false;
			}
		}
	}
}
