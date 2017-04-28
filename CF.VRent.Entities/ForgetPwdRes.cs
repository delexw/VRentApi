using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CF.VRent.Entities
{

    [Serializable]
    [DataContract]
    public class ForgotPwdRes
    {
        /// <summary>
        /// (if 0, there is no error occurred)
        /// </summary>
         [DataMember]
        public int Result { get; set; }

        /// <summary>
        /// (if 1, there is no error occurred)
        /// </summary>
         [DataMember]
        public int success { get; set; }

        /// <summary>
        /// if there is an error, error message returned
        /// </summary>
         [DataMember]
        public string message { get; set; }
    }
}
