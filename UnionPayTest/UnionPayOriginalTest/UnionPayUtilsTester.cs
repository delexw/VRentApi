using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.UPSDK;
using System.Collections.Generic;
using CF.VRent.Entities.KemasWrapper;
using UnionPayTest.TestHeaders;
using CF.VRent.Common;
using CF.VRent.UPSDK.Entities;
using CF.VRent.UPSDK.SDK;
using System.Text;
using System.Threading;
using CF.VRent.Common.Entities;

namespace UnionPayTest.UnionPayOriginalTest
{
    [TestClass]
    public class UnionPayUtilsTester:TestHeader
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        private string _convert(UnionPay pay)
        {
            return HttpClient.CreateLinkstringUrlencode(pay.ToDictionary<string, string>(true, "UniqueID"), Encoding.UTF8);
        }

        private void _storeUpParameters(UnionPayUtils pay)
        {
            TestHeader.UnionPayParams.UpToken = pay.UnionPayRequest.TokenPayData;
            TestHeader.UnionPayParams.QueryId = pay.UnionPayResponse.QueryId;
            TestHeader.UnionPayParams.Response = pay.UnionPayResponse;
            TestHeader.UnionPayParams.OrderTime = pay.UnionPayResponse.TxnTime;
            TestHeader.UnionPayParams.OrderId = pay.UnionPayResponse.OrderId;
        }

        [TestMethod()]
        public void Preauth()
        {
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            pay.TxnTime = TestHeader.UnionPayParams.OrderTime;
            pay.OrderId = TestHeader.UnionPayParams.OrderId;
            pay.TxnAmt = "123";
            pay.CurrencyCode = "156";
            pay.BackUrl = "http://booking-vrent.mcon.net/uplTest/ListenService/ListenCompletePreauth3";
            var ret = pay.PreAuthorize(TestHeader.UnionPayParams.UpToken);
            _storeUpParameters(pay);
            Assert.AreEqual(ret.Success, 1);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
            TestContext.WriteLine(TestHeader.UnionPayParams.QueryId);
        }

        [TestMethod()]
        public void CompletePreauth()
        {
            UnionPayUtils pay = new UnionPayUtils();
            pay.OrigQryId = TestHeader.UnionPayParams.QueryId;
            pay.TxnAmt = "12.3";
            pay.BackUrl = "http://booking-vrent.mcon.net/uplTest/ListenService/ListenCompletePreauth2";
            var ret = pay.FinishPreAuthorization();
            Assert.AreEqual(ret.Success, 1);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }


        [TestMethod()]
        public void CancelPreauthCompleted()
        {
            //UnionPayUtils pay = new UnionPayUtils();
            //pay.OrigQryId = "201507241132212553608";
            //pay.BackUrl = "http://booking-vrent.mcon.net/uplTest/ListenService/ListenCompletePreauth2";
            //pay.TxnAmt = "10";
            //var ret = pay.ReturnGoodsConsume();
            //Assert.AreEqual(ret.Success, 1);
            //TestContext.WriteLine(ret.Message);
        }

        [TestMethod()]
        public void CheckPaymentStatus()
        {
            UnionPayUtils pay = new UnionPayUtils();
            pay.OrigQryId = TestHeader.UnionPayParams.QueryId;
            pay.UPStateTime = new UnionPayState("03");

            ReturnResult ret = new ReturnResult();

            foreach (int i in pay.UPStateTime.TimeSpan)
            {
                Thread.Sleep(i);
                ret = pay.CheckPaymentStatus();
                
            }
            Assert.AreEqual(ret.Success, 1);
            var a = pay.UnionPayResponse.ToDictionary<string, string>(true, "UniqueID");
            TestContext.WriteLine(HttpClient.CreateLinkstringUrlencode(a, Encoding.UTF8));
        }

        [TestMethod()]
        public void Deduction()
        {
            UnionPayUtils pay = new UnionPayUtils();
            pay.TxnAmt = "123";
            pay.BackUrl = "http://booking-vrent.mcon.net/uplTest/ListenService/ListenCompletePreauth2";
            var ret = pay.Consume(TestHeader.UnionPayParams.UpToken);
            _storeUpParameters(pay);
            Assert.AreEqual(ret.Success, 1);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
            TestContext.WriteLine(ret.Message);
        }

        [TestMethod()]
        public void SendPreauthSmsCode()
        {
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            pay.TxnAmt = "123";
            var ret = pay.SendPreauthorizaitonSMS(TestHeader.UnionPayParams.UpToken);
            _storeUpParameters(pay);
            Assert.AreEqual(ret.Success, 1);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        #region BindCard
        [TestMethod()]
        public void BindCardAllInfoCorrect()
        {
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 1);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongCardNo()
        {
            TestHeader.UpCustomer.CardNo = "1111111";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongUserCertIdLengthLessThanRegular()
        {
            TestHeader.UpCustomer.CertifId = "1111111";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongUserCertIdLengthEqualRegular()
        {
            TestHeader.UpCustomer.CertifId = "341126197709218367";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongCustomNm()
        {
            TestHeader.UpCustomer.CustomerNm = "111";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongSmsCodeLengthLessThanSix()
        {
            TestHeader.UpCustomer.SmsCode = "111";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongSmsCodeLengthEqualSix()
        {
            TestHeader.UpCustomer.SmsCode = "123456";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongPhoneNoLengthLessThanRegular()
        {
            TestHeader.UpCustomer.PhoneNo = "1383381823";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongPhoneNoLengthEqualRegular()
        {
            TestHeader.UpCustomer.PhoneNo = "18600340025";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongCvn2LengthLessThanThree()
        {
            TestHeader.UpCustomer.Cvn2 = "11";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongCvn2LengthEqualThree()
        {
            TestHeader.UpCustomer.Cvn2 = "456";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongCvn2LengthGreaterThanThree()
        {
            TestHeader.UpCustomer.Cvn2 = "4561";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongExpiredLengthLessThanFour()
        {
            TestHeader.UpCustomer.Expired = "171";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongExpiredLengthEqualFour()
        {
            TestHeader.UpCustomer.Expired = "1712";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongExpiredLengthGreaterThanFour()
        {
            TestHeader.UpCustomer.Expired = "17111";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void BindCardWithWrongExpiredParttenMonthYear()
        {
            TestHeader.UpCustomer.Expired = "1117";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.OpenUnionPayCard();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }
        #endregion


        #region SendBindCardSmsCode
        [TestMethod()]
        public void SendBindCardSmsCode()
        {
            var pay = UPSDKFactory.CreateUtls(TestHeader.UpCustomer);
            var ret = pay.SendVerificationSMS();
            Assert.AreEqual(ret.Success, 1);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void SendBindCardSmsCodeWithWrongPhoneNoLengthLessThanRegular()
        {
            TestHeader.UpCustomer.PhoneNo = "1383381823";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.SendVerificationSMS();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void SendBindCardSmsCodeWithWrongPhoneNoLengthEqualRegular()
        {
            TestHeader.UpCustomer.PhoneNo = "13833818234";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.SendVerificationSMS();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        }

        [TestMethod()]
        public void SendBindCardSmsCodeWithWrongPhoneNoLengthGreaterThanRegular()
        {
            TestHeader.UpCustomer.PhoneNo = "138338182341";
            UnionPayUtils pay = new UnionPayUtils(TestHeader.UpCustomer);
            var ret = pay.SendVerificationSMS();
            Assert.AreEqual(ret.Success, 0);
            TestContext.WriteLine(_convert(pay.UnionPayResponse));
        } 
        #endregion
    }
}
