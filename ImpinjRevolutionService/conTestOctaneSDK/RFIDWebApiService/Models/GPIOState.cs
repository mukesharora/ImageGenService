using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RFIDWebApiService.Models
{
	[DataContract(Namespace = "")]
	public class GPIOState
	{
		[DataMember(IsRequired = true, Order = 0)]
		public int PortNum { get; set; }

		[DataMember(IsRequired = true, Order = 1)]
		public bool State { get; set; }
	}
}