using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnionPayTest.TestHeaders;
using CF.VRent.Entities.KemasWrapper;

namespace UnionPayTest.UserMgmtTest
{
    [TestClass]
    public class KemasCompanyApi:TestHeader
    {
        [TestMethod]
        public void GetCompaies()
        {
            var user = Login.LoginKemas("vrent.mgr@abc.com", TestHeader.LoginParameters.UserPwd);
            KemasConfigsAPIProxy api = new KemasConfigsAPIProxy();
            var client = api.getClients(user.SessionID);

            base.OutputMessage(client);
            base.OutputMessage(client.Error);
            base.OutputMessage(client.Clients);

            Assert.IsNotNull(client.Clients);
        }
        [TestMethod]
        public void GetRole()
        {
            var user = Login.LoginKemas("service.center@abc.com", TestHeader.LoginParameters.UserPwd);
            KemasUserAPI api = new KemasUserAPI();
            var client = api.getRoles(user.SessionID);

            base.OutputMessage(client);
            base.OutputMessage(client.Error);
            base.OutputMessage(client.Roles);

            Assert.IsNotNull(client.Roles);
        }
        [TestMethod]
        public void GetRoleByFindUser()
        {
            var user = Login.LoginKemas("adam", TestHeader.LoginParameters.UserPwd);
            KemasUserAPIProxy api = new KemasUserAPIProxy();
            var client = api.findUser2(user.ID, user.SessionID);

            base.OutputMessage(client);
            base.OutputMessage(client.Error);
            base.OutputMessage(client.UserData);

            Assert.IsNotNull(client.UserData);
        }
    }
}
