using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    public interface IEmailAddressValidation
    {
        string RegexPattern { get; }
        bool Validate(string address);
    }
}
