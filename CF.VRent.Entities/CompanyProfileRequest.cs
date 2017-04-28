using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    [DataContract]
    public class CompanyProfileRequest
    {
        [DataMember]
        public UserCompanyExtenstion CompanyProfile { get; set; }

        [DataMember]
        public UserExtension VMProfile { get; set; }
    }
}
