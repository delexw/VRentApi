using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.UPSDK.Entities
{
    [Serializable]
    [DataContract]
    public class UnionPay
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string Encoding { get; set; }

        [DataMember]
        public string CertId { get; set; }

        [DataMember]
        public string Signature { get; set; }

        [DataMember]
        public string SignMethod { get; set; }

        [DataMember]
        public string TxnType { get; set; }

        [DataMember]
        public string TxnSubType { get; set; }

        [DataMember]
        public string BizType { get; set; }

        [DataMember]
        public string AccessType { get; set; }

        [DataMember]
        public string ChannelType { get; set; }

        [DataMember]
        public string MerId { get; set; }

        [DataMember]
        public string OrderId { get; set; }

        [DataMember]
        public string TxnTime { get; set; }

        [DataMember]
        public string AccNo { get; set; }

        [DataMember]
        public string AccType { get; set; }

        public string CustomerInfo { get; set; }

        [DataMember]
        public string EncryptCertId { get; set; }

        [DataMember]
        public string TokenPayData { get; set; }

        [DataMember]
        public string RespCode { get; set; }

        [DataMember]
        public string RespMsg { get; set; }

        [DataMember]
        public string ActivateStatus { get; set; }

        [DataMember]
        public string IssInsCode { get; set; }

        [DataMember]
        public string BackUrl { get; set; }

        [DataMember]
        public string TxnAmt { get;set;}

        [DataMember]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Used to identity preauthorization
        /// </summary>
        [DataMember]
        public string QueryId { get; set; }

        [DataMember]
        public string SettleAmt { get; set; }

        [DataMember]
        public string SettleCurrencyCode { get; set; }

        [DataMember]
        public string SettleDate { get; set; }

        [DataMember]
        public string TraceNo { get; set; }

        [DataMember]
        public string TraceTime { get; set; }

        [DataMember]
        public string PreAuthId { get; set; }

        [DataMember]
        public string OrigQryId { get; set; }

        /// <summary>
        /// RespCode ( Return when succeeded to check payment status)
        /// </summary>
        [DataMember]
        public string OrigRespCode { get; set; }

        [DataMember]
        public string OrigRespMsg { get; set; }

        /// <summary>
        /// Not belong to Unionpay api remember to exclude it before sending request
        /// </summary>
        [DataMember]
        public string UniqueID { get; set; }

        [DataMember]
        public string ReqReserved { get; set; }
    }
}
