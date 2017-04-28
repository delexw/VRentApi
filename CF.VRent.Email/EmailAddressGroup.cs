using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    public class EmailAddressGroup
    {
        private EmailAddressEntityCollection[] _addressGroups;
        public EmailAddressEntityCollection[] AddressGroups { get { return _addressGroups; } }

        public EmailAddressGroup(IEnumerable<EmailAddressEntityCollection> addressGroups)
        {
            _addressGroups = addressGroups.ToArray();
        }

        public EmailAddressEntityCollection this[string groupName]
        {
            get
            {
                return _addressGroups.FirstOrDefault(r => r.Name.Trim() == groupName.Trim());
            }
        }
    }
}
