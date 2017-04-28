using CF.VRent.Entities.AccountingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class DebitNotePaymentState
    {
        [DataMember]
        public PaymentState State { get; set; }
    }
}
