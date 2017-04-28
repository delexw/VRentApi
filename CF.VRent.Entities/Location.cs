using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class ProxyLocation
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Text { get; set; }
    }
}
