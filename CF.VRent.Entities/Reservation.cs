using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class Reservation
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public int UPPaymentID { get; set; }

        [DataMember]
        public string Creator { get; set; }

        [DataMember]
        public string Driver { get; set; }

        [DataMember]
        public string DateBegin { get; set; }

        [DataMember]
        public string DateEnd { get; set; }

        [DataMember]
        public string StartLocation { get; set; }

        [DataMember]
        public string BillingOption { get; set; }

        [DataMember]
        public string VehicleCategory { get; set; }
    }
}
