using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnionPayTest.TestHeaders;
using CF.VRent.BLL.BLLFactory.Payment;
using CF.VRent.Entities.EntityFactory;

namespace UnionPayTest.UserMgmtTest
{
    [TestClass]
    public class UserBlockerTest:TestHeader
    {
        [TestMethod]
        public void BlockUser()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);
            var user2 = Login.LoginKemas("adam", "123456");

            UserBlocker b = new UserBlocker(new CF.VRent.Entities.UserExtension()
            {
                SessionID = user.SessionID
            }, new CF.VRent.Entities.UserExtension() { ID = user2.ID }
                );

            var r = b.DeactiveDUB();

            Assert.IsTrue(r);
        }
    }
}
