using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public class GeneralLedgerItem : DBEntityObject
    {
        [DataMember]
        public long ID { get; set; }

        [DataMember]
        public long HeaderID { get; set; }

        [DataMember]
        public VRentDataDictionay.GLItemType ItemType { get; set; }

        [DataMember]
        public string PostingBody { get; set; }

        [DataMember]
        public Guid ClientID { get; set; }

        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string BusinessArea { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        public Guid? CreatedBy { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public Guid? ModifiedBy { get; set; }
    }
}
