using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Common;
using CF.VRent.BLL;
using CF.VRent.Entities;
using System.IO;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.EntityFactory;
using System.Xml.Serialization;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Contract;
using Microsoft.Practices.Unity;

namespace ProxyTest
{
    /// <summary>
    /// Summary description for Self_RegistrationUnitTest
    /// </summary>
    [TestClass]
    public class Self_RegistrationUnitTest
    {
        public Self_RegistrationUnitTest()
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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ReadUserDataUnitTest()
        {
            string UserDataFile = @"C:\CF-repo\vrent514-2\ProxyTest\TestData\GetUserData\BigUserData.xml";

            FileStream fs = null;
            XmlSerializer xs = null;
            getUsers2Response fu2R = null;
            if (File.Exists(UserDataFile))
            {
                try
                {
                    xs = new XmlSerializer(typeof(getUsers2Response));
                    fs = new FileStream(UserDataFile, FileMode.Open);
                    fu2R = xs.Deserialize(fs) as getUsers2Response;
                }
                catch (Exception ex)
                {
                    //throw new VrentApplicationException(UnknownErroProcesingPricingCode, string.Format(UnknownErroProcesingPricingMessage, ex.Message, ex.StackTrace, _xmlStr), ResultType.VRENT);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                        fs = null;
                    }

                    if (xs != null)
                    {
                        xs = null;
                    }
                }
            }

        }

        #region Registration
        [TestMethod]
        public void RawCreateUserTestMethod()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("3@3.com", "123456");

            WS_Auth_Response Adminauth = AuthencationUnitTestKemasAPI.SignOn();


            AppRegistrationBLL burb = new AppRegistrationBLL();

            updateUser2Request createUserRequest = new updateUser2Request();
            createUserRequest.SessionID = auth.SessionID;
            createUserRequest.Language = "english";

            updateUserData create = new updateUserData();

            create.Mail = "100@100.com";
            create.PNo = "InitReg0006";
            create.Department = "Registration";
            create.ID = string.Empty;
            create.Status = "NotEMPTY";

            createUserRequest.UserData = create;

            KemasUserAPI kemasUser = new KemasUserAPI();

            updateUser2Response createRet = kemasUser.updateUser2(createUserRequest);

            
            findUser2Response fu21 = kemasUser.findUser2(auth.ID,Adminauth.SessionID);
            //findUser2Response fu22 = kemasUser.findUser2(createRet.ID, auth.SessionID);



            UserData2 ud2 = createRet.UserData;

