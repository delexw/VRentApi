using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    public class Payment : RestfulCommonObject
    {
        [DataMember]
        public int PaymentID { get; set; }

        [DataMember]
        public string TempOrderId { get; set; }

        [DataMember]
        public string TempOrderTime { get; set; }
    }
}
