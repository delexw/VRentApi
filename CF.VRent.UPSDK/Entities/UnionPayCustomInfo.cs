using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.UPSDK.Entities
{
    /// <summary>
    /// CustomerInfo used by UnionPay
    /// </summary>
    [Serializable]
    [DataContract]
    public class UnionPayCustomInfo
    {
        /// <summary>
        /// CardNo(but dont need in UP customerInfo, after converted to Dictionary, shoud delete it from Dictionary)
        ///</summary>
        [DataMember]
        public string CardNo { get; set; }

        [DataMember]
        public string CertifTp { get; set; }

        [DataMember]
        public string CertifId { get; set; }

        [DataMember]
        public string CustomerNm { get; set; }

        [DataMember]
        public string PhoneNo { get; set; }

        [DataMember]
        public string Expired { get; set; }

        [DataMember]
        public string Cvn2 { get; set; }

        [DataMember]
        public string SmsCode { get; set; }

        [DataMember]
        public string Pin { get; set; }

        [DataMember]
        public string CardId { get; set; }

        [DataMember]
        public string Bank { get; set; }
    }
}
