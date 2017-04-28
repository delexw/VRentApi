using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Log.ConcreteLog
{
    public class GeneralLedgerLogger : VRentLog
    {
        public GeneralLedgerLogger() : base("VwfscnLog_GL") { }
    }
}
