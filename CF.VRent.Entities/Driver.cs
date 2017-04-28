using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class ProxyDriver
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ID { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        

    }
}
