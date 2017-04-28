using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using VWFSCN.IT.Log;
using VWFSCN.IT.Log.Configuration;

namespace CF.VRent.Log.ConcreteLog
{
    public class EmailLogger : VRentLog
    {
        public EmailLogger()
            : base("VwfscnLog_Email")
        {
        }
    }
}
