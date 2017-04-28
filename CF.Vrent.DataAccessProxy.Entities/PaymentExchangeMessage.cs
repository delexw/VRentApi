using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Common.Entities.DBEntity;

namespace CF.VRent.DataAccessProxy.Entities
{
    public class PaymentExchangeMessage : DBEntityObject
    {
        /// <summary>
        /// ID(Guid)
        /// </summary>
        [DataMember]
        public string UniqueID { get; set; }
        /// <summary>
        /// No use temporarily
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public int State { get; set; }
        /// <summary>
        /// ID(Int)
        /// </summary>
        [DataMember]
        public int PaymentID { get; set; }
        [DataMember]
        public int RetryCount { get; set; }
        [DataMember]
        public string Operation { get; set; }
        [DataMember]
        public string PreAuthID { get; set; }

        [DataMember]
        public string PreAuthQueryID { get; set; }

        [DataMember]
        public string PreAuthDateTime { get; set; }

        [DataMember]
        public string PreAuthTempOrderID { get; set; }

        [DataMember]
        public string PreAuthPrice { get; set; }

        [DataMember]
        public string DeductionPrice { get; set; }

        [DataMember]
        public string UserID { get; set; }

        /// <summary>
        /// Binding card ID
        /// </summary>
        [DataMember]
        public string CardID { get; set; }

        [DataMember]
        public string SmsCode { get; set; }

        [DataMember]
        public int LastPaymentID { get; set; }

        [DataMember]
        public string RealPreAuthPrice { get; set; }

        [DataMember]
        public int Retry { get; set; }

        [DataMember]
        public string ReservedMsg { get; set; }

        [DataMember]
        public int ProxyBookingID { get; set; }

        public PaymentExchangeMessage()
        {
            Retry = VRentDataDictionay.TransactionRetry.Default.GetValue();
        }
    }
}
