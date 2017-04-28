using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionPayTest.TestHeaders
{
    public class GetUserSettings
    {
        public static ProxyUserSetting GetUser(string userName, string userPwd)
        {
            KemasAuthencationAPI api = new KemasAuthencationAPI();
            var a = api.authByLogin(userName, userPwd);

            KemasUserAPI user = new KemasUserAPI();
            var userObj = user.findUser2(a.ID, a.SessionID);

            var UserSetting = new ProxyUserSetting()
            {
                ID = a.ID,
                VName = a.VName,
                Name = a.Name,
                Mail = userObj.UserData.Mail,
                SessionID = a.SessionID
            };

            return UserSetting;
        }
    }
}
