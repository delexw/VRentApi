using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionPayTest.TestHeaders.AggregateEntity
{
    public class UserLogin : IAggregateRoot
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }

        public UserLogin()
        {
            UserName = "service.center@abc.com";
            UserPwd = "123456";
        }
    }
}
