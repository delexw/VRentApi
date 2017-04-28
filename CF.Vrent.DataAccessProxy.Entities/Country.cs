using CF.VRent.Common.Entities.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public class Country : DBEntityObject
    {
        [DataMember]
        public int COUNTRY_KEY { get; set; }
        [DataMember]
        public string ISO_CODE_2 { get; set; }
        [DataMember]
        public string ISO_CODE_3 { get; set; }
        [DataMember]
        public string EN_NAME { get; set; }
        [DataMember]
        public string DE_NAME { get; set; }
        [DataMember]
        public string CN_NAME { get; set; }
    }
}
