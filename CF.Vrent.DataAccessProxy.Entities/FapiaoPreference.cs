using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.DataAccessProxy.Entities
{
    /// <summary>
    /// FapiaoPreference Entity
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProxyFapiaoPreference
    {
        [DataMember]
        public string ID { get; set; }
        
        [DataMember]
        public string UserID { get; set; }
      
        [DataMember]
        public string CustomerName { get; set; }
      
        [DataMember]
        public string MailType { get; set; }

       
        [DataMember]
        public string MailAddress { get; set; }

        
        [DataMember]
        public string MailPhone { get; set; }

    
        [DataMember]
        public string AddresseeName { get; set; }

        
        [DataMember]
        public int FapiaoType { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public Guid CreatedBy { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public Guid? ModifiedBy { get; set; }

        [DataMember]
        public int State { get; set; }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ProxyFapiaoPreference)
            {
                ProxyFapiaoPreference pfp = obj as ProxyFapiaoPreference;
                return GetHashCode().Equals(pfp.GetHashCode());
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            Guid id = Guid.Parse(ID);
            return id.GetHashCode();
        }
    }
}
