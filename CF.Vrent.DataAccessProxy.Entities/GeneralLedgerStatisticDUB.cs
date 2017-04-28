using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.UPSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public class GeneralLedgerStatisticDUB : DBEntityObject
    {
        /// <summary>
        /// ProxyBookingID
        /// </summary>
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public Guid UserID { get; set; }
        [DataMember]
        public int BookingType { get; set; }
        [DataMember]
        public VRentDataDictionay.GLItemDetailType FeeType { get; set; }
        [DataMember]
        public int RentPaymentID { get; set; }
        [DataMember]
        public double RentCreditPrice { get; set; }
        [DataMember]
        public double RentDebitPrice { get; set; }
        [DataMember]
        public DateTime? RentalTime { get; set; }
        [DataMember]
        public PaymentStatusEnum RentalPaymentStatus { get; set; }
        [DataMember]
        public Guid? ClientID { get; set; }
        [DataMember]
        public string CompanyCode { get; set; }
        [DataMember]
        public string BusinessArea { get; set; }
    }
}
