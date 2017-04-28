using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.BLL;
using CF.VRent.Entities;
using System.Collections.Generic;

namespace ProxyTest
{
    [TestClass]
    public class UserTransferUnitTest
    {
//+		[0]	{[f5b84c82-8ea6-4a67-8088-2f91907379b6, Mcon]}	System.Collections.Generic.KeyValuePair<string,string>
//+		[1]	{[05803200-c269-4ca5-8640-ce11340b4271, Testclient]}	System.Collections.Generic.KeyValuePair<string,string>
//+		[2]	{[0f41cfb6-ed85-480f-863f-f8d741f59aad, C04]}	System.Collections.Generic.KeyValuePair<string,string>
//+		[3]	{[0dcc7ea1-9e31-45c4-887e-d8efb485bf13, C02]}	System.Collections.Generic.KeyValuePair<string,string>
//+		[4]	{[d6b3b374-133a-4ed8-be12-309436367926, C01]}	System.Collections.Generic.KeyValuePair<string,string>
//+		[5]	{[e1c286c4-ae86-4c7d-810f-1b6357892f9f, NMS]}	System.Collections.Generic.KeyValuePair<string,string>
        public Guid targetClientID = Guid.Parse("e1c286c4-ae86-4c7d-810f-1b6357892f9f"); //NMS
        public Guid endUserClient = Guid.Parse("f5b84c82-8ea6-4a67-8088-2f91907379b6"); //Mcon
        public Guid userID = Guid.Parse("9A5DF047-86D6-4998-9392-0ABE052C4640");
        public Guid VMID = Guid.Parse("9c4dafea-2854-4847-9bfa-69fc0ab6f078");

        [TestMethod]
        public void AddUserTransferTestMethod()
        {
            ProxyUserSetting userinfo = new ProxyUserSetting();

            userinfo.ID = userID.ToString();
            userinfo.Mail = "Daniel.Li@crm-factory.com.cn";
            userinfo.ClientID = endUserClient.ToString();

            UserTransferBLL utb = new UserTransferBLL(userinfo);
            UserTransferRequest urtReq = utb.RequestUserTransfer(targetClientID.ToString());


            Assert.IsTrue(urtReq != null, "add a user transfer request");
        }

        [TestMethod]
        public void ApproveOrRejectUserTransferTestMethod()
        {
            ProxyUserSetting userInfo = new ProxyUserSetting();
            userInfo.ID = VMID.ToString();
            userInfo.ClientID = targetClientID.ToString();
            List<ProxyRole> roles = new List<ProxyRole>();
            ProxyRole vmrole = new ProxyRole() { RoleMember = "VRent Manager" };
            roles.Add(vmrole);
            userInfo.AllRoles = roles;


            UserTransferBLL utb = new UserTransferBLL(userInfo);
            UserTransferRequest transferRet = utb.DetermineUserTransfer(userID.ToString(), false);

            Assert.IsTrue(transferRet != null, "add a user transfer request");
        }

        [TestMethod]
        public void RetrieveUserTransferByVMTestMethod()
        {
            ProxyUserSetting userInfo = new ProxyUserSetting();
            userInfo.ID = VMID.ToString();
            userInfo.ClientID = targetClientID.ToString();
            List<ProxyRole> roles = new List<ProxyRole>();
            ProxyRole vmrole = new ProxyRole() { RoleMember = "VRent Manager" };
            roles.Add(vmrole);
            userInfo.AllRoles = roles;

            UserTransferBLL utb = new UserTransferBLL(userInfo);
            UserTransferRequest[] requests = utb.LoadUserTransfer();

            Assert.IsTrue(requests != null, "add a user transfer request");
        }

        [TestMethod]
        public void RetrieveUserTransferByUSerTestMethod()
        {
            ProxyUserSetting userInfo = new ProxyUserSetting();
            userInfo.ID = userID.ToString();
            userInfo.ClientID = endUserClient.ToString();
            List<ProxyRole> roles = new List<ProxyRole>();
            ProxyRole vmrole = new ProxyRole() { RoleMember = "Employee" };
            roles.Add(vmrole);
            userInfo.AllRoles = roles;

            UserTransferBLL utb = new UserTransferBLL(userInfo);
            UserTransferRequest[] requests = utb.LoadUserTransfer();

            Assert.IsTrue(requests != null, "add a user transfer request");
        }



        [TestMethod]
        public void PendingUserTransferByUserTestMethod()
        {
            ProxyUserSetting userInfo = new ProxyUserSetting();
            userInfo.ID = userID.ToString();
            userInfo.ClientID = endUserClient.ToString();
            List<ProxyRole> roles = new List<ProxyRole>();
            ProxyRole vmrole = new ProxyRole() { RoleMember = "Employee" };
            roles.Add(vmrole);
            userInfo.AllRoles = roles;

            UserTransferBLL utb = new UserTransferBLL(userInfo);
            UserTransferRequest updateRet = utb.LoadPendingUserTransferRequest(userID.ToString());

            Assert.IsTrue(updateRet != null, "add a user transfer request");
        }

                [TestMethod]
        public void PendingUserTransferByVMTestMethod()
        {
            ProxyUserSetting userInfo = new ProxyUserSetting();
            userInfo.ID = VMID.ToString();
            userInfo.ClientID = targetClientID.ToString();
            List<ProxyRole> roles = new List<ProxyRole>();
            ProxyRole vmrole = new ProxyRole() { RoleMember = "VRent Manager" };
            roles.Add(vmrole);
            userInfo.AllRoles = roles;

            UserTransferBLL utb = new UserTransferBLL(userInfo);
            UserTransferRequest updateRet = utb.LoadPendingUserTransferRequest(userID.ToString());

            Assert.IsTrue(updateRet != null, "add a user transfer request");
        }

    }
}
