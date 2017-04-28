using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.UserStatus
{
    /// <summary>
    /// User status extension collection
    /// </summary>
    [Serializable]
    [DataContract]
    public class UserStatusExtensionEntityCollection : UserStatusEntityCollection
    {
        public UserStatusExtensionEntityCollection(IEnumerable<UserStatusExtensionEntity> extension)
            : base(extension)
        { }

        public UserStatusExtensionEntityCollection()
            : this(null)
        { }

        /// <summary>
        /// Login from portal
        /// </summary>
        public void RegFromProtal()
        {
            if (this.Count >= 2)
            {
                this["2"].Value = 1;
            }
        }

        /// <summary>
        /// Login from portal
        /// </summary>
        public void RegFromApp()
        {
            if (this.Count >= 2)
            {
                this["1"].Value = 1;
            }
        }
    }
}
