using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace VRentDJ.Utility
{
    public class KemasLogin
    {
        public static ProxyUserSetting Login(string userName, string userPwd)
        {
            var auth = new KemasAuthencationAPI().authByLogin(userName, userPwd);
            var userApi = KemasAccessWrapper.CreateKemasUserAPI2Instance();
            var user = userApi.findUser2(auth.ID, auth.SessionID);

            ProxyUserSetting proxyUser = null;

            if (user != null && user.UserData != null)
            {
                proxyUser = new ProxyUserSetting();
                proxyUser.SessionID = auth.SessionID;
                proxyUser.ID = auth.ID;
                proxyUser.Name = user.UserData.Name;
                proxyUser.VName = user.UserData.VName;
                proxyUser.Result = auth.Result.ToInt();
                proxyUser.Mail = user.UserData.Mail;
                proxyUser.Blocked = user.UserData.Blocked;
                proxyUser.Enabled = user.UserData.Enabled;
            }
            return proxyUser;
        }
    }
}
