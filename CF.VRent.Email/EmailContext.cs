using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    public class EmailContext
    {
        public static EmailManager CreateManager(EmailType type)
        {
            return new EmailManager("VRentEmail", type);
        }
    }
}
