using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.UPSDK.Entities
{
    [Serializable]
    [DataContract]
    public class UnionPayTokenPay
    {
        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public string TrId { get; set; }

        [DataMember]
        public string TokenLevel { get; set; }

        [DataMember]
        public string TokenBegin { get; set; }

        [DataMember]
        public string TokenEnd { get; set; }

        [DataMember]
        public string TokenType { get; set; }
    }
}
