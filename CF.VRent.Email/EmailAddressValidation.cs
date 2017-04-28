using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CF.VRent.Email
{
    /// <summary>
    /// Validate email address
    /// </summary>
    public class EmailAddressValidation : IEmailAddressValidation
    {
        private string _regexPattern;
        public string RegexPattern
        {
            get
            {
                return _regexPattern;
            }
        }

        public EmailAddressValidation()
            : this(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")
        { }

        public EmailAddressValidation(string regexPattern)
        {
            _regexPattern = regexPattern;
        }

        public bool Validate(string address)
        {
            if (String.IsNullOrWhiteSpace(address))
            {
                return false;
            }
            return Regex.IsMatch(address, this._regexPattern);
        }
    }
}
