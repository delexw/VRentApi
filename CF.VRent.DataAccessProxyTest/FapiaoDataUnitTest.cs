using CF.VRent.Common;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF.VRent.DataAccessProxyTest
{

    [TestClass]
    public class FapiaoDataUnitTest
    {

        [TestMethod]
        public void SaveFapiaoDataTest()
        {
            ProxyFapiao pfDetail = new ProxyFapiao();

            pfDetail.OrderID = 1;

            pfDetail.UniqueID = "Crm";
            pfDetail.DealNumber = "ABCDE";
            pfDetail.ContractNumber = "ABCDE";

            pfDetail.CustomerCode = "ABCDE";
            pfDetail.CustomerName = "ABCDE";
            pfDetail.TaxRegistrationID = "ABCDE";
            pfDetail.CustomerAddress = "ABCDE";
            pfDetail.CustomerPhone = "ABCDE";

            pfDetail.BankName = "ABCDE";
            pfDetail.BankAccount = "ABCDE";

            pfDetail.FPCustomerName = "ABCDE";
            pfDetail.FPMailType = "ABCDE";
            pfDetail.FPMailingAddress = "ABCDE";
            pfDetail.FPMailingPhone = "ABCDE";
            pfDetail.FPAddresseeName = "ABCDE";

            pfDetail.ProductCode = "ABCDE";
            pfDetail.SpecMode = "ABCDE";
            pfDetail.UnitMeasure = "ABCDE";
            pfDetail.SalesQuantity = 1;
            pfDetail.UnitPrice = 5.85m;

            pfDetail.AmountExclVAT = 5.00m;
            pfDetail.TaxRate = 1.234m;
            pfDetail.Tax = 0.85m;
            pfDetail.AmountIncVAT = 5.85m;

            pfDetail.FapiaoType = 1;
            pfDetail.Remark = "test";

            pfDetail.FapiaoNumber = 12345;
            pfDetail.FapiaoCode = 23456;
            pfDetail.IssueDate = DateTime.Now;
            pfDetail.MailID = "45678";


            pfDetail.FapiaoState = 0;

            pfDetail.CreatedOn = DateTime.Now;
            pfDetail.CreatedBy = Guid.NewGuid();

            ProxyFapiao fpdDB = FapiaoDataDAL.CreateFapiaoData(pfDetail);

            Assert.IsTrue(fpdDB != null, "should create a fapiao data record!");
        }


        [TestMethod]
        public void RetrieveFapiaoDataDetailTest()
        {
            int FapiaoDataID = 3;

            ProxyFapiao fpdDB = FapiaoDataDAL.RetrieveFapiaoDataDetail(FapiaoDataID);

            Assert.IsTrue(fpdDB != null, "should create a fapiao data record!");
        }

        [TestMethod]
        public void RetrieveAllMyFapiaoDataTest()
        {
            //it is not testable
            Guid userID = Guid.Empty;

            ProxyFapiao[] myfpdatas = FapiaoDataDAL.RetrieveAllMyFapiaoData(userID);

            Assert.IsTrue(myfpdatas != null, "should create a fapiao data record!");
        }

    }
}
