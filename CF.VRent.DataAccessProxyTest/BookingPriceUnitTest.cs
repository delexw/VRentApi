using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.DAL;

namespace CF.VRent.DataAccessProxyTest
{
    /// <summary>
    /// Summary description for BookingPriceUnitTest
    /// </summary>
    [TestClass]
    public class BookingPriceUnitTest
    {
        public BookingPriceUnitTest()
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
        public void SaveProxyBookingPriceTestMethod()
        {
            ProxyPrincingItem[] items = new ProxyPrincingItem[2]
            {
                new ProxyPrincingItem(),
                new ProxyPrincingItem()
            };

            ProxyBookingPrice pbp = new ProxyBookingPrice()
            {
                ProxyBookingID = 1,
                Total = 5.65m,
                Timestamp = DateTime.Now.Ticks.ToString(),
                TagID = "12345",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.NewGuid(),
                ModifiedOn = null,
                ModifiedBy = null,
                PrincingItems = items
            };

            PricingDAL pd = new PricingDAL();

            PricingDAL.SavePricingItems(pbp);
        }

        [TestMethod]
        public void SaveProxyBookingPricewithNullsTestMethod()
        {
            ProxyPrincingItem[] items = new ProxyPrincingItem[2]
            {
                new ProxyPrincingItem(),
                new ProxyPrincingItem()
            };

            ProxyBookingPrice pbp = new ProxyBookingPrice()
            {
                ProxyBookingID = 1,
                Total = 5.65m,
                Timestamp = null,
                TagID = null,
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.NewGuid(),
                ModifiedOn = null,
                ModifiedBy = null,
                PrincingItems = items
            };

            PricingDAL pd = new PricingDAL();

            PricingDAL.SavePricingItems(pbp);
        }

        [TestMethod]
        public void SaveProxyBookingPriceDetailTestMethod()
        {
            ProxyPrincingItem[] items = new ProxyPrincingItem[2]
            {
        //public int BookingPriceID { get; set; }
        //public string Description { get; set; }
        //public string Group { get; set; }
        //public string Category { get; set; }
        //public string Type { get; set; }
        //public decimal? UnitPrice { get; set; }
        //public decimal? Quantity { get; set; }
        //public decimal Total { get; set; }
        //public int State { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public Guid? CreatedBy { get; set; }
        //public DateTime? ModifiedOn { get; set; }
        //public Guid? ModifiedBy { get; set; }

                new ProxyPrincingItem()
                {
                    BookingPriceID = 1,
                    Description = "ABC",
                    Category = "rental",
                    Type = "Cancel",
                    Group = "Rental",
                    UnitPrice = 5.68m,
                    Quantity = 50m,
                    Total = 101,
                    State = 0,
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    ModifiedOn = null,
                    ModifiedBy = null
                },
                new ProxyPrincingItem()
                {
                    BookingPriceID = 1,
                    Description = "DEF",
                    Category = "rental",
                    Type = "Cancel",
                    UnitPrice = 7.68m,
                    Group = "Fine",
                    Quantity = 51m,
                    Total = 102,
                    State = 0,
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    ModifiedOn = null,
                    ModifiedBy = null
                }
            };

            ProxyBookingPrice pbp = new ProxyBookingPrice()
            {
                ProxyBookingID = 1,
                Total = 5.65m,
                Timestamp = DateTime.Now.Ticks.ToString(),
                TagID = "12345",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.NewGuid(),
                ModifiedOn = null,
                ModifiedBy = null,
                PrincingItems = items
            };

            PricingDAL pd = new PricingDAL();

            PricingDAL.SavePricingItems(pbp);
        }

        [TestMethod]
        public void SaveProxyBookingPriceDetailWithNullsTestMethod()
        {
            ProxyPrincingItem[] items = new ProxyPrincingItem[2]
            {
                //public int BookingPriceID { get; set; }
                //public string Description { get; set; }
                //public string Group { get; set; }
                //public string Category { get; set; }
                //public string Type { get; set; }
                //public decimal? UnitPrice { get; set; }
                //public decimal? Quantity { get; set; }
                //public decimal Total { get; set; }
                //public int State { get; set; }
                //public DateTime? CreatedOn { get; set; }
                //public Guid? CreatedBy { get; set; }
                //public DateTime? ModifiedOn { get; set; }
                //public Guid? ModifiedBy { get; set; }

                new ProxyPrincingItem()
                {
                    BookingPriceID = 1,
                    Description = null,
                    Category = null,
                    Type = null,
                    Group = "Rental",
                    UnitPrice = null,
                    Quantity = null,
                    Total = 101,
                    State = 0,
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    ModifiedOn = null,
                    ModifiedBy = null
                },
                new ProxyPrincingItem()
                {
                    BookingPriceID = 1,
                    Description = "DEF",
                    Category = "rental",
                    Type = "Cancel",
                    UnitPrice = 7.68m,
                    Group = "Fine",
                    Quantity = 51m,
                    Total = 102,
                    State = 0,
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.NewGuid(),
                    ModifiedOn = null,
                    ModifiedBy = null
                }
            };

            ProxyBookingPrice pbp = new ProxyBookingPrice()
            {
                ProxyBookingID = 18,
                Total = 5.65m,
                Timestamp = DateTime.Now.Ticks.ToString(),
                TagID = "12345",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.NewGuid(),
                ModifiedOn = null,
                ModifiedBy = null,
                PrincingItems = items
            };


            ProxyBookingPrice fromDB = PricingDAL.SavePricingItems(pbp);

            Assert.IsTrue(fromDB != null,"should create a new set of princing");
        }

        [TestMethod]
        public void LoadProxyBookingPriceTestMethod()
        {
            PricingDAL pd = new PricingDAL();

            ProxyBookingPrice pbp = PricingDAL.LoadPricingItems(502);

            Assert.IsTrue(pbp != null);
        }
    }
}
