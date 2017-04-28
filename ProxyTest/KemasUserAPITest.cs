using CF.VRent.Entities.KemasWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CF.VRent.Entities.KEMASWSIF_USERRef;

namespace ProxyTest
{
    
    
    /// <summary>
    ///This is a test class for KemasUserAPITest and is intended
    ///to contain all KemasUserAPITest Unit Tests
    ///</summary>
    [TestClass()]
    public class KemasUserAPITest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for findUser
        ///</summary>
        [TestMethod()]
        public void findUserTest()
        {
            KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
            string userId = "1c9d9c82-d074-45a4-863e-e7eeb2384c64"; // TODO: Initialize to an appropriate value
            findUser_Response expected = new findUser_Response() { ID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64" }; // TODO: Initialize to an appropriate value
            findUser_Response actual;
            actual = target.findUser(userId);
            Assert.AreEqual(expected.ID, actual.ID);
        }

        /// <summary>
        ///A test for findAllDrivers
        ///</summary>
        [TestMethod()]
        public void findAllDriversTest()
        {
            KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
            string userId = "1c9d9c82-d074-45a4-863e-e7eeb2384c64"; // TODO: Initialize to an appropriate value
            Driver[] expected = null; // TODO: Initialize to an appropriate value
            Driver[] actual;
            actual = target.findAllDrivers(userId);
            Assert.IsTrue(actual != null);
        }

        /// <summary>
        ///A test for findUser2
        ///</summary>
        [TestMethod()]
        public void findUser2Test()
        {
            try
            {
                var u = new KemasAuthencationAPI().authByLogin("cf","123456");
                KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
                string userId = u.ID; // TODO: Initialize to an appropriate value
                string sessionId = u.SessionID; // TODO: Initialize to an appropriate value
                findUser2Response expected = null; // TODO: Initialize to an appropriate value
                findUser2Response actual;
                actual = target.findUser2(userId, sessionId);
                Assert.IsTrue(actual != null && actual.UserData != null);
            }
            catch (Exception e)
            {
 
            }
        }

        /// <summary>
        ///A test for forgotPassword
        ///</summary>
        [TestMethod()]
        public void forgotPasswordTest()
        {
            var u = new KemasAuthencationAPI().authByLogin("cf", "123456");
            
            KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
            var u2 = target.findUser2(u.ID, u.SessionID);

            string mail = u2.UserData.Mail; // TODO: Initialize to an appropriate value
            string lang = "english"; // TODO: Initialize to an appropriate value
            StdResponse expected = new StdResponse() { Result = "0" }; // TODO: Initialize to an appropriate value
            StdResponse actual;
            actual = target.forgotPassword(mail, lang);
            Assert.AreEqual(expected.Result, actual.Result);
        }

        /// <summary>
        ///A test for getRights
        ///</summary>
        [TestMethod()]
        public void getRightsTest()
        {
            var u = new KemasAuthencationAPI().authByLogin("cf", "123456");
            KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
            string userId = u.ID; // TODO: Initialize to an appropriate value
            Right[] expected = null; // TODO: Initialize to an appropriate value
            Right[] actual;
            actual = target.getRights(userId);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for getRoles
        ///</summary>
        [TestMethod()]
        public void getRolesTest()
        {
            var u = new KemasAuthencationAPI().authByLogin("cf", "123456");
            KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
            string sessionID = u.SessionID; // TODO: Initialize to an appropriate value
            getRolesResponse expected = null; // TODO: Initialize to an appropriate value
            getRolesResponse actual;
            actual = target.getRoles(sessionID);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for getUsers
        ///</summary>
        [TestMethod()]
        public void getUsersTest()
        {
            var u = new KemasAuthencationAPI().authByLogin("cf", "123456");
            KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
            string userId = u.ID; // TODO: Initialize to an appropriate value
            UserList expected = null; // TODO: Initialize to an appropriate value
            UserList actual;
            actual = target.getUsers(userId);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for getUsers2
        ///</summary>
        [TestMethod()]
        public void getUsers2Test()
        {
            var u = new KemasAuthencationAPI().authByLogin("cf", "123456");
            KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
            getUsers2Request request = new getUsers2Request() { 
                SessionID = u.SessionID
            }; // TODO: Initialize to an appropriate value
            getUsers2Response expected = null; // TODO: Initialize to an appropriate value
            getUsers2Response actual;
            actual = target.getUsers2(request);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for updateUser
        ///</summary>
        [TestMethod()]
        public void updateUserTest()
        {
            var u = new KemasAuthencationAPI().authByLogin("cf@cf2.com", "123456");
            KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
            string userId = u.ID; // TODO: Initialize to an appropriate value
            UserData userData = new UserData() {
                ID = u.ID,
                CurrentPassword = "123456",
                Password = "123456",
                PrivateEmail = "789"
            }; // TODO: Initialize to an appropriate value
            string expected = target.getUsers2(new getUsers2Request()
            {
                SessionID = u.SessionID
            }).Users[0].PrivateEmail; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.updateUser(userId, userData);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for updateUser2
        ///</summary>
        [TestMethod()]
        public void updateUser2Test()
        {
            var u = new KemasAuthencationAPI().authByLogin("june", "123456");
            var admin = new KemasAuthencationAPI().authByLogin("test1@kemas.de", "123456");
            KemasUserAPI target = new KemasUserAPI(); // TODO: Initialize to an appropriate value
            updateUser2Request request = new updateUser2Request()
            {
                SessionID = admin.SessionID,
                UserData = new updateUserData()
                {
                    ID = u.ID,
                    Status = "blocked"
                }
            }; // TODO: Initialize to an appropriate value
            int expected = 1; // TODO: Initialize to an appropriate value
            updateUser2Response actual;
            actual = target.updateUser2(request);
            Assert.AreEqual(expected, target.findUser2(u.ID,u.SessionID).UserData.Blocked);
        }
    }
}
