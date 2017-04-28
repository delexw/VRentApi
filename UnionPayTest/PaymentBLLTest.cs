using CF.VRent.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using CF.VRent.UPSDK.Entities;
using CF.VRent.Entities.PaymentService;
using System.Collections.Generic;
using CF.VRent.Entities;
using CF.VRent.Common;
using CF.VRent.UPSDK;
using System.Linq;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Mail;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Net.Mime;
using CF.VRent.Entities.KemasWrapper;
using System.Collections.Specialized;
using System.Net;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.UPSDK.SDK;
using CF.VRent.Common.UserContracts;
namespace UnionPayTest
{


    /// <summary>
    ///This is a test class for PaymentBLLTest and is intended
    ///to contain all PaymentBLLTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PaymentBLLTest
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

        private string _carId = "b686a472-2c77-4cd5-9ed9-96caedfc05a6";
        private string _paymentUniqueId = "92ae08b6-0b6b-4c32-a412-bf72c7d3b1ed";
        private int _paymentId = 30;
        private string _uid = "792d78f9-5d4f-439f-b5cf-8f0b58e2a87c";
        private string _tempOrderId = "20150604101728731";
        private string _tempOrderTime = "20150604101728";

        private static string UserId;
        private static string UserSessionId;
        private static ProxyUserSetting UserSetting;

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            KemasAuthencationAPI api = new KemasAuthencationAPI();
            var a = api.authByLogin("jack.sun@abc.com", "123456");
            UserId = a.ID;
            UserSessionId = a.SessionID;

            KemasUserAPI user = new KemasUserAPI();
            var userObj = user.findUser2(a.ID, a.SessionID);

