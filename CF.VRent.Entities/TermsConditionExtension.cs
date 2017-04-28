using CF.VRent.Common.Entities;
using CF.VRent.Entities.TermsConditionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    public class TermsConditionExtension : RestfulCommonObject
    {
        [DataMember]
        public TermsCondition TC { get; set; }

        [DataMember]
        public string CreatedUserName { get; set; }

        [DataMember]
        public string CreatedDate { get; set; }
    }
}
