using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client.messages;
using CALCManager.Models;

namespace Middleware.client.commands
{
    /// <summary>
    /// Notes: 
    /// 1. The syntax of ImageURL validated against RFC 2396 and RFC 2732 using Uri.IsWellFormedUriString.
    /// 2. Example local url: "file:///C:/temp/image.bmp".
    /// </summary>
	public class OmniImageUrlUpdateCommand: OmniAPICommand
	{
		public int PageNumber { get; set; }
		public string VisualTagUID { get; set; }
		public string ImageURL { get; set; }
		public string GatewayID { get; set; }
        public bool IsTransactional { get; set; }

		protected override void InitCommandType()
		{
			CommandType = OmniAPICommandType.OmniImageUrlUpdateCommand;
			HasCALCResponse = true;
		}

		internal override bool IsValid(out OmniCommandErrorResultEvent error)
		{
			if(VisualTagUID == null || ImageURL == null)
			{
				error = OmniCommandErrorResultEvent.Missing_Field_Error;
				error.TransactionID = TransactionID;

				return false;
			}
			else if(!Uri.IsWellFormedUriString(ImageURL, UriKind.RelativeOrAbsolute) ||
				PageNumber <= 0 || !ImageURL.ToLower().EndsWith(".bmp"))
			{
				error = OmniCommandErrorResultEvent.Invalid_Field_Error;
				error.TransactionID = TransactionID;

				return false;
			}

			return base.IsValid(out error);
		}

		/**
		 * Sends the image at the given URL to an Active Tag using the CALC and
		 * posts status messages to the client as needed.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			Coral coral = new Coral()
			{
				CurrentPage = (uint)PageNumber,
				ImageUrl = ImageURL,
				Uid = VisualTagUID,
				CalcId = GatewayID,
                IsTransactionalCommand = IsTransactional
			};

			var coralUpdateStatus = model.Clients.CoralsPostClient.Post(coral);

			responseMessages = new List<OmniAPIMessage>();
			OmniAPIMessage error;

			if(model.CheckWebResponse(coralUpdateStatus, this, out error))
			{
				CALCResponseHandle = coralUpdateStatus.responseData.Handle;

				responseMessages.Add(new OmniImageUpdateCommandResultEvent()
				{
					TransactionID = TransactionID,
					VisualTagUID = coral.Uid
				}.SetCommandQueued());

				return true;
			}
			else
			{
				responseMessages.Add(new OmniImageUpdateCommandResultEvent()
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
