using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
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
    public class RequestFapiaoUnitTest
    {
        #region Fapiao Request

        [TestMethod]
        public void RetrieveFapiaoRequestUnitTest()
        {
            string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";
            UserProfile up = new UserProfile()
            {
                UserID = Guid.Parse(testUserID),
                CorporateID = null
            };

            int proxybookingID = 40;
            List<int> sources = new List<int>();

            FapiaoSource source1 = FapiaoSource.RentalFee;
            FapiaoSource source2 = FapiaoSource.IndirectFee;

            sources.Add((int)source1);
            sources.Add((int)source2);

            ReturnResultExtRetrieve fapiaoRequest = RequestFapiaoDAL.RetrieveFapiaoRequestDetailFullByFapiaoSource(proxybookingID, sources.ToArray(), up);

            Assert.IsTrue(fapiaoRequest != null, "should create a fapiao data record!");
        }

        [TestMethod]
        public void UpdateFapiaoRequestUnitTest()
        {
            string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";
            UserProfile up = new UserProfile()
            {
                UserID = Guid.Parse(testUserID),
                CorporateID = null
            };
            int proxybookingID = 40;
            FapiaoSource source = FapiaoSource.RentalFee;

            ProxyFapiaoRequest request = new ProxyFapiaoRequest()
            {
                ProxyBookingID = proxybookingID,
                ID = 1,
                FapiaoPreferenceID = Guid.Parse("03e59ac3-ae93-48d1-9a34-0b12c6a09ca6"),
                FapiaoSource = (int)source,
                ModifiedOn = DateTime.Now,
                ModifiedBy = Guid.NewGuid()
            };

            ReturnResultExt fapiaoRequest = RequestFapiaoDAL.UpdateFapiaoRequest(request, up);

            Assert.IsTrue(fapiaoRequest != null, "should create a fapiao data record!");
        }

        [TestMethod]
        public void UpdateFapiaoRequestToNoFPUnitTest()
        {
            string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";
            UserProfile up = new UserProfile()
            {
                UserID = Guid.Parse(testUserID),
                CorporateID = null
            };

            int proxybookingID = 40;
            FapiaoSource source = FapiaoSource.RentalFee;

            ProxyFapiaoRequest request = new ProxyFapiaoRequest()
            {
                ProxyBookingID = proxybookingID,
                ID = 1,
                FapiaoPreferenceID = null,
                FapiaoSource = (int)source,
                ModifiedOn = DateTime.Now,
                ModifiedBy = Guid.NewGuid()
            };

            ReturnResultExt fapiaoRequest = RequestFapiaoDAL.UpdateFapiaoRequest(request, up);

            Assert.IsTrue(fapiaoRequest != null, "should create a fapiao data record!");
        }

        #endregion


        [TestMethod]
        public void UpdateBookingFPUnitTest() 
        {
            string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";
            UserProfile up = new UserProfile()
            {
                UserID = Guid.Parse(testUserID),
                CorporateID = null
            };

            ProxyFapiaoRequest fpReq = new ProxyFapiaoRequest();
            fpReq.ProxyBookingID = 3;
            fpReq.FapiaoPreferenceID = Guid.NewGuid();

            fpReq.FapiaoSource = (int)FapiaoSource.RentalFee;

            fpReq.State = (int)FapiaoState.Active;

            fpReq.CreatedOn = DateTime.Now;
            fpReq.CreatedBy = Guid.NewGuid();

            fpReq.ModifiedOn = DateTime.Now;
            fpReq.ModifiedBy = Guid.NewGuid();

            ReturnResultExt fapiaoRequest = RequestFapiaoDAL.UpdateFapiaoRequest(fpReq, up);

            Assert.IsTrue(fapiaoRequest != null);

        }

        [TestMethod]
        public void RemoveFapiaoRequestUnitTest()
        {
            string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";
            UserProfile up = new UserProfile()
            {
                UserID = Guid.Parse(testUserID),
                CorporateID = null
            };

            ProxyFapiaoRequest fpReq = new ProxyFapiaoRequest();
            fpReq.ProxyBookingID = 3;
            fpReq.FapiaoPreferenceID = null;
            fpReq.FapiaoSource = (int)FapiaoSource.RentalFee;

            fpReq.State = (int)FapiaoState.Active;

            fpReq.CreatedOn = DateTime.Now;
            fpReq.CreatedBy = Guid.NewGuid();

            fpReq.ModifiedOn = DateTime.Now;
            fpReq.ModifiedBy = Guid.NewGuid();

            ReturnResultExt fapiaoRequest = RequestFapiaoDAL.UpdateFapiaoRequest(fpReq, up);

            Assert.IsTrue(fapiaoRequest != null);

        }
    }
}
