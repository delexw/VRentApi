using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Common;
using CF.VRent.BLL;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Common.UserContracts;

namespace ProxyTest
{
    /// <summary>
    /// Summary description for IndirectFeeUnitTest
    /// </summary>
    [TestClass]
    public class IndirectFeeUnitTest
    {

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
        #endregion

        [TestMethod]
        public void RetrieveIndirectFeeTypesTestMethod()
        {
            IDataService ids = new DataAccessProxyManager();

            IndirectFeeType[] types = ids.RetrieveIndirectFeeTypes();

            string jsonSer = SerializedHelper.JsonSerialize<IndirectFeeType[]>(types);

            Assert.IsTrue(types.Length > 0);
        }



        public static Guid cfUserID = Guid.Parse("1c9d9c82-d074-45a4-863e-e7eeb2384c64");

        [TestMethod]
        public void AddIndirectFeeTypesTestMethod()
        {
            ProxyUserSetting pus = new ProxyUserSetting();
            pus.ID = cfUserID.ToString();

            IndirectFeeBLL ifb = new IndirectFeeBLL(pus);

            List<IndirectFeeType> fees = new List<IndirectFeeType>();

            IndirectFeeType fee1 = new IndirectFeeType
            {
                Type = "Later",
                Group = "INDIRECTFEE",
                Note = "Missing"
            };
            fees.Add(fee1);

            IndirectFeeType fee2 = new IndirectFeeType
            {
                Type = "Broken",
                Group = "INDIRECTFEE",
                Note = "not missing",
            };
            fees.Add(fee2);

            int affected = ifb.AddIndirectFeeTypes(fees.ToArray());

            Assert.IsTrue(affected > 0);
        }

        //[TestMethod]
        //public void AddOrderItemsTestMethod()
        //{
        //    OrderItemConstant orderItemCon = new OrderItemConstant();
        //    string[] groups = new string[1] { "RENTALFEE" };

        //    List<ProxyOrderItem> orderItems = new List<ProxyOrderItem>();
        //    ProxyOrderItem item1 = new ProxyOrderItem()
        //    {
        //        OrderID = 10,
        //        Category = orderItemCon.Groups[0],
        //        Type = "A",
        //        UnitPrice = 40,
        //        SalesQuantity = 5,
        //        AmountIncVAT = 200,
        //        Remark = "Some thing"
        //    };
        //    ProxyOrderItem item2 = new ProxyOrderItem()
        //    {
        //        OrderID = 10,
        //        Category = orderItemCon.Groups[1],
        //        Type = "B",
        //        UnitPrice = 40,
        //        SalesQuantity = 5,
        //        AmountIncVAT = 200,
        //        Remark = "Some thing"
        //    };
        //    ProxyOrderItem item3 = new ProxyOrderItem()
        //    {
        //        OrderID = 10,
        //        Category = orderItemCon.Groups[2],
        //        TypeID = 4,
        //        Type = "LateREturn",
        //        UnitPrice = 40,
        //        SalesQuantity = 5,
        //        AmountIncVAT = 200,
        //        Remark = "Some thing"
        //    };

        //    orderItems.Add(item1);
        //    orderItems.Add(item2);
        //    orderItems.Add(item3);

        //    ProxyUserSetting pus = new ProxyUserSetting();
        //    pus.ID = cfUserID.ToString();

        //    IndirectFeeBLL idb = new IndirectFeeBLL(pus);

        //    int affectedCnt = idb.AddOrderItems(orderItems.ToArray());

        //    Assert.IsTrue(affectedCnt > 0, "should return something");
        //}

        [TestMethod]
        public void AddOrderItemsByBookingIDTestMethod()
        {
            OrderItemConstant orderItemCon = new OrderItemConstant();
            string[] groups = new string[1] { "RENTALFEE" };

            List<ProxyOrderItem> orderItems = new List<ProxyOrderItem>();
            ProxyOrderItem item1 = new ProxyOrderItem()
            {
                OrderID = 10,
                Category = orderItemCon.Groups[0],
                Type = "A",
                UnitPrice = 40,
                SalesQuantity = 5,
                AmountIncVAT = 200,
                Remark = "Some thing"
            };
            ProxyOrderItem item2 = new ProxyOrderItem()
            {
                OrderID = 10,
                Category = orderItemCon.Groups[1],
                Type = "B",
                UnitPrice = 40,
                SalesQuantity = 5,
                AmountIncVAT = 200,
                Remark = "Some thing"
            };
            ProxyOrderItem item3 = new ProxyOrderItem()
            {
                OrderID = 10,
                Category = orderItemCon.Groups[2],
                TypeID = 4,
                Type = "LateREturn",
                UnitPrice = 40,
                SalesQuantity = 5,
                AmountIncVAT = 200,
                Remark = "Some thing"
            };

            orderItems.Add(item1);
            orderItems.Add(item2);
            orderItems.Add(item3);

            ProxyUserSetting pus = new ProxyUserSetting();
            pus.ID = cfUserID.ToString();

            IndirectFeeBLL idb = new IndirectFeeBLL(pus);

            int affectedCnt = idb.AddOrderItems(3, orderItems.ToArray());

            Assert.IsTrue(affectedCnt > 0, "should return something");
        }

        [TestMethod]
        public void RetrieveOrderItemsByGroupTestMethod()
        {
            OrderItemConstant orderItemCon = new OrderItemConstant();
            string[] groups = new string[1] { "RENTALFEE" };


            ProxyUserSetting pus = new ProxyUserSetting();
            pus.ID = cfUserID.ToString();

            IndirectFeeBLL idb = new IndirectFeeBLL(pus);

            ProxyOrderItem[] items = idb.RetrieveOrderItems(3,groups);

            string jsonResponse = SerializedHelper.JsonSerialize<ProxyOrderItem[]>(items);
            Assert.IsTrue(items.Length > 0, "should return something");
        }
    }
}
