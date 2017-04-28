using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    public class UserLicenseExtension : RestfulCommonObject
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string RFID { get; set; }
        [DataMember]
        public int State { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int Lic_Expired { get; set; }
        [DataMember]
        public string ExpireDate { get; set; }
        [DataMember]
        public string DateOfIssue { get; set; }
        [DataMember]
        public string LicenseNumber { get; set; }
        [DataMember]
        public string LastCheck { get; set; }
        [DataMember]
        public int PIN { get; set; }
        [DataMember]
        public bool PINSpecified { get; set; }
        [DataMember]
        public int PIN2 { get; set; }
        [DataMember]
        public bool PIN2Specified { get; set; }
        [DataMember]
        public int UsePIN { get; set; }
        [DataMember]        
        public bool UsePINSpecified { get; set; }
        [DataMember]
        public int ChangePIN { get; set; }
        [DataMember]
        public bool ChangePINSpecified { get; set; }
        [DataMember]
        public int ChangePINWithIdentification { get; set; }
        [DataMember]        
        public bool ChangePINWithIdentificationSpecified { get; set; }
    }
}
