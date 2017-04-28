using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.BLL;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.AccountingService;
using CF.VRent.Common;
using System.Globalization;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.UPSDK;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;

namespace ProxyTest
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
        public void DUBDetailTestMethod()
        {
            WS_Auth_Response auth = KemasAdmin.SignOn();

            ProxyUserSetting userInfo = new ProxyUserSetting()
            {
                ID = auth.ID,
                SessionID = auth.SessionID
            };

            IDUB dub = new DUBBLL(userInfo);

            DUBDetailSearchConditions ddsc = new DUBDetailSearchConditions();
            ddsc.DateBegin = DateTime.Now.AddMonths(-2);
            ddsc.DateEnd = DateTime.Now.AddDays(-26);
            //ddsc.UserID = Guid.Parse("BEB892AB-0B13-4805-9B0B-38AE5DE22C09");
            ddsc.UserName = "e";
            ddsc.KemasBookingNumber = "4";
            ddsc.UPState = UPProcessingState.Processing;

            ddsc.ItemsPerPage = 10;
            ddsc.PageNumber = 1;
            ddsc.TotalPages = -1;

            DUBDetailSearchConditions items = dub.RetrieveDUBDetails(ddsc);

            string jsonRes = SerializedHelper.JsonSerialize<DUBDetailSearchConditions>(items);

            Assert.IsTrue(items != null);
        }

    }
}
