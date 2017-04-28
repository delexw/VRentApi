using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.UserRole
{
    [Serializable]
    [DataContract]
    public class UserRoleEntity
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public KemasRoleEntity[] KemasRole { get; set; }

        [DataMember]
        public bool Enable { get; set; }

        /// <summary>
        /// Get a array which include kemas role id
        /// </summary>
        /// <param name="kEntity"></param>
        /// <returns></returns>
        public string[] GetKemasRoleIdSets()
        {
            List<string> ids = new List<string>();

            foreach (KemasRoleEntity e in KemasRole)
            {
                if (!String.IsNullOrWhiteSpace(e.ID) && e.Enable)
                {
                    ids.Add(e.ID);
                }
            }

            return ids.ToArray();
        }

        /// <summary>
        /// Get the default role id
        /// </summary>
        /// <returns></returns>
        public KemasRoleEntity GetDefaultKemasRole()
        {
            return KemasRole.FirstOrDefault(r => r.Enable && r.IsDefault);
        }
    }

    [Serializable]
    [DataContract]
    public class KemasRoleEntity
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public bool Enable { get; set; }
        [DataMember]
        public bool IsDefault { get; set; }
    }
}
