using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.FapiaoPreferenceProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class ProxyReservationsWithPagingResponse
    {
        [DataMember]
        public int ItemsPerPage { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int TotalPage { get; set; }

        [DataMember]
        public string[] RawWhereConditions { get; set; }

        [DataMember]
        public string[] RawOrderByConditions { get; set; }

        [DataMember]
        public KemasReservationEntity[] Reservations { get; set; }
    }
}
