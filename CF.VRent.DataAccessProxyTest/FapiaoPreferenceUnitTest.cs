using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using System.Collections.Generic;
using CF.VRent.Common;

namespace CF.VRent.DataAccessProxyTest
{
    [TestClass]
    public class FapiaoPreferenceUnitTest
    {

        public string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";

        [TestMethod]
        public void SaveFapiaoPreferenceTest()
        {
            ProxyFapiaoPreference pfp = new ProxyFapiaoPreference();

            pfp.ID = Guid.NewGuid().ToString();
            pfp.UserID = testUserID;
            pfp.CustomerName = "ABCDE";
            pfp.MailType = FapiaoDeliverType.Express.ToString(); 
            pfp.MailAddress = "Beijing";
            pfp.MailPhone = "1234567";
            pfp.AddresseeName = "Bob";
            pfp.FapiaoType = (int)FapiaoType.RentalFee;
            pfp.State = (int)CommonState.Active; //0:active, 1:delete
            pfp.CreatedOn = DateTime.Now;
            pfp.CreatedBy = Guid.Parse(testUserID);

            ProxyFapiaoPreference fpdDB = FapiaoPreferenceDAL.SaveFapiaoPreference(pfp);

            Assert.IsTrue(fpdDB != null, "should create a fapiao preference!");
        }

        [TestMethod]
        public void RetrieveFapiaoPreferenceDetailTest()
        {

            string fpID = "3148b22d-55cf-4828-8bbf-9314fe6acb61";

            ProxyFapiaoPreference fpdDB = FapiaoPreferenceDAL.GetFapiaoPreferenceDetail(fpID);

            Assert.IsTrue(fpdDB != null, "should retrieve a fapiao preference!");
        }


        [TestMethod]
        public void UpdateFapiaoPreferenceTest()
        {
            string fpID = "35e1fe22-126e-427f-8dc9-47e5e6aaeae3";
            ProxyFapiaoPreference existingfp = FapiaoPreferenceDAL.GetFapiaoPreferenceDetail(fpID);


            ProxyFapiaoPreference newFp = new ProxyFapiaoPreference();
            newFp.ID = Guid.NewGuid().ToString();
            newFp.UserID = existingfp.UserID;
            newFp.FapiaoType = (int)FapiaoType.RentalFee;

            newFp.CustomerName = "poiuy";
            newFp.MailAddress = "Tianjin";
            newFp.MailPhone = "zxcvb";
            newFp.AddresseeName = "Tom";
            newFp.MailType = FapiaoDeliverType.Express.ToString();
            newFp.State = (int)CommonState.Active;
            newFp.CreatedOn = DateTime.Now;
            newFp.CreatedBy = Guid.NewGuid();


            existingfp.State = (int)CommonState.Deleted;

            ProxyFapiaoPreference latestFPd = FapiaoPreferenceDAL.UpdateFapiaoPreference(existingfp, newFp);

            Assert.IsTrue(latestFPd != null, "update a fapiao preference!");
        }


        [TestMethod]
        public void GetAllFapiaoPreference()
        {
            List<ProxyFapiaoPreference> pres = FapiaoPreferenceDAL.GetAllFapiaoPreference(testUserID);

            Assert.IsTrue(pres.Count > 0, "there should be a few active FapiaoPreference");
        }


        [TestMethod]
        public void DeleteFapiaoPreference()
        {
            string fpID = "3148b22d-55cf-4828-8bbf-9314fe6acb61";

            //retrieve the one
            ProxyFapiaoPreference fpdDB = FapiaoPreferenceDAL.GetFapiaoPreferenceDetail(fpID);

            FapiaoPreferenceDAL.DeleteFapiaoPreference(fpID);

            List<ProxyFapiaoPreference> pres = FapiaoPreferenceDAL.GetAllFapiaoPreference(testUserID);

            Assert.IsTrue(pres.Contains(fpdDB) == false, "no longer exists!");

        }
    }
}
