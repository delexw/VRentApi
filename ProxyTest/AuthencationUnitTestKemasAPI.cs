using CF.VRent.Common;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProxyTest
{
    [TestClass]
    public class AuthencationUnitTestKemasAPI
    {

        public static WS_Auth_Response SignOn() 
        {
            string userName = "cf";
            string password = "123456";
            IKemasAuthencation kemas = new KemasAuthencationAPI();
            return kemas.authByLogin(userName, ":md5:" + Encrypt.EncryptMD5(password));
            
        }

        [TestMethod]
        public void authByLoginSuccessTestMethod()
        {
            //public WS_Auth_Response authByLogin(string user, string pass)

            string userName = "cf";
            string password = "123456";
            IKemasAuthencation kemas = new KemasAuthencationAPI();
            WS_Auth_Response res = kemas.authByLogin(userName, ":md5:" + Encrypt.EncryptMD5(password));

            Assert.IsTrue(res != null && !res.ID.Equals(Guid.Empty), "should sign on successful.");
        }

        [TestMethod]
        public void authByLoginFailTestMethod()
        {
            //public WS_Auth_Response authByLogin(string user, string pass)
            try
            {
                string userName = "cf";
                string password = "123";
                IKemasAuthencation kemas = new KemasAuthencationAPI();
                WS_Auth_Response res = kemas.authByLogin(userName, ":md5:" + Encrypt.EncryptMD5(password));

                Assert.IsTrue(res != null && string.IsNullOrEmpty( res.ID), "should sign on fail.");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        [TestMethod]
        public void logOutTestMethod()
        {
            //public WS_Auth_Response authByLogin(string user, string pass)
            string userName = "cf";
            string password = "123456";
            IKemasAuthencation kemas = new KemasAuthencationAPI();
            WS_Auth_Response res = kemas.authByLogin(userName, ":md5:" + Encrypt.EncryptMD5(password));

            try
            {
                Error logoutError = kemas.logout(res.SessionID);
                Assert.IsTrue(logoutError != null,"logout successfully");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

    }
}
