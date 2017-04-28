using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.DAL;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using System.Globalization;
using CF.VRent.Common;
using CF.VRent.UPSDK;

namespace CF.VRent.DataAccessProxyTest
{
    /// <summary>
    /// Summary description for DUBDetailUnitTest
    /// </summary>
    [TestClass]
    public class DUBDetailUnitTest
    {
        public DUBDetailUnitTest()
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
        public void RetrieveDUBDetailsTestMethod()
        {
            ProxyUserSetting userInfo = null;

            DUBDetailSearchConditions dcp = new DUBDetailSearchConditions();

            dcp.DateBegin = DateTime.Now.AddMonths(-2);
            dcp.DateEnd = DateTime.Now.AddDays(-14);
            //dcp.UserID = Guid.Parse("BEB892AB-0B13-4805-9B0B-38AE5DE22C09");
            //dcp.KemasBookingNumber = 1667.ToString();
            //dcp.UPState = new Nullable<PaymentStatusEnum>();

            //dcp.UPState = UPProcessingState.Processing;

            dcp.ItemsPerPage = 10;
            dcp.PageNumber = 1;
            dcp.TotalPages = -1;

            DUBDetailSearchConditions output = DUBClosingDAL.RetrieveDUBDetailsByConditions(dcp, userInfo);
            Assert.IsTrue(output!= null);
        }
    }
}
