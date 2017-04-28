using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Entities
{
    [Serializable]
    [DataContract]
    public class ReponseRes<T>
    {
        [DataMember]
        public int Result{get;set;}
        [DataMember]
        public string message{get; set;}

        [DataMember]
        public float DuringTime { get; set; }


        [DataMember]
        public T Data { get; set; }
    }
}
