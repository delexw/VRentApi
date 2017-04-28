using CF.VRent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    [Serializable]
    [DataContract]
    public class ProxyFapiaoRequest
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int ProxyBookingID { get; set; }

        [DataMember]
        public Guid? FapiaoPreferenceID { get; set; }

        [DataMember]
        public ProxyFapiaoPreference FapiaoPreference { get; set; }

        [DataMember]
        public int FapiaoSource { get; set; }

        [DataMember]
        public int State { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        public Guid? CreatedBy { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public Guid? ModifiedBy { get; set; }
    }
}
