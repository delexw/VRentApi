using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public class BookingIndirectFee: BookingIndirectFeePayment
    {
        [DataMember]
        public string CardID { get; set; }

        [DataMember]
        public decimal Fee { get; set; }

        [DataMember]
        public int OrderID { get; set; }

        [DataMember]
        public string UserID { get; set; }

        [DataMember]
        public int[] OrderItemIDs { get; set; }
    }
}
