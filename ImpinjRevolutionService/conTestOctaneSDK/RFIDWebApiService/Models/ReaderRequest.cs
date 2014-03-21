using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace RFIDWebApiService.Models
{
	[DataContract(Namespace = "", Name = "request")]
	public class ReaderRequest
	{
		[DataMember(Name = "reader_name", IsRequired = true, Order = 0)]
		public string ReaderName { get; set; }

		[DataMember(Name = "gpo_change", IsRequired = false, Order = 1)]
		public GPIOState GpoStateChange { get; set; }

		[DataMember(Name = "reader_change", IsRequired = false, Order = 2)]
		public bool? ReaderStateChange { get; set; }
	}
}