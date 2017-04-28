using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    public class UserCompanyExtenstion : RestfulCommonObject
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public int Enabled { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public string ContactPerson { get; set; }

        #region Address
        //kemas field
        [DataMember]
        public string Address { get; set; }

        //FE fields
        [DataMember]
        public string RegisteredAddress { get; set; }

        [DataMember]
        public string OfficeAddress { get; set; }
        #endregion

        [DataMember]
        public string ContactInfo { get; set; }
        [DataMember]
        public int CountEmployees { get; set; }
        [DataMember]
        public int Deposit { get; set; }

        #region BankAccount Info
        //kemas Field
        [DataMember]
        public string BankAccountInfo { get; set; }

        //FEFields
        [DataMember]
        public string BankAccountName { get; set; }

        //FEFields
        [DataMember]
        public string BankAccountNo { get; set; }

        #endregion

        [DataMember]
        public string BusinessLicenseID { get; set; }
        [DataMember]
        public string OrgCodeCertificate { get; set; }
        [DataMember]
        public string LegalRepresentativeID { get; set; }
        [DataMember]
        public string Mail { get; set; }

        [DataMember]
        public string Status { get; set; }
    }

    public class Status 
    {
        public int status { get; set; }
    }
}
