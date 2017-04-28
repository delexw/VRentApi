using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Log.ConcreteLog
{
    public class PaymentLogger: VRentLog
    {
        public PaymentLogger()
            : base("VwfscnLog_Pay")
        { }
    }
}
