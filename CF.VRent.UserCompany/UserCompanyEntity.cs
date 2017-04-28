using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.UserCompany
{
    [Serializable]
    [DataContract]
    public class UserCompanyEntity
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public KemasCompanyEntity[] KemasCompany { get; set; }

        [DataMember]
        public bool Enable { get; set; }

        /// <summary>
        /// Get a array which include kemas company id
        /// </summary>
        /// <param name="kemasCompany"></param>
        /// <returns></returns>
        public string[] GetKemasCompanyIdSets()
        {
            List<string> ids = new List<string>();

            foreach (KemasCompanyEntity e in KemasCompany)
            {
                if (!String.IsNullOrWhiteSpace(e.ID) && e.Enable)
                {
                    ids.Add(e.ID);
                }
            }

            return ids.ToArray();
        }

        /// <summary>
        /// Get the default company id
        /// </summary>
        /// <returns></returns>
        public string GetDefaultKemasCompanyId()
        {
            return KemasCompany.FirstOrDefault(r => !String.IsNullOrWhiteSpace(r.ID) && r.Enable && r.IsDefault).ID;
        }

        /// <summary>
        /// Get the default company
        /// </summary>
        /// <returns></returns>
        public KemasCompanyEntity GetDefaultKemasCompany()
        {
            return KemasCompany.FirstOrDefault(r => r.Enable && r.IsDefault);
        }
    }

    [Serializable]
    [DataContract]
    public class KemasCompanyEntity
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public bool Enable { get; set; }
        [DataMember]
        public bool IsDefault { get; set; }

        /// <summary>
        /// CustomerCode for accounting
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// CompanyCode for accounting
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
