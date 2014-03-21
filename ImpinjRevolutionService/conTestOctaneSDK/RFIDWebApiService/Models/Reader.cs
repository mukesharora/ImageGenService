using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace RFIDWebApiService.Models
{
    [DataContract(Namespace = "")]
    public class Reader
    {
		[DataMember(IsRequired = true, Order = 0)]
		public string Name { get; set; }

		[DataMember(IsRequired = false, Order = 2)]
        public bool? IsConnected { get; set; }

		[DataMember(IsRequired = false, Order = 3)]
		public bool? IsReading { get; set; }

		[DataMember(IsRequired = false, Order = 4)]
		public List<GPIOState> GPIPorts { get; set; }
    }
}