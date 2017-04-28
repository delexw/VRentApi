using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    public class EmailHostEntity
    {
        private string _address;
        public string Address { get { return _address; } }

        private string _from;
        public string From { get { return _from; } }

        private string _password;
        public string Password { get { return _password; } }

        private bool _async;
        public bool Async { get { return _async; } }

        public EmailHostEntity(string address, string from, string pwd, bool isAsync)
        {
            _address = address;
            _from = from;
            _password = pwd;
            _async = isAsync;
        }
    }
}
