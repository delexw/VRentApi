using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public enum RoleType { BookingUser,VrentManager,ServiceCenter };

    [Serializable]
    [DataContract]
    public class UserProfile
    {
        [DataMember]
        public int BillingOption { get; set; }

        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public Guid UserID { get; set; }

        [DataMember]
        public Guid? CorporateID { get; set; }

    }
}
