using CF.VRent.Common.Entities.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Common.UserContracts
{
    [DataContract]
    public class ProxyRole : DBEntityObject
    {
        [DataMember]
        public string RoleMember { get; set; }
    }
}
