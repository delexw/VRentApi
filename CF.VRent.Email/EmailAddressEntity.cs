using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    public class EmailAddressEntity
    {
        private string _value;
        public string Value { get { return _value; } }

        public EmailAddressEntity(string value)
        {
            _value = value;
        }
    }
}
