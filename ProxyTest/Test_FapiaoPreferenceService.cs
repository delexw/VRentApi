using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using CF.VRent.Entities;
using CF.VRent.Common;


using System.Configuration;
using CF.VRent.Entities.FapiaoPreferenceProxy;
using CF.VRent.Contract;
using CF.VRent.BLL;
using CF.VRent.Common.UserContracts;

namespace ProxyTest
{
    [TestClass]
    public class Test_FapiaoPreferenceService
    {
        string prefix = ConfigurationManager.AppSettings["TestDir"];
        string uniqueID = "a9118286-f601-4849-84c1-cea366bf3bd5";

        private WebClient client;
        public Test_FapiaoPreferenceService()
        {
        }

        public string testUserID = "1c9d9c82-d074-45a4-863e-e7eeb2384c64";

        #region Create a new FapiaoPreference
        
        [TestMethod]
        public void SaveFapiaoPreferenceTest()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            ProxyFapiaoPreference pfp = new ProxyFapiaoPreference();

            pfp.CustomerName = "ABCDE";
            pfp.MailAddress = "Beijing";
            pfp.MailPhone = "1234567";
            pfp.AddresseeName = "Bob";


            //string request = SerializedHelper.JsonSerialize<ProxyFapiaoPreference>(pfp);


            IFapiaoPreference ifp = new FapiaoPreferenceImpl(setting);
            ProxyFapiaoPreference fpdDB = ifp.SaveFapiaoPreference(pfp);

            string response = SerializedHelper.JsonSerialize<ProxyFapiaoPreference>(fpdDB);

            Assert.IsTrue(fpdDB != null, "should create a fapiao preference!");
        }

        [TestMethod]
        public void TestCreateFapiaoPreference()
        {
            string actual = string.Empty;
            string data = ProxyTest.DatReader.Read(prefix + "CreateFapiaoPreferenceData.txt");
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.UploadString(UnitTestConfiguration.GetHostName() + "/DataService/User/1c9d9c82-d074-45a4-863e-e7eeb2384c64/FaPiaoPreferences", "POST", data);
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            actual = client.UploadString(UnitTestConfiguration.GetHostName() + "/DataService/User/" + testUserID + "/FaPiaoPreferences", "POST", data);

            Assert.AreNotEqual(expected, actual);

            //check return data is the correct fromat
            ProxyFapiaoPreference Res = SerializedHelper.JsonDeserialize<ProxyFapiaoPreference>(actual);
            Assert.IsNotNull(Res);

            ProxyFapiaoPreference SourceRes = SerializedHelper.JsonDeserialize<ProxyFapiaoPreference>(data);
            Assert.AreEqual(SourceRes.CustomerName, Res.CustomerName);
            Assert.AreEqual(SourceRes.MailAddress, Res.MailAddress);
            Assert.AreEqual(SourceRes.MailPhone, Res.MailPhone);
            Assert.AreEqual(SourceRes.AddresseeName, Res.AddresseeName);
            Assert.AreEqual(SourceRes.FapiaoType, Res.FapiaoType);
        }

        #endregion

        #region retrieve an existing FapiaoPreference
        [TestMethod]
        public void RetrieveFapiaoPreferenceDetailTest()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            string fpID = "5bd1f4b4-07cc-4c6f-a9be-dd2bcfb50c09";

            IFapiaoPreference ifp = new FapiaoPreferenceImpl(setting);
            ProxyFapiaoPreference fpdDB = ifp.GetFapiaoPreferenceDetail(fpID);

            string response = SerializedHelper.JsonSerialize<ProxyFapiaoPreference>(fpdDB);

