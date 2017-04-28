using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public class GeneralLedgerHeader : DBEntityObject
    {
        [DataMember]
        public long ID { get; set; }

        [DataMember]
        public DateTime PostingFrom { get; set; }

        [DataMember]
        public DateTime PostingEnd { get; set; }

        [DataMember]
        public VRentDataDictionay.GLHeaderType HeaderType { get; set; }

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
