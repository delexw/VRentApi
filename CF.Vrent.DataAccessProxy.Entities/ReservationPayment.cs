using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    [Serializable]
    [DataContract]
    public class ProxyReservationPayment
    {
        [DataMember]
        public string KemasBookingID { get; set; }

        [DataMember]
        public string UserID { get; set; }

        [DataMember]
        public int ProxyBookingID { get; set; }

        [DataMember]
        public string PriceDetials { get; set; }

        [DataMember]
        public string KemasBookingNumber { get; set; }

        [DataMember]
        public string CardID { get; set; }

        [DataMember]
        public int PaymentID { get; set; }

        [DataMember]
        public string PriceStructure { get; set; }

        [DataMember]
        public int BookingType { get; set; }

        [DataMember]
        public string KemasState { get; set; }
    }
}