            Assert.IsTrue(fpdDB != null, "should retrieve a fapiao preference!");
        }

        [TestMethod]
        public void TestGetFapiaoPreferenceDetail()
        {
            //
            // TODO: Add test logic here
            //
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/User/1c9d9c82-d074-45a4-863e-e7eeb2384c64/FaPiaoPreferences/5119f9a4-f231-48b4-87e9-057fc56a1c2d");
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/User/" + testUserID + "/FaPiaoPreferences/" + uniqueID);
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            Assert.AreNotEqual(expected, actual);

            //check return data is the correct fromat
            ProxyFapiaoPreference Res = SerializedHelper.JsonDeserialize<ProxyFapiaoPreference>(actual);
            Assert.IsNotNull(Res);

            Assert.AreEqual("1c9d9c82-d074-45a4-863e-e7eeb2384c64", Res.UserID);
            Assert.AreEqual("5119f9a4-f231-48b4-87e9-057fc56a1c2d", Res.ID);
        }

        #endregion

        #region Update an existing FapiaoPreference

        [TestMethod]
        public void UpdateFapiaoPreferenceTest()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            string fpID = "5bd1f4b4-07cc-4c6f-a9be-dd2bcfb50c09";

            IFapiaoPreference ifp = new FapiaoPreferenceImpl(setting);
            ProxyFapiaoPreference fpdDB = ifp.GetFapiaoPreferenceDetail(fpID);

            string response = SerializedHelper.JsonSerialize<ProxyFapiaoPreference>(fpdDB);

            fpdDB.CustomerName = "QWERT";
            fpdDB.MailAddress = "Shanghai";
            fpdDB.MailPhone = "9999999";
            fpdDB.AddresseeName = "DDDDD";

            ProxyFapiaoPreference latest = ifp.UpdateFapiaoPreference(fpdDB);

            Assert.IsTrue(latest != null, "update a fapiao preference!");
        }
        [TestMethod]
        public void TestUpdateFapiaoPreference()
        {
            //
            // TODO: Add test logic here
            //
            string actual = string.Empty;
            string data = ProxyTest.DatReader.Read(prefix + "UpdateFapiaoPreferenceData.txt");
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.UploadString(UnitTestConfiguration.GetHostName() + "/DataService/User/1c9d9c82-d074-45a4-863e-e7eeb2384c64/FaPiaoPreferences/e1ce8b13-2a0f-4a83-aba3-92b0974f4c99", "PUT", data);

            string expected = string.Empty;
            Assert.AreNotEqual(expected, actual);

            //check return data is the correct fromat
            ProxyFapiaoPreference Res = SerializedHelper.JsonDeserialize<ProxyFapiaoPreference>(actual);
            Assert.IsNotNull(Res);

            ProxyFapiaoPreference fp = SerializedHelper.JsonDeserialize<ProxyFapiaoPreference>(data);
            Assert.IsNotNull(fp);

            Assert.AreEqual(Res.CustomerName, fp.CustomerName);
            Assert.AreEqual(Res.MailAddress, fp.MailAddress);
            Assert.AreEqual(Res.MailPhone, fp.MailPhone);
            Assert.AreEqual(Res.AddresseeName, fp.AddresseeName);
            //equal test            
            Assert.AreEqual(Res.FapiaoType, fp.FapiaoType);
            Assert.AreEqual(Res.UserID, fp.UserID);
        }

        #endregion

        #region retrieve all My FapiaoPreference
        
        [TestMethod]
        public void RetrieveMyFapiaoPreferencesTest()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            IFapiaoPreference ifp = new FapiaoPreferenceImpl(setting);
            ProxyFapiaoPreference[] existingFP = ifp.GetAllFapiaoPreference(testUserID);

            string response = SerializedHelper.JsonSerialize<ProxyFapiaoPreference[]>(existingFP);

            Assert.IsTrue(existingFP.Length > 0, "retrieve my fapiaoPreferences!");
        }

        [TestMethod]
        public void TestGetAllFapiaoPreference()
        {        
            string actual = string.Empty;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/User/1c9d9c82-d074-45a4-863e-e7eeb2384c64/FaPiaoPreferences");
            client.Encoding = System.Text.Encoding.UTF8;

            actual = client.DownloadString(UnitTestConfiguration.GetHostName() + "/DataService/User/" + testUserID + "/FaPiaoPreferences");
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            Assert.AreNotEqual(expected, actual);

          
            //check return data is the correct fromat
  
            List<ProxyFapiaoPreference> Res = SerializedHelper.JsonDeserialize<List<ProxyFapiaoPreference>>(actual);
            Assert.IsNotNull(Res);
            Assert.IsTrue(Res.Count() > 0);     
        }

        #endregion

        #region Delete a FapiaoPreference

        [TestMethod]
        public void TestDeleteFapiaoPreference()
        {

            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);

            string fpID = "5bd1f4b4-07cc-4c6f-a9be-dd2bcfb50c09";

            IFapiaoPreference ifp = new FapiaoPreferenceImpl(setting);
            ProxyFapiaoPreference fpdDBBefore = ifp.GetFapiaoPreferenceDetail(fpID);

            ifp.DeleteFapiaoPreference(fpID);

            ProxyFapiaoPreference fpdDBAfter = ifp.GetFapiaoPreferenceDetail(fpID);

            Assert.IsTrue(fpdDBAfter == null, "no longer exists");
        }

        #endregion
    }
}
