using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniSystemErrorEvent : OmniAPIMessage
	{
		public string ErrorCode { get; set; }
		public string Information { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniSystemErrorEvent;
		}

		public override string ToString()
		{
			return String.Format("ErrorCode: {0}, Information: {1}",
				ErrorCode, Information);
		}

		#region Pre-baked Errors

		private const string SERVER_ERROR = "Server Error";
		private const string WEB_ERROR = "Web Error";
		private const string RFID_ERROR = "RFID Reader Error";
        private const string UNSUPPORTED_MESSAGE_TYPE_ERROR = "Unsupported message type Error";


		static internal OmniSystemErrorEvent Server_Unreachable_Error
		{
			get
			{
				return new OmniSystemErrorEvent()
				{
					ErrorCode = SERVER_ERROR,
					Information = "The remote server is unreachable."
				};
			}
		}

		static internal OmniSystemErrorEvent Internal_Server_Error
		{
			get
			{
				return new OmniSystemErrorEvent()
				{
					ErrorCode = SERVER_ERROR,
					Information = "The remote server encountered an error:\n"
				};
			}
		}

		static internal OmniSystemErrorEvent RFID_Reader_Error
		{
			get
			{
				return new OmniSystemErrorEvent()
				{
					ErrorCode = RFID_ERROR,
					Information = "The RFID Revolution Service encountered an error:\n"
				};
			}
		}

		static internal OmniSystemErrorEvent Web_Error
		{
			get
			{
				return new OmniSystemErrorEvent()
				{
					ErrorCode = WEB_ERROR,
					Information = "A Web Exception has been encountered:\n"
				};
			}
		}

        /// <summary>
        /// Instantiates an OmniSystemErrorEvent object to report that
        /// we got a message (from Calman, OmniInpinj, etc),
        /// with a unsupported message type that we don't currently 
        /// support. Caller should fill in the Information field.
        /// </summary>
        static internal OmniSystemErrorEvent Unsupported_Message_Type
        {
            get
            {
                return new OmniSystemErrorEvent()
                {
                    ErrorCode = UNSUPPORTED_MESSAGE_TYPE_ERROR,                    
                };
            }
        }

		#endregion
	}
}
