using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
	public class OmniCommandErrorResultEvent : OmniAPIMessage
	{
		public Guid TransactionID { get; set; }
		public string ErrorCode { get; set; }
		public string Information { get; set; }

		protected override void InitMessageType()
		{
			MessageType = OmniAPIMessageType.OmniCommandErrorResultEvent;
		}

		public override string ToString()
		{
			return String.Format("TransactionID: {0}, ErrorCode: {1}, Information: {2}",
				TransactionID, ErrorCode, Information);
		}

		#region Pre-baked Errors

		private const string MISSING_FIELD = "Field Missing";
		private const string BAD_COMMAND = "Bad Command";
		private const string REJECTED_COMMAND = "Rejected Command";

		static internal OmniCommandErrorResultEvent No_TID_Error
		{
			get
			{
				return new OmniCommandErrorResultEvent()
				{
					ErrorCode = MISSING_FIELD,
					Information = "No TransactionID specified."
				};
			}
		}

		static internal OmniCommandErrorResultEvent Missing_Field_Error
		{
			get
			{
				return new OmniCommandErrorResultEvent()
				{
					ErrorCode = MISSING_FIELD,
					Information = "One or more required fields is missing."
				};
			}
		}

		static internal OmniCommandErrorResultEvent Unsupported_Command_Error
		{
			get
			{
				return new OmniCommandErrorResultEvent()
				{
					ErrorCode = BAD_COMMAND,
					Information = "The given Command Type is unsupported."
				};
			}
		}

		static internal OmniCommandErrorResultEvent Invalid_Field_Error
		{
			get
			{
				return new OmniCommandErrorResultEvent()
				{
					ErrorCode = BAD_COMMAND,
					Information = "The data in one or more fields is invalid."
				};
			}
		}

		static internal OmniCommandErrorResultEvent Rejected_Command_Error
		{
			get
			{
				return new OmniCommandErrorResultEvent()
				{
					ErrorCode = REJECTED_COMMAND,
					Information = "The remote server rejected the command:\n"
				};
			}
		}

		#endregion
	}
}
