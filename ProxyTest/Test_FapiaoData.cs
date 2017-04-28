using CF.VRent.BLL;
using CF.VRent.Common;
using CF.VRent.Contract;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProxyTest
{
    [TestClass]
    public class Test_FapiaoData
    {
        #region new API
        [TestMethod]
        public void UnitTestRetrieveMyFapiaoData()
        {

            Guid userID = Guid.Empty;

            IDataService dsc = new DataAccessProxyManager();
            ProxyFapiao[] fapiaoData = dsc.RetrieveMyFapiaoData(userID);

            string response = SerializedHelper.JsonSerialize<ProxyFapiao[]>(fapiaoData);

            Assert.IsTrue(fapiaoData.Length > 0 && response != null, "should retrieve all my fapiao records!");
        }

        [TestMethod]
        public void RetrieveFapiaoDetail()
        {
            int existingFapiaoID = 3;

            IDataService dsc = new DataAccessProxyManager();
            ProxyFapiao fapiaoData = dsc.RetrieveFapiaoDataDetail(existingFapiaoID);

            string response = SerializedHelper.JsonSerialize<ProxyFapiao>(fapiaoData);

            Assert.IsTrue(fapiaoData != null && response != null, "should retrieve a fapiao data record!");
        }

        #endregion
    }
}
