using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Common.UserContracts
{
    [Serializable]
    [DataContract]
    public class ProxyRight
    {
        [DataMember]
        public string RightMember { get; set; }
    }
}
