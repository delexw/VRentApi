using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.Entities.KemasWrapper;
using UnionPayTest.TestHeaders;

namespace UnionPayTest.UserMgmtTest
{
    [TestClass]
    public class KemasUserApi:TestHeader
    {
        [TestMethod]
        public void FinUserToCheckStatus()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);
            var user2 = Login.LoginKemas("adam", "123456");
            KemasUserAPI kapi = new KemasUserAPI();
            TestContext.WriteLine(DateTime.Now.ToString());

            var user2Ext = kapi.findUser2(user2.ID, user.SessionID);

            TestContext.WriteLine(DateTime.Now.ToString());
            Assert.IsNotNull(user2Ext.UserData);
            base.OutputMessage(user2Ext.Error);
            base.OutputMessage(user2Ext.UserData);
            base.OutputMessage(user2Ext.UserData.License);
            base.OutputMessage(user2Ext.UserData.Clients);
            base.OutputMessage(user2Ext.UserData.Roles);
        }

        [TestMethod]
        public void LoginKemas()
        {
            var user2 = Login.LoginKemas("adam", "123456");
            Assert.IsNotNull(user2);
            base.OutputMessage(user2);
        }

        [TestMethod]
        public void UpdateUserStatusKemas()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);
            var user2 = Login.LoginKemas("adam", "123456");
            KemasUserAPI kapi = new KemasUserAPI();
            var newUser2 = kapi.updateUser2(new CF.VRent.Entities.KEMASWSIF_USERRef.updateUser2Request()
            {
                SessionID = user.SessionID,
                Language = "english",
                UserData = new CF.VRent.Entities.KEMASWSIF_USERRef.updateUserData() {
                    ID = user2.ID,
                    Status = "0010000",
                    Enabled = 1,
                    Clients = new string[] {
                        "f5b84c82-8ea6-4a67-8088-2f91907379b6",
                        "e1c286c4-ae86-4c7d-810f-1b6357892f9f"
                    }
                }
            });
            Assert.IsNotNull(newUser2.UserData);
            base.OutputMessage(newUser2.UserData);
        }

        [TestMethod]
        public void GetUserByPage()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);
            KemasUserAPI kapi = new KemasUserAPI();
            TestContext.WriteLine(DateTime.Now.ToString());
            var users = kapi.getUsers2(new CF.VRent.Entities.KEMASWSIF_USERRef.getUsers2Request()
            {
                SessionID = user.SessionID,
                ItemsPerPageSpecified = true,
                PageSpecified = true,
                ItemsPerPage = 10,
                Page = 0
            });
            TestContext.WriteLine(DateTime.Now.ToString());
            base.OutputMessage(users);
            base.OutputMessage(users.Error);
            base.TestContext.WriteLine(users.Users.Length.ToString());
            base.OutputMessage(users.Users);
            
            Assert.AreEqual(users.rows, 1);
            
        }

        [TestMethod]
        public void UpdateUser1()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);
            var user2 = Login.LoginKemas("adam", "123456");
            KemasUserAPI kapi = new KemasUserAPI();
            var newUser2 = kapi.updateUser(user2.ID,new CF.VRent.Entities.KEMASWSIF_USERRef.UserData(){
                PrivateAddress ="123"
            });
            base.OutputMessage(newUser2);
            Assert.AreEqual(newUser2,"0");
        }

        [TestMethod]
        public void GetRoles()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);
            KemasUserAPI kapi = new KemasUserAPI();
            var kRoles = kapi.getRoles(user.SessionID);

            Assert.IsNotNull(kRoles.Roles);

            base.OutputMessage(kRoles.Roles);

        }
    }
}
