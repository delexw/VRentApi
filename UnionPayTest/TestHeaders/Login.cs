using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionPayTest.TestHeaders
{
    public class Login
    {
        public static WS_Auth_Response LoginKemas(string userName, string pwd)
        {
            KemasAuthencationAPI authApi = new KemasAuthencationAPI();
            return authApi.authByLogin(userName, pwd);
        }
    }
}
