using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Log.ConcreteLog
{
    public class TimerLogger : VRentLog
    {
        public TimerLogger() : base("VwfscnLog_JobTimer") { }
    }
}
