using CF.VRent.BLL;
using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace ProxyTest
{
    [TestClass]
    public class CompanyUnitTest
    {
        [TestMethod]
        public void VerifyAppDomainHasConfigurationSettings()
        {
            var unity = ConfigurationManager.GetSection("unity");
            Assert.IsFalse(unity != null, "No App.Config found.");
        }

        [TestMethod]
        public void CombineComanyInfoUnitTest()
        {

        }

        #region Assign newly created client to all sc users

        [TestMethod]
        public void RetrieveSCUsersUnitTest()
        {
            UserSettingBLL usb = new UserSettingBLL();

            UserExtension ue = usb.Login("service.center@abc.com", "123456");
            ProxyUserSetting userInfo = ServiceUtility.ConvertFromUserExtention(ue);

            KemasUserAPI kemasRoleAPI = new KemasUserAPI();
            getRolesResponse grr = kemasRoleAPI.getRoles(userInfo.SessionID);

            Role SCRole = grr.Roles.FirstOrDefault(m => m.Name.Equals("Service Center"));

            UserData2[] scUSers = CompanyUtility.FindAllSCUserByRole(SCRole.Name, SCRole.ID, userInfo);

            Assert.IsTrue(scUSers != null);
        }

        [TestMethod]
        public void AddNewCLientToSCUsersUnitTest()
        {
            UserSettingBLL usb = new UserSettingBLL();

            UserExtension ue = usb.Login("jack.sun@crm-factory.com.cn", "123456");
            ProxyUserSetting userInfo = ServiceUtility.ConvertFromUserExtention(ue);

            CompanyProfileRequest cpr = new CompanyProfileRequest();
            cpr.CompanyProfile = new UserCompanyExtenstion()
            {
                Number = "Test",
                ID = "",
                Name = "BUG1",
                ContactInfo = "BUG1",

                Address = "BUG1",
                RegisteredAddress = "REG1",
                OfficeAddress = "Off1",

                BankAccountInfo = "Bank1",
                BankAccountName = "Name1",
                BankAccountNo = "1231",

                Comment = "BUG",
                ContactPerson = "BUG",
                Deposit = 123,
                BusinessLicenseID = "BUG",
                Enabled = 1,
                LegalRepresentativeID = "BUG",
                Mail = "corp@corp.com",
                OrgCodeCertificate = "BUG"
            };

            cpr.VMProfile = new UserExtension()
            {
                Mail = "Daniel.Li@crm-factory.com.cn",
                VName = "VM1",
                Name = "VM1"
            };

            string jsonReq = SerializedHelper.JsonSerialize<CompanyProfileRequest>(cpr);

            CompanyBLL cb = new CompanyBLL(userInfo);

            UserCompanyExtenstion created = cb.CreateCompany(cpr);


            KemasUserAPI kemasRoleAPI = new KemasUserAPI();
            getRolesResponse grr = kemasRoleAPI.getRoles(userInfo.SessionID);

            Role SCRole = grr.Roles.FirstOrDefault(m => m.Name.Equals("Service Center"));

            UserData2[] scUSers = CompanyUtility.FindAllSCUserByRole(SCRole.Name, SCRole.ID, userInfo);

            KemasConfigsAPI kemasClients = new KemasConfigsAPI();
            getClientsResponse clientsRes = kemasClients.getClients(userInfo.SessionID);


            int max = scUSers.Max(m => m.Clients == null ? 0 : m.Clients.Length);
            UserData2 maxClients = scUSers.FirstOrDefault(m => (m.Clients != null && m.Clients.Length == max));

            string[] newclients = new string[max + 1];

            Array.Copy(clientsRes.Clients.Select(m => m.ID).ToArray(), newclients, newclients.Length - 1);
            newclients[newclients.Length - 1] = created.ID;

            UserData2[] updateSCUsers = CompanyUtility.AddNewClientToScUsers(newclients,created.Name,SCRole.ID,SCRole.Name, scUSers, userInfo, "english");

            Assert.IsTrue(scUSers != null);
        }

        #endregion Assign newly created client to all sc users


        [TestMethod]
        public void CreateclientTestMethod()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("service.center@abc.com", "123456");
            
            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = auth.ID.ToString();
            userinfo.SessionID = auth.SessionID;

            CompanyProfileRequest cpr = new CompanyProfileRequest();


            cpr.CompanyProfile = new UserCompanyExtenstion()
            {
                Number = "Test",
                ID = "",
                Name = "BUG2",

                Address = "BUG2",
                RegisteredAddress = "REG",
                OfficeAddress = "Off",
                ContactInfo = "BUG2",

                BankAccountInfo = "Bank",
                BankAccountName = "Name",
                BankAccountNo = "123",

                Comment = "BUG2",
                ContactPerson = "BUG",
                Deposit = 123,
                BusinessLicenseID = "BUG",
                Enabled = 1,
                LegalRepresentativeID = "BUG",
                Mail = "corp1@corp1.com",
                OrgCodeCertificate = "BUG"
            };

            cpr.VMProfile = new UserExtension()
            {
                Mail = "vm2@vm2.com",
                VName = "VM2",
                Name = "VM2",
                Phone = "1234567890"
            };

            string jsonReq = SerializedHelper.JsonSerialize<CompanyProfileRequest>(cpr);

            CompanyBLL cb = new CompanyBLL(userinfo);

            UserCompanyExtenstion created = cb.CreateCompany(cpr);

            UserCompanyExtenstion newclient = cb.RetrieveCompanyByID(created.ID);

            string jsonRes = SerializedHelper.JsonSerialize <UserCompanyExtenstion>(created);

            Assert.IsTrue(created != null, "add a user transfer request");
        }

        [TestMethod]
        public void ShawnCreateclientTestMethod()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("service.center@abc.com", "123456");

            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = auth.ID.ToString();
            userinfo.SessionID = auth.SessionID;

            string path = @"C:\CF-repo\vrent766\ProxyTest\TestData\Client\NewClient.txt";

            string jsonReq = File.ReadAllText(path);
            CompanyProfileRequest cpr = SerializedHelper.JsonDeserialize<CompanyProfileRequest>(jsonReq);

            CompanyBLL cb = new CompanyBLL(userinfo);

            UserCompanyExtenstion created = cb.CreateCompany(cpr);

            UserCompanyExtenstion newclient = cb.RetrieveCompanyByID(created.ID);

            string jsonRes = SerializedHelper.JsonSerialize<UserCompanyExtenstion>(created);

            Assert.IsTrue(created != null, "add a user transfer request");
        }




        [TestMethod]
        public void EnableDisableClientUnitTest() 
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("service.center@abc.com", "123456");

            KemasConfigsAPI kemasClients = new KemasConfigsAPI();
            getClientsResponse clientsRes = kemasClients.getClients(auth.SessionID);

            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client bugClient = clientsRes.Clients.FirstOrDefault(m => m.Name.Equals("BUG2"));

            if (bugClient != null)
            {
                ProxyUserSetting userinfo = new ProxyUserSetting();
                userinfo.ID = auth.ID.ToString();
                userinfo.SessionID = auth.SessionID;

                int status = 0;
                CompanyBLL cb = new CompanyBLL(userinfo);
                UserCompanyExtenstion bugCompany = cb.EnableDisableCompany(bugClient.ID, status);

                Assert.IsTrue(bugCompany.Status.Equals(status.ToString()), "should disable client"); 
            }

        }

        [TestMethod]
        public void EnableDisableClientRawUnitTest()
        {
            string raw = "{\"status\":0}";
            Status status = SerializedHelper.JsonDeserialize<Status>(raw);

            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("service.center@abc.com", "123456");

            KemasConfigsAPI kemasClients = new KemasConfigsAPI();
            getClientsResponse clientsRes = kemasClients.getClients(auth.SessionID);

            CF.VRent.Entities.KEMASWSIF_CONFIGRef.Client bugClient = clientsRes.Clients.FirstOrDefault(m => m.Name.Equals("Test Client Deactivation"));

            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = auth.ID.ToString();
            userinfo.SessionID = auth.SessionID;


            string statusString = SerializedHelper.JsonSerialize<int>(1);
            CompanyBLL cb = new CompanyBLL(userinfo);
            UserCompanyExtenstion bugCompany = cb.EnableDisableCompany(bugClient.ID, status.status);

            Assert.IsTrue(bugCompany.Status.Equals(status.ToString()), "should disable client");

        }

        [TestMethod]
        public void TestEscap() 
        {
            string rawAddress = "[&amp;amp;quot;REG&amp;amp;quot;,&amp;amp;quot;Off&amp;amp;quot;]";
            while(rawAddress.Contains("&amp;"))
            {
                rawAddress = rawAddress.Replace("&amp;", "&");
            }
            rawAddress = rawAddress.Replace("&quot;", "\"");

            string[] addressPArts = SerializedHelper.JsonDeserialize<string[]>(rawAddress);

        }
    }
}
