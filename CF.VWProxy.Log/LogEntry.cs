using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Log
{
    public class LogEntry
    {
        /// <summary>
        /// 
        /// </summary>
        public string OperationDirection { get; set; }

        public string RequestClient { get; set; }

        public string OperationName { get; set; }

        public string OperationUser { get; set; }

        public string Format { get; set; }

        public string IsUrlMatched { get; set; }

        public string Uri { get; set; }

        public string RequestHttpMethod { get; set; }

        public string ReturnValue { get; set; }
    }
}
