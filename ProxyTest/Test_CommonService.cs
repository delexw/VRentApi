using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using CF.VRent.Entities;
using CF.VRent.Common;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using CF.VRent.BLL;
using CF.VRent.Common.UserContracts;

namespace ProxyTest
{
    /// <summary>
    /// Summary description for CommonService
    /// </summary>
    [TestClass]
    public class Test_CommonService
    {
        private WebClient client;
        public Test_CommonService()
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //client = new WebClient();

            //client.Headers[HttpRequestHeader.ContentType] = "application/json";
            //string data = ProxyTest.DatReader.Read("LoginData_Corret.txt");
            //string loginRes = client.UploadString(UnitTestConfiguration.GetHostName() + "/LoginService/Login", "POST", data);
        }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() { }
        
        #endregion

        [TestMethod]
        public void TestGetQRCode()
        {
            //
            // TODO: Add test logic here
            //
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/User/0de53307-00b3-47d9-9744-4916637246d4/Reservations/24856742-4674-4382-a2d3-6318834d6258/QRCode");
            string expected = string.Empty;
            //check return is not empty
            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void TestAllRights()
        {
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/AllRights");
            string expected = string.Empty;
            //check return is not empty
            Assert.AreNotEqual(expected, actual);
        }

        //
        [TestMethod]
        public void TestGetAvaliableCarCounts()
        {
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/AvaliableCars?BookingId=&DateBegin=2013-08-23 18:4&DateEnd=2013-08-23 19:45&VehicleCategory=0&TypeOfJourney=1");
            string expected = string.Empty;
            //check return is not empty
            Assert.AreNotEqual(expected, actual);
        }

        /// <summary>
        /// Test method for get all drivers
        /// </summary>
        [TestMethod]
        public void TestDrivers()
        {
            //FindAllDrivers
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/Drivers");
            string expected = string.Empty;
            //check return is not empty
            Assert.AreNotEqual(expected, actual);

            List<Driver> list = SerializedHelper.JsonDeserialize<List<Driver>>(actual);

            Assert.IsNotNull(list);
        }

        /// <summary>
        /// Test method for get all drivers
        /// </summary>
        [TestMethod]
        public void TestConfigurations()
        {
            //FindAllDrivers
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/Configurations");
            string expected = string.Empty;
            //check return is not empty
            Assert.AreNotEqual(expected, actual);

            List<SystemConfig> list = SerializedHelper.JsonDeserialize<List<SystemConfig>>(actual);

            Assert.IsNotNull(list);
        }

        /// <summary>
        /// Get all locations
        /// </summary>
        [TestMethod]
        public void TestAllLocations()
        {
            //FindAllDrivers
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/Locations");
            string expected = string.Empty;
            //check return is not empty
            Assert.AreNotEqual(expected, actual);

            List<Location> list = SerializedHelper.JsonDeserialize<List<Location>>(actual);

            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TestGetConfigurations()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            SystemConfigurationBLL scb = new SystemConfigurationBLL(setting);
            SystemConfig config = scb.GetSystemConfiguration();

            Assert.IsTrue(config != null);
        }
    }
}
