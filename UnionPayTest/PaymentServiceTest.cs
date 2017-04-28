using CF.VRent.DataAccessProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace UnionPayTest
{
    
    
    /// <summary>
    ///This is a test class for PaymentServiceTest and is intended
    ///to contain all PaymentServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PaymentServiceTest
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
        ///A test for DeductionViaUP
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void DeductionViaUPTest()
        {
            PaymentService target = new PaymentService(); // TODO: Initialize to an appropriate value
            string price = "100"; // TODO: Initialize to an appropriate value
            int paymentId = 16; // TODO: Initialize to an appropriate value
            string cardId = "9dd95e1d-7825-4112-b0f5-1f25dfd1158f"; // TODO: Initialize to an appropriate value
            string cardUserId = "792d78f9-5d4f-439f-b5cf-8f0b58e2a87c"; // TODO: Initialize to an appropriate value
            string reservedMsg = "text"; // TODO: Initialize to an appropriate value
            string uid = "792d78f9-5d4f-439f-b5cf-8f0b58e2a87c"; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            //actual = target.DeductionViaUP(price, paymentId, cardId, cardUserId, reservedMsg, uid);
            //Assert.AreEqual(expected, actual);
        }
    }
}
