using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.DAL;
using CF.VRent.Common;
using System.Globalization;

namespace CF.VRent.DataAccessProxyTest
{
    /// <summary>
    /// Summary description for DebitNoteExcelUnitTest
    /// </summary>
    [TestClass]
    public class DebitNoteExcelUnitTest
    {
        public DebitNoteExcelUnitTest()
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
        public void RetrieveMonthlySummaryTestMethod()
        {
            ProxyUserSetting userinfo = null;

            DebitNotesSearchConditions dnsc = new DebitNotesSearchConditions();
            dnsc.ItemsPerPage = 10;
            dnsc.PageNumber = 1;
            dnsc.TotalPages = -1;

            DebitNotesSearchConditions output = DebitNoteDAL.RetrieveDebitNotesWithPaging(dnsc,userinfo);

            Assert.IsTrue(output.Notes != null);

            if (output.Notes != null)
            {
                PricingItemMonthlysummary[] summary = DebitNoteExcelDAL.RetrievePricingCatalog( output.Notes, userinfo);

                Assert.IsTrue(summary != null && summary.Length > 0);
            }
        }
    }
}