            UserSetting = new ProxyUserSetting()
            {
                ID = a.ID,
                VName = a.VName,
                Name = a.Name,
                Mail = userObj.UserData.Mail,
                SessionID = a.SessionID
            };
        }

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
        ///A test for AddUPBindingCard
        ///</summary>
        [TestMethod()]
        public void AddUPBindingCardTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            UnionPayCustomInfo cusObj = new UnionPayCustomInfo()
            {
                CardNo = "6221558812340000",
                CertifId = "341126197709218366",
                CertifTp = "01",
                CustomerNm = "互联网",
                SmsCode = "111111",
                PhoneNo = "13552535506",
                Cvn2 = "123",
                Expired = "1711"
            }; // TODO: Initialize to an appropriate value;
            string uid = UserId; // TODO: Initialize to an appropriate value
            UnionPay returnUP = null; // TODO: Initialize to an appropriate value
            Action exceptionCallBack = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.AddUPBindingCard(cusObj, uid, returnUP, exceptionCallBack);
            Assert.IsNotNull(actual);

        }

        /// <summary>
        ///A test for GetUserCreditCard
        ///</summary>
        [TestMethod()]
        public void GetUserCreditCardTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            string uid = UserId; // TODO: Initialize to an appropriate value
            IEnumerable<PaymentCard> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<PaymentCard> actual;
            actual = target.GetUserCreditCard(uid);

            Assert.IsTrue(actual.Count() > 0);
        }

        /// <summary>
        ///A test for SendBindingSMSCode
        ///</summary>
        [TestMethod()]
        public void SendBindingSMSCodeTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            UnionPayCustomInfo cardObject = new UnionPayCustomInfo() { PhoneNo = "13552535506", CardNo = "6221558812340000" }; // TODO: Initialize to an appropriate value
            string uid = UserId; // TODO: Initialize to an appropriate value
            Payment expected = null; // TODO: Initialize to an appropriate value
            Payment actual;
            actual = target.SendBindingSMSCode(cardObject, uid);

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for SendPreauthorizationSMSCode
        ///</summary>
        [TestMethod()]
        public void SendPreauthorizationSMSCodeTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            UnionPayCustomInfo cardObject = new UnionPayCustomInfo() { PhoneNo = "13552535506", CardId = _carId }; // TODO: Initialize to an appropriate value
            string uid = _uid; // TODO: Initialize to an appropriate value
            string price = "11.11";
            Payment expected = null; // TODO: Initialize to an appropriate value
            Payment actual;
            actual = target.SendPreauthorizationSMSCode(cardObject, price, uid);

            Assert.IsNotNull(actual);
        }


        private Payment _preauth()
        {
            UnionPayCustomInfo cusObj = new UnionPayCustomInfo()
            {
                CardNo = "6221558812340000",
                CertifId = "341126197709218366",
                CertifTp = "01",
                CustomerNm = "互联网",
                SmsCode = "111111",
                PhoneNo = "13552535506",
                Cvn2 = "123",
                Expired = "1711"
            }; // TODO: Initialize to an appropriate value;

            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value

            KemasAuthencationAPI api = new KemasAuthencationAPI();

            string cardId = target.AddUPBindingCard(cusObj, UserId);
            string price = "1"; // TODO: Initialize to an appropriate value

            var t = UnionPayUtils.GenerateTempOrder();

            string tempOrderId = t.Item1; // TODO: Initialize to an appropriate value
            string tempOrderTime = t.Item2; // TODO: Initialize to an appropriate value
            // TODO: Initialize to an appropriate value
            // TODO: Initialize to an appropriate value

            Payment actual; ;

            actual = target.PreAuthorize(price, cardId, "111111", UserSetting, 0, tempOrderId, tempOrderTime);

            return actual;
        }

        /// <summary>
        ///A test for PreAuthorize
        ///</summary>
        [TestMethod()]
        public void PreAuthorizeTest()
        {
            var actual = _preauth();
            Assert.IsTrue(actual.PaymentID > 0);
        }

        /// <summary>
        ///A test for CheckPaymentStatus
        ///</summary>
        [TestMethod()]
        public void CheckPaymentStatusTest()
        {
            PaymentBLL target = new PaymentBLL();
            var pre = _preauth();
            string userId = UserSetting.ID; // TODO: Initialize to an appropriate value
            string expected = PaymentStatusEnum.PreAuthorized.ToString(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CheckPaymentStatus(pre.PaymentID.ToStr(), userId);

            Assert.AreEqual(expected, Enum.GetName(typeof(PaymentStatusEnum), actual));
        }

        /// <summary>
        ///A test for DeleteUPBindingCard
        ///</summary>
        [TestMethod()]
        public void DeleteUPBindingCardTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            string cardId = _carId; // TODO: Initialize to an appropriate value
            string uid = _uid; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.DeleteUPBindingCard(cardId, uid);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CancelCreditCard
        ///</summary>
        [TestMethod()]
        public void CancelCreditCardTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            string cardId = _carId; // TODO: Initialize to an appropriate value
            string uid = _uid; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CancelCreditCard(cardId, uid);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetPaymentStatusByBookingId
        ///</summary>
        [TestMethod()]
        public void GetPaymentStatusByBookingIdTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            int bookingId = 158; // TODO: Initialize to an appropriate value
            int expected = 14; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetPaymentStatusByBookingId(bookingId);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SendEmailWithTemplate()
        {
            PaymentBLL target = new PaymentBLL();
            Dictionary<string, string> paras = new Dictionary<string, string>();
            paras.Add("Price", "100");
            paras.Add("Name", "113");
            paras.Add("VName", "345");
            List<string> to = new List<string>();
            //to.Add("adam.liu@crm-factory.com.cn");
            //to.AddRange(ConfigurationManager.AppSettings["TestUser"].Split(','));
            //target.SendPaymentEmail(paras, EmailType.Preauthorization_Preauth_Successful, to.ToArray());
        }

        /// <summary>
        ///A test for UpdateBookingStatusAfterPayment
        ///</summary>
        [TestMethod()]
        public void UpdateBookingStatusAfterPaymentTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            string kmId = "313c5786-5e2c-45d6-ae69-b28316a5206e"; // TODO: Initialize to an appropriate value
            string state = "1"; // TODO: Initialize to an appropriate value
            string userId = "119ca92c-7509-434b-b558-51483696f219"; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.UpdateBookingStatusAfterPayment(kmId, state, userId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FeeDeduction
        ///</summary>
        [TestMethod()]
        public void FeeDeductionTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            int bookingId = 3; // TODO: Initialize to an appropriate value
            string userId = UserId; // TODO: Initialize to an appropriate value
            string userSessionId = UserSessionId; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.FeeDeduction(bookingId, UserSetting);

            //});
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RedoPreauth
        ///</summary>
        [TestMethod()]
        public void RedoPreauthTest()
        {
            PaymentBLL target = new PaymentBLL(); // TODO: Initialize to an appropriate value
            int bookingId = 4; // TODO: Initialize to an appropriate value
            string price = "100"; // TODO: Initialize to an appropriate value
            string cardId = "86f19455-5588-4516-8ba7-e4a695df3cc7"; // TODO: Initialize to an appropriate value
            string smsCode = "111111"; // TODO: Initialize to an appropriate value
            ProxyUserSetting user = UserSetting; // TODO: Initialize to an appropriate value

            var t = UnionPayUtils.GenerateTempOrder();

            string tempOrderId = t.Item1; // TODO: Initialize to an appropriate value
            string tempOrderTime = t.Item2; // TODO: Initialize to an appropriate value

            int actual;
            actual = target.RedoPreauth(bookingId, price, cardId, smsCode, user, tempOrderId, tempOrderTime);
            Assert.IsTrue(actual > 0);
        }

        [TestMethod()]
        public void TestKemasClients()
        {
            KemasConfigsAPI api = new KemasConfigsAPI();
            var clients = api.getClients(UserSessionId);
        }

        [TestMethod()]
        public void GetPaymentStatusTest()
        {
            PaymentBLL target = new PaymentBLL();
            var r = target.GetPaymentStatus(65, UserId);
            Assert.AreEqual(r, 1);
        }

        [TestMethod()]
        public void AddPaymentExchangeMessageHistory()
        {
            PaymentBLL target = new PaymentBLL();

            target.AddPaymentExchangeMessageHistory(target.GetPaymentExchangeInfo(1, UserId));
        }
    }
}
