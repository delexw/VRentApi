using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    public class EmailAddressEntityCollection : IEnumerable<EmailAddressEntity>, ICollection<EmailAddressEntity>
    {
        private EmailAddressEntity[] _addresses;

        private string _name;
        public string Name { get { return _name; } }

        public EmailAddressEntityCollection()
            : this(new List<EmailAddressEntity>(), "")
        { }

        public EmailAddressEntityCollection(IEnumerable<EmailAddressEntity> addresses, string name)
        {
            _name = name;
            _addresses = addresses.ToArray();
        }

        /// <summary>
        /// Get all email addressed configured in web.config
        /// </summary>
        /// <returns></returns>
        public string[] GetAllAddresses()
        {
            List<string> addr = new List<string>();
            foreach (EmailAddressEntity ae in _addresses)
            {
                addr.Add(ae.Value);
            }
            return addr.ToArray();
        }

        public IEnumerator<EmailAddressEntity> GetEnumerator()
        {
            return _addresses.ToList().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _addresses.GetEnumerator();
        }

        public void Add(EmailAddressEntity item)
        {
            var list = _addresses.ToList();
            list.Add(item);
            _addresses = list.ToArray();
        }

        public void Clear()
        {
            _addresses = new EmailAddressEntity[] { };
        }

        public bool Contains(EmailAddressEntity item)
        {
            return _addresses.Contains(item);
        }

        public void CopyTo(EmailAddressEntity[] array, int arrayIndex)
        {
            _addresses.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _addresses.Length; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(EmailAddressEntity item)
        {
            var list = _addresses.ToList();
            var ret = list.Remove(item);
            _addresses = list.ToArray();
            return ret;
        }
    }
}