            Assert.IsTrue(ud2 != null && ud2.Status.Equals(create.Status), "shoud have the same status.");
        }

        [TestMethod]
        public void RegisterPrivateUserTestMethod()
        {
            AppRegistrationBLL burb = new AppRegistrationBLL();

            UserExtension ueFE = new UserExtension();
            ueFE.Mail = "rent1@rent1.com";
            ueFE.Password = "123456";
            UserExtension created = burb.UserRegistration(ueFE,"english");

            string res = SerializedHelper.JsonSerialize<UserExtension>(created);
            Assert.IsTrue(created != null && created.Status == "01000000000");
        }


        [TestMethod]
        public void RegisterCorporateUserTestMethod()
        {
            //string datafile = Path.Combine(@"C:\CF-repo\vrent514ext\ProxyTest\TestData\UserRegistration", "RegPrivateRequest.txt");
            //string data = ProxyTest.DatReader.Read(datafile);

            //UserExtension ueFE = SerializedHelper.JsonDeserialize<UserExtension>(data);

            UserExtension ueFE = new UserExtension();
            //    "LoginName":"ABCDE@ABCDE.com",
            //    "Password":"Password",
            ueFE.Mail = "4@4.com";
            ueFE.Password = "Password";
            ueFE.PNo = "InitReg0004";
            ueFE.Company = "Registration";
            ueFE.ClientID = "Registration";

            AppRegistrationBLL burb = new AppRegistrationBLL();

            UserExtension created = burb.UserRegistration(ueFE, "english");

            string response = SerializedHelper.JsonSerialize<UserExtension>(created);

            Assert.IsNotNull(created != null && created.StatusEntities[1].Value == 1 && response != null, "the use should flaged to init Reg");
        }

        #endregion

        [TestMethod]
        public void FindUserRolesTestMethod()
        {
            UserSettingBLL ub = new UserSettingBLL();
            UserExtension profile = ub.Login("1@1.com", "123456");

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(profile);


            KemasUserAPI kemasUser = new KemasUserAPI();
            //string userID = "e54ca541-b92c-49d0-a3dd-2ae080e5288e";

            findUser2Response user2Res = kemasUser.findUser2(setting.ID, setting.SessionID);
            findUser_Response user1Res = kemasUser.findUser(setting.ID);
            getRolesResponse roles = kemasUser.getRoles(setting.SessionID);

            RightBLL rb = new RightBLL(setting);
            List<ProxyRole> roles1 = rb.GetAllRoles(setting.ID,setting.SessionID);


            Role[] kemasroles = user2Res.UserData.Roles;

        }

        #region Change PWD

        [TestMethod]
        public void RawChangePWDUserTestMethod()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("3@3.com", "123");
            ProxyUserSetting pus = new ProxyUserSetting();
            pus.ID = auth.ID;
            pus.SessionID = auth.SessionID;


            updateUserData uud = new updateUserData();
            uud.ID = auth.ID;
            uud.Password = "123";
            uud.CurrentPassword = "123456";

            updateUser2Request uu2r = new updateUser2Request();
            uu2r.Language = "english";
            uu2r.SessionID = auth.SessionID;
            uu2r.UserData = uud;

            KemasUserAPI kemasUser = new KemasUserAPI();

            updateUser2Response updated =  kemasUser.updateUser2(uu2r);

        }

        [TestMethod]
        public void ChangePWDUserTestMethod()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("3@3.com", "123456");
            ProxyUserSetting pus = new ProxyUserSetting();
            pus.ID = auth.ID;
            pus.SessionID = auth.SessionID;

            UserExtension ueFE = new UserExtension();
            ueFE.ID = auth.ID;
            ueFE.Password = "123";
            ueFE.RepeatPassword = "123456";

            AppRegistrationBLL burb = new AppRegistrationBLL(pus);

            UserExtension updated = burb.ChangePassword(ueFE,"english");

            string response = SerializedHelper.JsonSerialize<UserExtension>(updated);
        }

        #endregion


        [TestMethod]
        public void UpdateProfileTestMethod()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("rent1@rent1.com", "123456");

            UserExtension ueFE = new UserExtension();
            ueFE.ID = auth.ID;
            ueFE.PNo = "ABCDE";
            ueFE.Department = "TEST_VRENT";
            ueFE.Name = "ABCDE";
            ueFE.VName = "ABCDE";
            ueFE.Phone = "ABCDE";
            ueFE.Mail = "rent1@rent1.com";

            ueFE.PersonInCharge = "ABCDE";
            ueFE.PrivateMobileNumber = "ABCDE";
            ueFE.PrivateBankAccount = "ABCDE";

            ueFE.PrivateEmail = "6@6.com";
            ueFE.PrivateAddress = "ABCDE";
            ueFE.BusinessAddress = "ABCDE";//not changable in kemas
            ueFE.Valid_to = "2020-08-31";

            ueFE.BirthDay = DateTime.Now.ToString();
            ueFE.Gender = 1;
            ueFE.Postcode = "ABCDE";
            ueFE.City = "ABCDE";
            ueFE.Street = "ABCDE";
            ueFE.Province = "ABCDE";
            //ueFE.ClientID = "05803200-c269-4ca5-8640-ce11340b4271";

            UserLicenseExtension ule = new UserLicenseExtension();
            ule.ExpireDate = DateTime.Now.ToString();
            ule.LicenseNumber = "1234567890";
            ule.DateOfIssue = DateTime.Now.ToString();
            ule.PIN = 123456;
            ule.PIN2 = 123456;

            ueFE.ProxyLicense = ule;
            //UpdateUserToInitialState(ueFE,1,auth);

            ProxyUserSetting pus = new ProxyUserSetting();
            pus.ID = auth.ID;
            pus.SessionID = auth.SessionID;

            AppRegistrationBLL burb = new AppRegistrationBLL(pus);

            UserExtension created = burb.UpdateProfile(ueFE, "english");

            string response = SerializedHelper.JsonSerialize<UserExtension>(created);

            Assert.IsNotNull(created != null && created.StatusEntities[1].Value == 1 && response != null, "the use should flaged to init Reg");
        }

        [TestMethod]
        public void SCRejectUser()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            //WS_Auth_Response adminAuth = kemasAuth.authByLogin("service.center@abc.com", "123456");

            WS_Auth_Response userAuth = kemasAuth.authByLogin("rent1@rent1.com", "123456");

            //UserData2 adminUser = UserRegistrationConst.RetrieveKemasUserByID(adminAuth.ID,adminAuth.SessionID);
            UserData2 user = UserRegistrationConst.RetrieveKemasUserByID(userAuth.ID, userAuth.SessionID);

            //UserStatusManager usm = new UserStatusManager(user.Status);

            //usm.Status["F"].Value = 1;
            //usm.Status["2"].Value = 1;
            //usm.Status["3"].Value = 0;
            //usm.Status["7"].Value = 1;

            //Corporate client: NMS:e1c286c4-ae86-4c7d-810f-1b6357892f9f
            //user.Clients = new string[] { Guid.Parse("e1c286c4-ae86-4c7d-810f-1b6357892f9f").ToString() };
           

            updateUserData uud = UserRegistrationConst.ConvertUserData2ToUpdateUserData(user);


            //uud.Status = usm.Status.BinaryPattern;

            UserData2 ud2 = UserRegistrationConst.UpdateKemasUser(uud,userAuth.SessionID,"english");

            Assert.IsTrue(ud2 != null ,"should be rejected");
        }


        [TestMethod]
        public void ProxyLicenseUnitTest()
        {
            string datafile = Path.Combine(@"C:\CF-repo\vrent450-adv\ProxyTest\TestData\Users", "SampleLicense.txt");
            string data = ProxyTest.DatReader.Read(datafile);

            UserLicenseExtension ule = SerializedHelper.JsonDeserialize<UserLicenseExtension>(data);

        }

        [TestMethod]
        public void UserInfoUnitTest()
        {
            string datafile = Path.Combine(@"C:\CF-repo\vrent450-adv\ProxyTest\TestData\Users", "UpdateUser.txt");
            string data = ProxyTest.DatReader.Read(datafile);

            UserExtension ule = SerializedHelper.JsonDeserialize<UserExtension>(data);

        }

        [TestMethod]
        public void AssignClientsUnitTest()
        {
        //            [0]	"f5b84c82-8ea6-4a67-8088-2f91907379b6"	string
        //[1]	"0f41cfb6-ed85-480f-863f-f8d741f59aad"	string
        //[2]	"05803200-c269-4ca5-8640-ce11340b4271"	string
        //[3]	"e1c286c4-ae86-4c7d-810f-1b6357892f9f"	string
        //[4]	"d6b3b374-133a-4ed8-be12-309436367926"	string
        //[5]	"0dcc7ea1-9e31-45c4-887e-d8efb485bf13"	string


            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("service.center@abc.com", "123456");

            KemasUserAPI kemasUser = new KemasUserAPI();
            findUser2Response user2Res = kemasUser.findUser2(auth.ID, auth.SessionID);

            updateUserData uud = UserRegistrationConst.ConvertUserData2ToUpdateUserData(user2Res.UserData);

            WS_Auth_Response adminAuth = AuthencationUnitTestKemasAPI.SignOn();
            KemasConfigsAPI config = new KemasConfigsAPI();
            getClientsResponse configRes = config.getClients(adminAuth.SessionID);

            string clientID = configRes.Clients[1].ID;
            string clientName = configRes.Clients[1].Name;

            string[] clients = configRes.Clients.Select(m => m.ID).ToArray();

            uud.Clients = new string[] { clientID };
            uud.Company = clientName;

            //"Required fields: Surname, Given Name, Department, Phone number, Mobile phone number"

            uud.Name = "Mike";
            uud.VName = "Tom";
            uud.Department = "VW_Test";
            uud.Phone = "1234567890";
            uud.PrivateMobileNumber = "1234567890";

            uud.License.RFID = "gtb7";

            UserData2 updatedUser = UserRegistrationConst.UpdateKemasUser(uud, auth.SessionID, "english");

            Assert.IsTrue(updatedUser != null && updatedUser.Clients[0].ID == clientID && updatedUser.Clients[0].Name == updatedUser.Company, "should be the same");
            
        }

        #region Check change PWD
        [TestMethod]
        public void ChangeAccount()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("service.center@abc.com", "123456");

            KemasUserAPI kemasUser = new KemasUserAPI();
            findUser2Response user2Res = kemasUser.findUser2(auth.ID, auth.SessionID);

            updateUserData uud = UserRegistrationConst.ConvertUserData2ToUpdateUserData(user2Res.UserData);


            UserExtension ue = new UserExtension();
            ue.Mail = "";
            ue.Name = "BUG";
            ue.VName = "BUG";
            ue.ClientID = "";

            UserSettingBLL usb = new UserSettingBLL();
            UserExtension vmUE = usb.Login("vrent.mgr@abc.com", "123456");



            UserManagementBLL umb = new UserManagementBLL();
        }


        [TestMethod]
        public void CreateCorporateAccount() 
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("vrent.mgr@abc.com", "123456");

            KemasUserAPI kemasUser = new KemasUserAPI();
            findUser2Response user2Res = kemasUser.findUser2(auth.ID, auth.SessionID);

            ProxyUserSetting userinfo = new ProxyUserSetting();
            userinfo.ID = auth.ID;
            userinfo.SessionID = auth.SessionID;

            updateUserData uud = UserRegistrationConst.ConvertUserData2ToUpdateUserData(user2Res.UserData);

            UserExtension ue = new UserExtension();
            ue.Mail = "corpUser@corpUSer.com";
            ue.Name = "PWD";
            ue.VName = "PWD";
            ue.ClientID = uud.Clients[0];

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = auth.SessionID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "VRent Manager"
                        }
                }
            };

            UserExtension created = bll.CreateCorpUser(ue);

            Assert.IsTrue(created != null, "should create a new corporate user");
        }
        #endregion


        [TestMethod]
        public void ConvertFromRoleEntitiesToVrentrolesUnitTest()
        {
            KemasAuthencationAPI kemasAuth = new KemasAuthencationAPI();
            WS_Auth_Response auth = kemasAuth.authByLogin("service.center@abc.com", "123456");

            KemasUserAPI kemasUser = new KemasUserAPI();
            findUser2Response user2Res = kemasUser.findUser2(auth.ID, auth.SessionID);

            UserExtension complexRole = UserRegistrationConst.AssembleUserExtention(user2Res.UserData, auth);

            ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(complexRole);



            Assert.IsTrue(complexRole != null, "convert to vrentroles");
        }
    }
}
