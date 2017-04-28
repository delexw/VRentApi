using CF.VRent.Common.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Common.Entities
{
    [Serializable]
    [DataContract]
    public class RestfulCommonObject : IRestfulCommon
    {
        [DataMember]
        public ReturnResult RestfulResult
        {
            get;
            set;
        }
    }
}
