using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Common;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.DataAccessProxyTest
{
    /// <summary>
    /// Summary description for IndirectFeeUnitTest
    /// </summary>
    [TestClass]
    public class IndirectFeeUnitTest
    {

        public const string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";
        public IndirectFeeUnitTest()
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
        public void RetrieveAllIndirectFeeTypeTestMethod()
        {
            IndirectFeeType[] fees = IndirectFeeDAL.RetrieveAllIndirectFeeTypes();

            Assert.IsTrue(fees != null, "should return something");
        }

        [TestMethod]
        public void CreateIndirectFeeTypesTestMethod()
        {
            List<IndirectFeeType> fees = new List<IndirectFeeType>();

            IndirectFeeType fee1 = new IndirectFeeType
            {
                Type = "Bug1",
                Group = "INDIRECT FEE",
                SourceType = IndirectFeeSourceType.WriteIn,
                Note = "Missing",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Parse(testUserID),
                ModifiedOn = null,
                ModifiedBy = null
            };
            fees.Add(fee1);

            IndirectFeeType fee2 = new IndirectFeeType
            {
                Type = "Bug2",
                Group = "INDIRECT FEE",
                SourceType = IndirectFeeSourceType.WriteIn,
                Note = "not missing",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Parse(testUserID),
                ModifiedOn = null,
                ModifiedBy = null
            };
            fees.Add(fee2);

            int affected = IndirectFeeDAL.SaveIndirectFeeType(fees.ToArray());

            Assert.IsTrue(fees != null && fees.Count > 0, "should return something");
        }

        [TestMethod]
        public void CreateOrderItemsWhenCreatingOrderTestMethod()
        {
            //public static int AddOrderAfterPayment(ProxyOrder order, string userId)

            int proxyBookingID = 8;
            string userID = "1C9D9C82-D074-45A4-863E-E7EEB2384C64";

            ProxyOrder paymentOrder = new ProxyOrder()
            { 
              ProxyBookingID = proxyBookingID,
              BookingUserID = userID,
              State = 0
            };

            int ret = PaymentDAL.AddOrderAfterPayment(paymentOrder,userID);


            Assert.IsTrue(ret  > 0, "should return something");
        }


        [TestMethod]
        public void RetrieveOrderItemsByGroupsTestMethod()
        {
            string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";
            ProxyUserSetting up = new ProxyUserSetting()
            {
                ID = testUserID
            };

            int proxyBookingID = 3;
            OrderItemConstant orderItemCon = new OrderItemConstant();
            string[] groups = new string[1] { "INDIRECTFEE" };


            ReturnResultRetrieveOrderItems items = IndirectFeeDAL.RetrieveBookingOrders(proxyBookingID, groups, up);

            Assert.IsTrue(items != null, "should return something");
        }


        [TestMethod]
        public void AddOrderItemsTestMethod()
        {



            OrderItemConstant orderItemCon = new OrderItemConstant();
            string[] groups = new string[1] { "RENTALFEE" };
            
            List<ProxyOrderItem> orderItems = new List<ProxyOrderItem>();
            ProxyOrderItem item1 = new ProxyOrderItem()
            {
                OrderID = 3,
                Category = orderItemCon.Groups[0],
                Type = "A",
                SalesQuantity = 5,
                AmountExclVAT = 100,
                TaxRate = 0,
                Tax = 0,
                AmountIncVAT = 200,
                Remark = "Some thing",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Parse(testUserID),
                ModifiedOn = null,
                ModifiedBy = null
            };
            ProxyOrderItem item2 = new ProxyOrderItem()
            {
                OrderID = 3,
                Category = orderItemCon.Groups[1],
                Type = "B",
                SalesQuantity = 5,
                AmountExclVAT = 100,
                TaxRate = 0,
                Tax = 0,
                AmountIncVAT = 200,
                Remark = "Some thing",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Parse(testUserID),
                ModifiedOn = null,
                ModifiedBy = null
            };
            ProxyOrderItem item3 = new ProxyOrderItem()
            {
                OrderID = 3,
                Category = orderItemCon.Groups[2],
                TypeID = 4,
                Type = "LateREturn",
                SalesQuantity = 5,
                AmountExclVAT = 100,
                TaxRate = 0,
                Tax = 0,
                AmountIncVAT = 200,
                Remark = "Some thing",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Parse(testUserID),
                ModifiedOn = null,
                ModifiedBy = null
            };

            orderItems.Add(item1);
            orderItems.Add(item2);
            orderItems.Add(item3);

            int affectedCnt = IndirectFeeDAL.AddOrderItems(orderItems.ToArray());

            Assert.IsTrue(affectedCnt > 0, "should return something");
        }


        [TestMethod]
        public void AddOrderItemsByBookingIDTestMethod()
        {
            string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";
            ProxyUserSetting up = new ProxyUserSetting()
            {
                ID = testUserID,
            };

            OrderItemConstant orderItemCon = new OrderItemConstant();
            string[] groups = new string[1] { "INDIRECTFEE" };

            List<ProxyOrderItem> orderItems = new List<ProxyOrderItem>();
            ProxyOrderItem item1 = new ProxyOrderItem()
            {
                OrderID = 3,
                Category = orderItemCon.Groups[2],
                Type = "Bug13",
                SalesQuantity = 5,
                AmountExclVAT = 100,
                TaxRate = 0,
                Tax = 0,
                AmountIncVAT = 200,
                Remark = "Some thing",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Parse(testUserID),
                ModifiedOn = null,
                ModifiedBy = null
            };
            ProxyOrderItem item2 = new ProxyOrderItem()
            {
                OrderID = 3,
                Category = orderItemCon.Groups[2],
                Type = "Bug13",
                SalesQuantity = 5,
                AmountExclVAT = 100,
                TaxRate = 0,
                Tax = 0,
                AmountIncVAT = 200,
                Remark = "Some thing",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Parse(testUserID),
                ModifiedOn = null,
                ModifiedBy = null
            };
            ProxyOrderItem item3 = new ProxyOrderItem()
            {
                OrderID = 3,
                Category = orderItemCon.Groups[2],
                //TypeID = 4,
                Type = "Bug12",
                SalesQuantity = 5,
                AmountExclVAT = 100,
                TaxRate = 0,
                Tax = 0,
                AmountIncVAT = 200,
                Remark = "Some thing",
                State = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Parse(testUserID),
                ModifiedOn = null,
                ModifiedBy = null
            };

            orderItems.Add(item1);
            orderItems.Add(item2);
            orderItems.Add(item3);

            ReturnResultAddIndirectFeeItems appendItems = IndirectFeeDAL.AddOrderItemsByBookingID(16, orderItems.ToArray(),up);

            Assert.IsTrue(appendItems != null, "should return something");
        }

    }


}
