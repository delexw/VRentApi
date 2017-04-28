using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using CF.VRent.UPSDK;namespace CF.VRent.DataAccessProxy.Entities
{
    [Serializable]
    [DataContract]
    public class RetryBooking
    {
        [DataMember]
        public int BookingId { get; set; }
        [DataMember]
        public string OrderItemId { get; set; }
        [DataMember]
        public int PaymentId { get; set; }
        [DataMember]
        public Guid? OldCard { get; set; }
        [DataMember]
        public string PreAuthPrice { get; set; }
        [DataMember]
        public string RealPreAuthPrice { get; set; }
        [DataMember]
        public string PreAuthQueryID { get; set; }
        [DataMember]
        public string DeductionPrice { get; set; }
        [DataMember]
        public int State { get; set; }
        [DataMember]
        public Guid UserID { get; set; }
        [DataMember]
        public Guid? NewCard { get; set; }
        [DataMember]
        public PayOperationEnum Operation { get; set; }
    }
}
