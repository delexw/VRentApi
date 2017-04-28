using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.UserCompany;
using CF.VRent.UserRole;
using CF.VRent.UserStatus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text; 

namespace CF.VRent.Entities
{
    public class UserExtensionHeader : RestfulCommonObject
    {
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string[] StatusName { get; set; }
        [DataMember]
        public UserStatusEntityCollection StatusEntities { get; set; }
        [DataMember]
        public UserStatusExtensionEntityCollection StatusExtensionEntities { get; set; }
        [JsonIgnore()]
        public UserStatusEntityCollection OriginalStatusEntities { get; set; }
        [DataMember]
        public string RepeatPassword { get; set; }
        [DataMember]
        public UserRoleEntityCollection RoleEntities { get; set; }
        [DataMember]
        public UserComanyEntityCollection ComanyEntites { get; set; }
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Company { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string VName { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string PrivateMobileNumber { get; set; }
    }
}
