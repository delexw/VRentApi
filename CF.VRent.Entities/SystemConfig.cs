using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class SystemConfig
    {
        [DataMember]
        public long BookingMaxDuration { get; set; }

        [DataMember]
        public long BookingAheadTime { get; set; }

        [DataMember]
        public long BookingBufferTime { get; set; }

        [DataMember]
        public long BookingMaxLeadTime { get; set; }

        [DataMember]
        public long BookingMaxWaitTime { get; set; }

        [DataMember]
        public long LostItemEnabled {  get;set; }

        [DataMember]
        public long LostItemTime {  get;set; }
    }
}
