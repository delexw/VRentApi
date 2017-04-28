using CF.VRent.BLL;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using UnionPayTest.TestHeaders;

namespace UnionPayTest.UserMgmtTest
{
    [TestClass]
    public class UserMgmtBLL : TestHeader
    {
        [TestInitialize()]
        public void MyClassInitialize()
        {
            HttpContext.Current = new HttpContext(new SimpleWorkerRequest("", "", "", "", new StringWriter()));
        }

        [TestMethod]
        public void GetUserDetail_UserCF()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting() { SessionID = user.SessionID, 
                AllRoles = new ProxyRole[] { new ProxyRole() { RoleMember = "Service Center" } } };

            var userD = bll.GetUserDetail("1c9d9c82-d074-45a4-863e-e7eeb2384c64");

            var s = SerializedHelper.JsonSerialize<UserExtension>(userD);

            base.OutputMessage(userD);
            base.OutputMessage(userD.StatusEntities);
            base.OutputMessage(userD.StatusExtensionEntities);
            base.OutputMessage(userD.RoleEntities);
            Assert.IsNotNull(userD); 
        }

        [TestMethod]
        public void GetAllCompaniesSC()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);

            ICompany bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<ICompany>();
            bll.UserInfo = new ProxyUserSetting() { SessionID = user.SessionID,
                AllRoles = new ProxyRole[]{new ProxyRole(){ RoleMember = "Service Center"}} };

            var c = bll.GetAllCompanies();

            base.OutputMessage(c);

            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void GetSelfCompaniesVM()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);

            ICompany bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<ICompany>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                AllRoles = new ProxyRole[] { new ProxyRole() { RoleMember = "VRent Manager" } }
            };

            var c = bll.GetAllCompanies();

            base.OutputMessage(c);

            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void GetCompanyPendingUser()
        {
            var user = Login.LoginKemas("vrent.mgr@abc.com", TestHeader.LoginParameters.UserPwd);
            var userInfo = new KemasUserAPIProxy().findUser2(user.ID,user.SessionID);

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();

            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                ClientID = userInfo.UserData.Clients[0].ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { RoleMember = "VRent Manager" } }
            };

            var c = bll.GetCompanyUserList(new UserExtension() { Status = "6" });

            base.OutputMessage(c);

            Assert.IsNotNull(c);
        }

        #region Create
        [TestMethod]
        public void CreateCorpUserSCWithCorporateCompany()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);

            var newUser = new CF.VRent.Entities.UserExtension()
            {
                Mail = UserUtility.RadomPassword(6) + "@vrenttest.com",
                Name = UserUtility.RadomPassword(5),
                VName = UserUtility.RadomPassword(5),
                ClientID = "d6b3b374-133a-4ed8-be12-309436367926"
            };

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Service Center"
                        }
                }
            };

            var c = bll.CreateCorpUser(newUser);

            Assert.IsNotNull(c.ID);
            Assert.AreEqual(c.Status.ToStr().Substring(0, 8), "10000600");
            base.OutputMessage(c);
            base.OutputMessage(c.StatusEntities.BinaryPattern);
            base.OutputMessage(c.RoleEntities);
        }

        [TestMethod]
        public void CreateCorpUserSCWithEndUserCompany()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);

            var newUser = new CF.VRent.Entities.UserExtension()
            {
                Mail = UserUtility.RadomPassword(6) + "@vrenttest.com",
                Name = UserUtility.RadomPassword(5),
                VName = UserUtility.RadomPassword(5),
                ClientID = "f5b84c82-8ea6-4a67-8088-2f91907379b6"
            };

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Service Center"
                        }
                }
            };

            var c = bll.CreateCorpUser(newUser);

            Assert.IsNotNull(c.ID);
            Assert.AreEqual(c.Status.ToStr().Substring(0, 8), "00000000");
            base.OutputMessage(c);
            base.OutputMessage(c.StatusEntities.BinaryPattern);
            base.OutputMessage(c.RoleEntities);
        }

        [TestMethod]
        public void CreateCorpUserVMWithCorporateCompany()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);

            var newUser = new CF.VRent.Entities.UserExtension()
            {
                Mail = UserUtility.RadomPassword(6) + "@vrenttest.com",
                Name = UserUtility.RadomPassword(3),
                VName = UserUtility.RadomPassword(3),
                ClientID = "d6b3b374-133a-4ed8-be12-309436367926"
            };

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                AllRoles = new ProxyRole[] { new ProxyRole(){
                RoleMember = "VRent Manager"
            } }
            };

            var c = bll.CreateCorpUser(newUser);

            Assert.IsNotNull(c.ID);
            Assert.AreEqual(c.Status.ToStr().Substring(0, 8), "10000070");
            base.OutputMessage(c);
            base.OutputMessage(c.StatusEntities.BinaryPattern);
            base.OutputMessage(c.RoleEntities);
        }

        [TestMethod]
        public void CreateCorpUserVMWithEndUserCompany()
        {
            var user = Login.LoginKemas(TestHeader.LoginParameters.UserName, TestHeader.LoginParameters.UserPwd);

            var newUser = new CF.VRent.Entities.UserExtension()
            {
                Mail = UserUtility.RadomPassword(6) + "@vrenttest.com",
                Name = UserUtility.RadomPassword(3),
                VName = UserUtility.RadomPassword(3),
                ClientID = "f5b84c82-8ea6-4a67-8088-2f91907379b6"
            };

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                AllRoles = new ProxyRole[] { new ProxyRole(){
                RoleMember = "VRent Manager"
            } }
            };

            var c = bll.CreateCorpUser(newUser);

            Assert.IsNotNull(c.ID);
            Assert.AreEqual(c.Status.ToStr().Substring(0, 8), "00000000");
            base.OutputMessage(c);
            base.OutputMessage(c.StatusEntities.BinaryPattern);
            base.OutputMessage(c.RoleEntities);
        } 
        #endregion
        
        [TestMethod]
        public void GenerateRadomPwd()
        {
            var s = UserUtility.RadomPassword(6);
            base.TestContext.WriteLine(s);
            Assert.AreEqual(s.Length, 6);
        }


        [TestMethod]
        public void CreateCorpUserSC()
        {
            var user = Login.LoginKemas("service.center@abc.com", TestHeader.LoginParameters.UserPwd);

            var newUser = new CF.VRent.Entities.UserExtension()
            {
                Mail = UserUtility.RadomPassword(6) + "@vrenttest.com",
                Name = UserUtility.RadomPassword(5),
                VName = UserUtility.RadomPassword(5),
                ClientID = "e1c286c4-ae86-4c7d-810f-1b6357892f9f",
                Gender = 1,
                TypeOfJourney = 1
            };

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Customer Service Agent"
                        }
                }
            };

            var c = bll.CreateCorpUser(newUser);

            Assert.IsNotNull(c.ID);
            Assert.AreEqual(c.Status.ToStr().Substring(0, 8), "10000600");
            base.OutputMessage(c);
            base.OutputMessage(c.StatusEntities.BinaryPattern);
            base.OutputMessage(c.RoleEntities);
            Thread.Sleep(600000);
        }

        [TestMethod]
        public void CreateCorpUserADMIN()
        {
            var user = Login.LoginKemas("operation.assist@abc.com", TestHeader.LoginParameters.UserPwd);

            var newUser = new CF.VRent.Entities.UserExtension()
            {
                Mail = UserUtility.RadomPassword(6) + "@vrenttest.com",
                Name = UserUtility.RadomPassword(5),
                VName = UserUtility.RadomPassword(5),
                ClientID = "f5b84c82-8ea6-4a67-8088-2f91907379b6",
                Gender = 1,
                TypeOfJourney = 1
            };

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Administration"
                        }
                }
            };

            var c = bll.CreateCorpUser(newUser);

            Assert.IsNotNull(c.ID);
            Assert.AreEqual(c.Status.ToStr().Substring(0, 8), "10000600");
            base.OutputMessage(c);
            base.OutputMessage(c.StatusEntities.BinaryPattern);
            base.OutputMessage(c.RoleEntities);
            Thread.Sleep(5000);
        }

        [TestMethod]
        public void CreateCorpUserVM()
        {
            var user = Login.LoginKemas("vrent.mgr@abc.com", TestHeader.LoginParameters.UserPwd);

            var newUser = new CF.VRent.Entities.UserExtension()
            {
                Mail = UserUtility.RadomPassword(6) + "@vrenttest.com",
                Name = UserUtility.RadomPassword(3),
                VName = UserUtility.RadomPassword(3),
                ClientID = "e1c286c4-ae86-4c7d-810f-1b6357892f9f"
            };

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                AllRoles = new ProxyRole[] { new ProxyRole(){
                RoleMember = "VRent Manager"
            } }
            };

            var c = bll.CreateCorpUser(newUser);

            Assert.IsNotNull(c.ID);
            Assert.AreEqual(c.Status.ToStr().Substring(0, 8), "10000070");
            base.OutputMessage(c);
            base.OutputMessage(c.StatusEntities.BinaryPattern);
            base.OutputMessage(c.RoleEntities);
            Thread.Sleep(5000);
        }

        [TestMethod]
        public void UpdateUserSCApprove_EndUser2CorporateUser_UserStatus0030000()
        {
            var user = Login.LoginKemas("service.center@abc.com", TestHeader.LoginParameters.UserPwd);

            var user2 = Login.LoginKemas("adam", "123456");

            KemasUserAPI kapi = new KemasUserAPI();

            var userInfo = kapi.findUser2(user.ID, user.SessionID);

            //Init data
            var adam = kapi.updateUser2(new CF.VRent.Entities.KEMASWSIF_USERRef.updateUser2Request()
            {
                SessionID = user.SessionID,
                Language = "english",
                UserData = new CF.VRent.Entities.KEMASWSIF_USERRef.updateUserData()
                {
                    ID = user2.ID,
                    Status = "0030000",
                    Enabled = 1,
                    Clients = new string[] { "f5b84c82-8ea6-4a67-8088-2f91907379b6" }
                }
            });

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                ClientID = userInfo.UserData.Clients[0].ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Customer Service Agent"
                        }
                }
            };

            var upUser = bll.UpdateUser(new UserExtension()
            {
                ID = user2.ID,
                Name = "123",
                VName = "123",
                Phone = "1234566",
                PrivateMobileNumber = "4567890",
                Company = "gdsffdg",
                Department = "12312",
                PersonInCharge = "112121212",
                Status = "D",
                ProxyLicense = new UserLicenseExtension() {
                    RFID = "0000002300010000002A"
                },
                Gender = 0,
                Description = "11212121212",
                TypeOfJourney = 1,
                ClientID = "e1c286c4-ae86-4c7d-810f-1b6357892f9f"
            });


            Assert.IsNotNull(upUser);
            Assert.AreEqual(upUser.Status.Trim().Substring(0, 8), "00000600");
            base.OutputMessage(upUser);
            Thread.Sleep(5000);
           // Assert.AreEqual(upUser.StatusEntities.Where(r=>r.Flag==""))
        }

        [TestMethod]
        public void UpdateUserSCApprove_CorporateUser2CorporateUser_UserStatus0030000()
        {
            var user = Login.LoginKemas("service.center@abc.com", TestHeader.LoginParameters.UserPwd);

            var user2 = Login.LoginKemas("adam", "123456");

            KemasUserAPI kapi = new KemasUserAPI();

            var userInfo = kapi.findUser2(user.ID, user.SessionID);

            //Init data
            var adam = kapi.updateUser2(new CF.VRent.Entities.KEMASWSIF_USERRef.updateUser2Request()
            {
                SessionID = user.SessionID,
                Language = "english",
                UserData = new CF.VRent.Entities.KEMASWSIF_USERRef.updateUserData()
                {
                    ID = user2.ID,
                    Status = "0030000",
                    Enabled = 1,
                    Clients = new string[] { "d6b3b374-133a-4ed8-be12-309436367926" }
                }
            });

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                ClientID = userInfo.UserData.Clients[0].ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Service Center"
                        }
                }
            };

            var upUser = bll.UpdateUser(new UserExtension()
            {
                ID = user2.ID,
                Name = "123",
                VName = "123",
                Phone = "1234566",
                PrivateMobileNumber = "4567890",
                Company = "gdsffdg",
                Department = "12312",
                PersonInCharge = "112121212",
                Status = "4",
                ClientID = "0dcc7ea1-9e31-45c4-887e-d8efb485bf13",
                TypeOfJourney = 1
            });


            Assert.IsNotNull(upUser);
            Assert.AreEqual(upUser.Status.Trim().Substring(0, 8), "00040600");
            base.OutputMessage(upUser);
            Thread.Sleep(5000);
            // Assert.AreEqual(upUser.StatusEntities.Where(r=>r.Flag==""))
        }


        [TestMethod]
        public void UpdateUserSCApprove_CorporateUser2EndUser_UserStatus0030000()
        {
            var user = Login.LoginKemas("service.center@abc.com", TestHeader.LoginParameters.UserPwd);

            var user2 = Login.LoginKemas("adam", "123456");

            KemasUserAPI kapi = new KemasUserAPI();

            var userInfo = kapi.findUser2(user.ID, user.SessionID);

            //Init data
            var adam = kapi.updateUser2(new CF.VRent.Entities.KEMASWSIF_USERRef.updateUser2Request()
            {
                SessionID = user.SessionID,
                Language = "english",
                UserData = new CF.VRent.Entities.KEMASWSIF_USERRef.updateUserData()
                {
                    ID = user2.ID,
                    Status = "0030000",
                    Enabled = 1,
                    Clients = new string[] { "e1c286c4-ae86-4c7d-810f-1b6357892f9f" }
                }
            });

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                ClientID = userInfo.UserData.Clients[0].ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Customer Service Agent"
                        }
                }
            };

            var upUser = bll.UpdateUser(new UserExtension()
            {
                ID = user2.ID,
                Name = "123",
                VName = "123",
                Phone = "1234566",
                PrivateMobileNumber = "4567890",
                Company = "gdsffdg",
                Department = "12312",
                PersonInCharge = "112121212",
                Status = "4",
                ClientID = "f5b84c82-8ea6-4a67-8088-2f91907379b6"
            });


            Assert.IsNotNull(upUser);
            Assert.AreEqual(upUser.Status.Trim().Substring(0, 8), "00040000");
            base.OutputMessage(upUser);
            Thread.Sleep(5000);
            // Assert.AreEqual(upUser.StatusEntities.Where(r=>r.Flag==""))
        }

        [TestMethod]
        public void UpdateUserSCReject_EndUser2CorporateUser_UserStatus0030000()
        {
            var user = Login.LoginKemas("service.center@abc.com", TestHeader.LoginParameters.UserPwd);

            var user2 = Login.LoginKemas("adam", "123456");

            KemasUserAPI kapi = new KemasUserAPI();

            var userInfo = kapi.findUser2(user.ID, user.SessionID);

            //Init data
            var adam = kapi.updateUser2(new CF.VRent.Entities.KEMASWSIF_USERRef.updateUser2Request()
            {
                SessionID = user.SessionID,
                Language = "english",
                UserData = new CF.VRent.Entities.KEMASWSIF_USERRef.updateUserData()
                {
                    ID = user2.ID,
                    Status = "0030000",
                    Enabled = 1,
                    Clients = new string[] { "f5b84c82-8ea6-4a67-8088-2f91907379b6" }
                }
            });

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                ClientID = userInfo.UserData.Clients[0].ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Customer Service Agent"
                        }
                }
            };

            var upUser = bll.UpdateUser(new UserExtension()
            {
                ID = user2.ID,
                Name = "123",
                VName = "123",
                Phone = "1234566",
                PrivateMobileNumber = "4567890",
                Company = "gdsffdg",
                Department = "12312",
                PersonInCharge = "112121212",
                Status = "5",
                Gender = 1,
                Description = "11212121212",
                TypeOfJourney = 1,
                ClientID = "e1c286c4-ae86-4c7d-810f-1b6357892f9f"
            });


            Assert.IsNotNull(upUser);
            Assert.AreEqual(upUser.Status.Trim().Substring(0, 8), "02005600");
            base.OutputMessage(upUser);
            Thread.Sleep(5000);
            // Assert.AreEqual(upUser.StatusEntities.Where(r=>r.Flag==""))
        }

        [TestMethod]
        public void UpdateUserSCReject_CorporateUser2CorporateUser_UserStatus0030000()
        {
            var user = Login.LoginKemas("service.center@abc.com", TestHeader.LoginParameters.UserPwd);

            var user2 = Login.LoginKemas("adam", "123456");

            KemasUserAPI kapi = new KemasUserAPI();

            var userInfo = kapi.findUser2(user.ID, user.SessionID);

            //Init data
            var adam = kapi.updateUser2(new CF.VRent.Entities.KEMASWSIF_USERRef.updateUser2Request()
            {
                SessionID = user.SessionID,
                Language = "english",
                UserData = new CF.VRent.Entities.KEMASWSIF_USERRef.updateUserData()
                {
                    ID = user2.ID,
                    Status = "0030000",
                    Enabled = 1,
                    Clients = new string[] { "e1c286c4-ae86-4c7d-810f-1b6357892f9f" }
                }
            });

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                ClientID = userInfo.UserData.Clients[0].ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Customer Service Agent"
                        }
                }
            };

            var upUser = bll.UpdateUser(new UserExtension()
            {
                ID = user2.ID,
                Name = "123",
                VName = "123",
                Phone = "1234566",
                PrivateMobileNumber = "4567890",
                Company = "gdsffdg",
                Department = "12312",
                PersonInCharge = "112121212",
                Status = "5",
                ClientID = "0dcc7ea1-9e31-45c4-887e-d8efb485bf13"
            });


            Assert.IsNotNull(upUser);
            Assert.AreEqual(upUser.Status.Trim().Substring(0, 8), "02005600");
            base.OutputMessage(upUser);
            Thread.Sleep(5000);
        }

        [TestMethod]
        public void UpdateUserSCReject_CorporateUser2EndUser_UserStatus0030000()
        {
            var user = Login.LoginKemas("service.center@abc.com", TestHeader.LoginParameters.UserPwd);

            var user2 = Login.LoginKemas("adam", "123456");

            KemasUserAPI kapi = new KemasUserAPI();

            var userInfo = kapi.findUser2(user.ID, user.SessionID);

            //Init data
            var adam = kapi.updateUser2(new CF.VRent.Entities.KEMASWSIF_USERRef.updateUser2Request()
            {
                SessionID = user.SessionID,
                Language = "english",
                UserData = new CF.VRent.Entities.KEMASWSIF_USERRef.updateUserData()
                {
                    ID = user2.ID,
                    Status = "0030000",
                    Enabled = 1,
                    Clients = new string[] { "e1c286c4-ae86-4c7d-810f-1b6357892f9f" }
                }
            });

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                ClientID = userInfo.UserData.Clients[0].ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "Customer Service Agent"
                        }
                }
            };

            var upUser = bll.UpdateUser(new UserExtension()
            {
                ID = user2.ID,
                Name = "123",
                VName = "123",
                Phone = "1234566",
                PrivateMobileNumber = "4567890",
                Company = "gdsffdg",
                Department = "12312",
                PersonInCharge = "112121212",
                Status = "5",
                ClientID = "f5b84c82-8ea6-4a67-8088-2f91907379b6"
            });


            Assert.IsNotNull(upUser);
            Assert.AreEqual(upUser.Status.Trim().Substring(0, 8), "02005000");
            base.OutputMessage(upUser);
            Thread.Sleep(5000);
        }

        [TestMethod]
        public void UpdateUserVMApprove_CorporateUser_UserStatus0030000()
        {
            var user = Login.LoginKemas("vrent.mgr@abc.com", TestHeader.LoginParameters.UserPwd);

            var user2 = Login.LoginKemas("adam", "123456");

            KemasUserAPI kapi = new KemasUserAPI();

            var userInfo = kapi.findUser2(user.ID, user.SessionID);

            kapi.updateUser2(new CF.VRent.Entities.KEMASWSIF_USERRef.updateUser2Request()
            {
                SessionID = user.SessionID,
                Language = "english",
                UserData = new CF.VRent.Entities.KEMASWSIF_USERRef.updateUserData()
                {
                    ID = user2.ID,
                    Status = "0030000",
                    Enabled = 1,
                    Clients = new string[] { "f5b84c82-8ea6-4a67-8088-2f91907379b6" }
                }
            });

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                ClientID = userInfo.UserData.Clients[0].ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "VRent Manager"
                        }
                }
            };

            var upUser = bll.UpdateUser(new UserExtension()
            {
                ID = user2.ID,
                //Name = "123",
                //VName = "123",
                //Phone = "1234566",
                //PrivateMobileNumber = "4567890",
                //Company = "gdsffdg",
                //Department = "12312",
                //PersonInCharge = "112121212",
                Status = "7",
                //Mail = "1111@1111.com",
                TypeOfJourney = 1
            });


            Assert.IsNotNull(upUser);
            Assert.AreEqual(upUser.Status.Trim().Substring(0,8), "00300070");
            base.OutputMessage(upUser);
            Thread.Sleep(5000);
            // Assert.AreEqual(upUser.StatusEntities.Where(r=>r.Flag==""))
        }

        [TestMethod]
        public void UpdateUserVMReject_CorporateUser_UserStatus0030000()
        {
            var user = Login.LoginKemas("vrent.mgr@abc.com", TestHeader.LoginParameters.UserPwd);

            var user2 = Login.LoginKemas("adam", "123456");

            KemasUserAPI kapi = new KemasUserAPI();

            var userInfo = kapi.findUser2(user.ID, user.SessionID);

            kapi.updateUser2(new CF.VRent.Entities.KEMASWSIF_USERRef.updateUser2Request()
            {
                SessionID = user.SessionID,
                Language = "english",
                UserData = new CF.VRent.Entities.KEMASWSIF_USERRef.updateUserData()
                {
                    ID = user2.ID,
                    Status = "0030000",
                    Enabled = 1,
                    Clients = new string[] { "f5b84c82-8ea6-4a67-8088-2f91907379b6" }
                }
            });

            IUserMgmt bll = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserMgmt>();
            bll.UserInfo = new ProxyUserSetting()
            {
                SessionID = user.SessionID,
                ID = user.ID,
                ClientID = userInfo.UserData.Clients[0].ID,
                AllRoles = new ProxyRole[] { new ProxyRole() { 
                RoleMember = "VRent Manager"
                        }
                }
            };

            var upUser = bll.UpdateUser(new UserExtension()
            {
                ID = user2.ID,
                Name = "123",
                VName = "123",
                Phone = "1234566",
                PrivateMobileNumber = "4567890",
                Company = "gdsffdg",
                Department = "12312",
                PersonInCharge = "112121212",
                Status = "8",
                Mail = "1111@1111.com",
                TypeOfJourney = 1
            });

            Assert.IsNotNull(upUser);
            Assert.AreEqual(upUser.Status.Trim().Substring(0, 8), "00300008");
            base.OutputMessage(upUser);
            Thread.Sleep(5000);
            // Assert.AreEqual(upUser.StatusEntities.Where(r=>r.Flag==""))
        }
    }
}
