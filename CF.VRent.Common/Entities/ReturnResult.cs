using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Common.Entities
{
    [Serializable]
    [DataContract]
    public class ReturnResult
    {

        [DataMember]
        public int Success { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public List<string> StackTrace { get; set; }

        [DataMember]
        public string StackFrameMethod { get; set; }

        [DataMember]
        public ResultType Type { get; set; }

        /// <summary>
        /// args for Message Field
        /// i.e String.Format(Message,MessageArgs)
        /// </summary>
        public object[] MessageArgs { get; set; }

        public ReturnResult():this(true)
        { }

        public ReturnResult(bool showFrame)
        {
            StackTrace = new List<string>();
            if (showFrame)
            {
                StackFrameMethod = Environment.StackTrace;
            }
        }
    }


    public enum ResultType
    {
        OTHER,
        KEMAS,
        VRENT,
        UNIONPAY,
        VRENTFE,
        DATAACCESSProxy,
        DataSync,
        Pricing
    }
}
