using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class ProxyCheckPriceInfo
    {
        [DataMember]
        public string KemasBookingID { get; set; }
        
        [DataMember]
        public string DateBegin { get; set; }

        [DataMember]
        public string DateEnd { get; set; }

        [DataMember]
        public string DriverID { get; set; }

        [DataMember]
        public string BillingOption { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string StartLocationID { get; set; }
 
    }
}
