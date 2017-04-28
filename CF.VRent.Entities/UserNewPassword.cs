using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class UserNewPassword
    {
        [DataMember]
        public string Current { get; set; }

        [DataMember]
        public string New { get; set; }

        [DataMember]
        public string ChangePwd { get; set; }
    }
}
