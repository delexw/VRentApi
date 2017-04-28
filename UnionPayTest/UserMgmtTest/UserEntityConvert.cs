using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnionPayTest.TestHeaders;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using System.Collections.Generic;

namespace UnionPayTest.UserMgmtTest
{
    [TestClass]
    public class UserEntityConvert: TestHeader
    {
        [TestMethod]
        public void ConvertFromKemasUserToVRentUser_UserAdam()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);
            var user2 = Login.LoginKemas("adam", "123456");
            KemasUserAPI kapi = new KemasUserAPI();
            var user2Ext = kapi.findUser2(user2.ID, user.SessionID);

            UserFactory uf = new UserFactory();
            var newUser = uf.CreateEntity(user2Ext.UserData);

            base.OutputMessage(newUser);
            base.OutputMessage(newUser.License);

            Assert.IsNotNull(newUser);
        }

        [TestMethod]
        public void ConvertFromKEmasUsersToVRentUsers_UserAdmaAndCF()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);
            var user2 = Login.LoginKemas("adam", "123456");
            KemasUserAPI kapi = new KemasUserAPI();
            var user2Ext = kapi.findUser2(user2.ID, user.SessionID);
            var user2Ext2 = kapi.findUser2(user.ID, user.SessionID);

            UserFactory uf = new UserFactory();

            List<UserData2> lu = new List<UserData2>();
            lu.Add(user2Ext.UserData);
            lu.Add(user2Ext2.UserData);

            var newUsers = uf.CreateEntity(lu);

            base.OutputMessage(newUsers);

            Assert.IsNotNull(newUsers);
        }
    }
}
