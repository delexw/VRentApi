using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    public class GeneralLedgerLine : RestfulCommonObject
    {
        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string BusinessArea { get; set; }


        [DataMember]
        public string PostingBody { get; set; }
    }
}
