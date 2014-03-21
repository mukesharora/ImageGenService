using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client.messages;
using ImageGenModels;
using CALCManager.Models;

namespace Middleware.client.commands
{
	public class OmniImageUpdateCommand : OmniAPICommand
	{
		public int? ImageTemplateID { get; set; }
        public CoralTypes CoralType { get; set; }
		public Dictionary<string, string> ImageTemplateParameters { get; set; }
		public int PageNumber { get; set; }
		public string VisualTagUID { get; set; }
		public string GatewayID { get; set; }
        public bool IsTransactional { get; set; }

		protected override void InitCommandType()
		{
			CommandType = OmniAPICommandType.OmniImageUpdateCommand;
			HasCALCResponse = true;
		}

		internal override bool IsValid(out OmniCommandErrorResultEvent error)
		{
			if(VisualTagUID == null || ImageTemplateID == null ||
				ImageTemplateParameters == null)
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
		 * Creates an image using the ImageGen Service and sends the image to
		 * an Active Tag using the CALC and posts status messages to the client
		 * as needed.
		 */
		internal override bool Execute(MiddlewareCommandModel model, out List<OmniAPIMessage> responseMessages)
		{
			List<ImageField> fields = new List<ImageField>();
			bool isFirst = true;

			//Translate string dictionary to list of ImageFields
			foreach(string fieldKey in ImageTemplateParameters.Keys)
			{
				fields.Add(new ImageField()
				{
					BarcodeType = isFirst ? BarcodeType.ONE_D : BarcodeType.NONE,
					Field = ImageTemplateParameters[fieldKey]
				});

				isFirst = false;
			}

			ImageData imgData = new ImageData()
			{
				CoralType = this.CoralType,
				Data = fields,
				TemplateNum = ImageTemplateID.Value
			};

			//Request a url to an image with the given fields
			var imageGenStatus = model.Clients.ImageGenPostClient.Post(imgData);

			responseMessages = new List<OmniAPIMessage>();
			OmniAPIMessage error;

			if(model.CheckWebResponse(imageGenStatus, this, out error))
			{
				Coral coral = new Coral()
				{
					CurrentPage = (uint)PageNumber,
					ImageUrl = imageGenStatus.responseData,
					Uid = VisualTagUID,
					CalcId = GatewayID,
                    IsTransactionalCommand = IsTransactional
				};

				var coralUpdateStatus = model.Clients.CoralsPostClient.Post(coral);

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
				}
			}
			else
			{
				responseMessages.Add(error);
			}

			return false;
		}
	}
}
