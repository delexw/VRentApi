using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Common;
using CF.VRent.BLL;
using CF.VRent.Common.UserContracts;

namespace ProxyTest
{
    /// <summary>
    /// Summary description for RequestFapiaoTestUnitTest
    /// </summary>
    [TestClass]
    public class RequestFapiaoTestUnitTest
    {
        public RequestFapiaoTestUnitTest()
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
        public void RetrieveFapiaoRequestUnitTest()
        {
            WS_Auth_Response res = AuthencationUnitTestKemasAPI.SignOn();
            UserProfile up = new UserProfile()
            {
                UserID = Guid.Parse( res.ID)
            };

            int proxybookingID = 19;

            FapiaoSource source1 = FapiaoSource.RentalFee;
            FapiaoSource source2 = FapiaoSource.IndirectFee;

            List<int> sources = new List<int>();
            sources.Add((int)source1);
            sources.Add((int)source2);

            string RawSource = string.Join(",", sources);

            ProxyUserSetting pus = new ProxyUserSetting()
            {
                ID = res.ID,
                SessionID = res.SessionID
            };

            RequestFapiaoBLL rfb = new RequestFapiaoBLL(pus);

            ProxyFapiaoRequest[] fapiaoRequests = rfb.RetrieveFapiaoRequestBySource(proxybookingID, sources.ToArray(), "english");

            string jsonRes = SerializedHelper.JsonSerialize<ProxyFapiaoRequest[]>(fapiaoRequests);

            Assert.IsTrue(fapiaoRequests != null, "should create a fapiao data record!");
        }

        [TestMethod]
        public void UpdateFapiaoRequestUnitTest()
        {
            WS_Auth_Response res = AuthencationUnitTestKemasAPI.SignOn();
            UserProfile up = new UserProfile()
            {
                UserID = Guid.Parse(res.ID)
            };

            int proxybookingID = 40;
            FapiaoSource source = FapiaoSource.RentalFee;

            ProxyFapiaoRequest request = new ProxyFapiaoRequest()
            {
                ProxyBookingID = proxybookingID,
                FapiaoPreferenceID = Guid.Parse("03e59ac3-ae93-48d1-9a34-0b12c6a09ca6"),
                FapiaoSource = (int)source,
                ModifiedOn = DateTime.Now,
                ModifiedBy = Guid.NewGuid()
            };

            ProxyUserSetting pus = new ProxyUserSetting()
            {
                ID = res.ID,
                SessionID = res.SessionID
            };

            RequestFapiaoBLL rfb = new RequestFapiaoBLL(pus);

            ProxyFapiaoRequest fapiaoRequest = rfb.UpdateFapiaoRequest(request, "english");

            string jsonRes = SerializedHelper.JsonSerialize<ProxyFapiaoRequest>(fapiaoRequest);

            Assert.IsTrue(fapiaoRequest != null, "should create a fapiao data record!");
        }

    }
}
