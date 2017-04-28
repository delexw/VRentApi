using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Common.UserContracts
{
    public enum RoleType { BookingUser, VrentManager, ServiceCenter };

    /// <summary>
    /// User setting class for user premission
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProxyUserSetting
    {

        /// result for login if 0 means successful
        /// </summary>
        [DataMember]
        public int Result { get; set; }

        /// <summary>
        /// User name for current user
        /// </summary>
        [DataMember]
        public string UName { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        [DataMember]
        public string ClientID { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public bool IsPrivateUser { get; set; }

        /// <summary>
        /// last name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// first name
        /// </summary>
        [DataMember]
        public string VName { get; set; }

        /// <summary>
        /// 1 if user is enabled
        /// </summary>
        [DataMember]
        public int Enabled { get; set; }

        /// <summary>
        /// (1 if user is blocked)
        /// </summary>
        [DataMember]
        public int Blocked { get; set; }

        /// <summary>
        /// if 0 user has to change his password 1 don't need to change
        /// </summary> 
        [DataMember]
        public int ChangePwd { get; set; }

        /// <summary>
        /// (1 if user is allowed to change his password in user data form)
        /// </summary>
        [DataMember]
        public int AllowChangePwd { get; set; }

        /// <summary>
        /// pwd for the usesetting
        /// </summary>
        [DataMember]
        public string Pwd { get; set; }

        //[DataMember]
        //public List<ProxyRight> AllRights { get; set; }

        [DataMember]
        public string SessionID { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Mail { get; set; }

        [DataMember]
        public IEnumerable<ProxyRole> AllRoles { get; set; }

        [DataMember]
        public List<ProxyRole> VrentRoles { get; set; }
    }

}
