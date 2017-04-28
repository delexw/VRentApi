using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class ForgotPasswordPara
    {
         [DataMember]
        public string Email { get; set; }
         [DataMember]
        public string Lang { get; set; }
    }
}
