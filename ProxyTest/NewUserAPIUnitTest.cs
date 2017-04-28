using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.BLL;
using CF.VRent.Entities;

namespace ProxyTest
{
    /// <summary>
    /// Summary description for NewUserAPIUnitTest
    /// </summary>
    [TestClass]
    public class NewUserAPIUnitTest
    {
        public NewUserAPIUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void LoginNewTestMethod()
        {
            UserSettingBLL usb = new UserSettingBLL();
            UserExtension ue = new UserExtension();
            ue.Mail = "rent42@rent42.com";
            ue.Password = "123456";

            UserExtension backend = usb.Login(ue);
            Assert.IsTrue(backend != null, "should contain all user info");
        }
    }
}
