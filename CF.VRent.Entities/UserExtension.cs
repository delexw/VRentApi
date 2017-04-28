using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities
{
    /// <summary>
    /// User entity
    /// </summary>
    public class UserExtension : UserExtensionHeader
    {
        
        [DataMember]
        public string PNo { get; set; }
        [DataMember]
        public int Enabled { get; set; }
        [DataMember]
        public int Blocked { get; set; }
        [DataMember]
        public int AllowChangePwd { get; set; }
        [DataMember]
        public string Department { get; set; }
        [DataMember]
        public string Mail { get; set; }
        [DataMember]
        public string CreateDate { get; set; } 
        [DataMember]
        public string PersonInCharge { get; set; }
        [DataMember]
        public string PrivateBankAccount { get; set; }
        [DataMember]
        public string PrivateEmail { get; set; }
        [DataMember]
        public string PrivateAddress { get; set; }
        [DataMember]
        public string BusinessAddress { get; set; }
        [DataMember]
        public string Valid_to { get; set; }
        
        [DataMember]
        public int TypeOfJourney { get; set; }
        [DataMember]
        public License License { get; set; }
        
        [DataMember]
        public string CurrentPassword { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string SessionID { get; set; }
        [DataMember]
        public Client[] Clients { get; set; }
        [DataMember]
        public Right[] Rights { get; set; }
        [DataMember]
        public Role[] Roles { get; set; }

        #region Additional Fields
        [DataMember]
        public string Costcenter { get; set; }
        [DataMember]
        public string Nationality { get; set; }
        [DataMember]
        public int Gender { get; set; }
        [DataMember]
        public string BirthDay { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Postcode { get; set; }
        [DataMember]
        public string Province { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string DateOfIssue { get; set; }
        #endregion

        [DataMember]
        public string Street2 { get; set; }

        #region Custom Fields
        [DataMember]
        public bool IsPrivateUser { get; set; }
        [JsonIgnore()]
        public bool IsPrivateUserBefore { get; set; }
        [DataMember]
        public string ClientID { get; set; }
        [DataMember]
        public UserLicenseExtension ProxyLicense { get; set; }
        //used by FE
        [DataMember]
        public List<ProxyJourneyType> BillingOptions { get; set; }

        [DataMember]
        public int ChangePwd { get; set; }
        #endregion
    }
}
