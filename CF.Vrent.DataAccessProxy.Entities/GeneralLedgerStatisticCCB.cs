using CF.VRent.Common.Entities.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public class GeneralLedgerStatisticCCB : DBEntityObject
    {
        [DataMember]
        public int DebitNoteID { get; set; }
        [DataMember]
        public Guid? ClientID { get; set; }
        [DataMember]
        public string CompanyCode { get; set; }
        [DataMember]
        public string BusinessArea { get; set; }
        [DataMember]
        public double CCBTotalDebit { get; set; }
        [DataMember]
        public double CCBTotalCredit { get; set; }
    }
}
