using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.BLL.BLLFactory;
using UnionPayTest.TestHeaders;
using CF.VRent.Common.Entities;
using CF.VRent.Common;
using CF.VRent.Entities.PaymentService;
using CF.VRent.Entities.DataAccessProxyWrapper;
using VWFSCN.IT.Log;

namespace UnionPayTest.TransactionTest
{
    [TestClass]
    public class TransactionTester
    {
        [TestMethod]
        public void PreRetry()
        {
            var tranc = ServiceImpInstanceFactory.CreateTransactionInstance(TestHeader.User);
            var payment = tranc.UpdateExchangeMessageEnableRetry(7);
            Assert.AreEqual(payment.Retry, VRentDataDictionay.TransactionRetry.Retry.GetValue());
        }
        [TestMethod]
        public void DisablePreRetry()
        {
            var tranc = ServiceImpInstanceFactory.CreateTransactionInstance(TestHeader.User);
            var payment = tranc.UpdateExchangeMessageDisableRetry(7);
            Assert.AreEqual(payment.Retry, VRentDataDictionay.TransactionRetry.Default.GetValue());
        }

        [TestMethod]
        public void RetryTrans()
        {
            var retry = ServiceImpInstanceFactory.CreateRetryInstance(TestHeader.User);
            var bookings = new DataAccessProxyManager().GetAllRetryBookings();
            retry.Retry(bookings);
        }

        [TestMethod]
        public void PreRetryByBooking()
        {
            var tranc = ServiceImpInstanceFactory.CreateTransactionInstance(TestHeader.User);
            var ret = tranc.UpdateExchangeMessageEnableRetryByBooking(158);
            Assert.AreEqual(ret.Success, 1);
        }
    }
}
