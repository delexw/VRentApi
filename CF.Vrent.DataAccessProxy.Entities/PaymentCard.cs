using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    [Serializable]
    [DataContract]
    public class PaymentCard
    {
        [DataMember]
        public string CardNo { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Bank { get; set; }

        [DataMember]
        public string CardId { get; set; }

        [DataMember]
        public string PhoneNo { get; set; }
    }
}
