using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using CF.VRent.Common;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;

namespace ProxyTest
{
    [TestClass]
    public class Test_UserService
    {
        private WebClient client;
        //You can use the following additional attributes as you write your tests:
        
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }
        
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            client = new WebClient();

            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string data = ProxyTest.DatReader.Read("LoginData_Corret.txt");
            string loginRes = client.UploadString("http://localhost:9999" + "/LoginService/Login", "POST", data);
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        
        /// <summary>
        /// Test method for Ping method
        /// </summary>
        [TestMethod]
        public void TestLoginMethod_Ping()
        {
            string actual = string.Empty;
       
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/LoginService/Logout");

            string expected = "\"200\"";// TODO: Initialize to an appropriate value

            Assert.AreNotSame(expected, actual);
        }

        /// <summary>
        /// Test method for changing the password
        /// </summary>
        [TestMethod]
        public void TestChangePwdMethod()
        {
            string actual = string.Empty;
            string data = ProxyTest.DatReader.Read("ChangePwdData_Corret.txt");
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.UploadString(UnitTestConfiguration.GetHostName() + "/DataService/User/0de53307-00b3-47d9-9744-4916637246d4/Password", "PUT", data);
            string expected = string.Empty; // TODO: Initialize to an appropriate value

            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        /// Test method for updating the user information
        /// </summary>
        //[TestMethod]
        //public void TestUpdateUserInformation()
        //{
        //    //Update to a new department
        //    string actual = string.Empty;
        //    string data = ProxyTest.DatReader.Read("ChangeUserInforUpdate.txt");
        //    client.Headers[HttpRequestHeader.ContentType] = "application/json";
        //    actual = client.UploadString(UnitTestConfiguration.GetHostName() + "/DataService/User/0de53307-00b3-47d9-9744-4916637246d4", "PUT", data);

        //    ProxyUser getUser = SerializedHelper.JsonDeserialize<ProxyUser>(actual);
        //    //check if the value is changed
        //    Assert.AreEqual(getUser.Department, "Engines");

        //    //Change back to the orignal value
        //    data = ProxyTest.DatReader.Read("ChangeUserInforOrg.txt");
        //    client.Headers[HttpRequestHeader.ContentType] = "application/json";
        //    actual = client.UploadString(UnitTestConfiguration.GetHostName() + "/DataService/User/0de53307-00b3-47d9-9744-4916637246d4", "PUT", data);

        //    //check if the value is changed to the orignal value
        //    getUser = SerializedHelper.JsonDeserialize<ProxyUser>(actual);
        //    Assert.AreEqual(getUser.Department, "Engine");
        //}
    }
}
