using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Common.UserContracts;
using System.Collections.Generic;

namespace CF.VRent.DataAccessProxyTest
{
    [TestClass]
    public class UserTransferUnitTest
    {
        [TestMethod]
        public void AddUserTransferTestMethod()
        {
            UserTransferRequest utr = new UserTransferRequest();

            utr.ClientIDFrom = Guid.NewGuid();
            utr.ClientIDTo = Guid.NewGuid();
            utr.UserID = Guid.NewGuid();
            utr.Mail = "rent@rent.com";
            utr.State = UserTransferState.Active;
            utr.TransferResult = UserTransferResult.Pending;
            utr.CreatedBy = Guid.NewGuid();
            utr.CreatedOn = DateTime.Now;

            ProxyUserSetting userInfo = new ProxyUserSetting();

            UserTransferCUDResult addRet = UserTransferDAL.AddTransferRequest(utr, userInfo);

            Assert.IsTrue (addRet != null, "add a user transfer request");
        }

        [TestMethod]
        public void UpdateUserTransferTestMethod()
        {
            UserTransferRequest utr = new UserTransferRequest();

            utr.UserID = Guid.Parse("EFF8D13C-FFFC-4AB1-9E2B-5783D971D753");
            utr.Mail = "rent@rent.com";

            utr.State = UserTransferState.Completed;
            utr.ApproverID = Guid.NewGuid();
            utr.TransferResult = UserTransferResult.Approve;
            utr.ModifiedBy = utr.ApproverID;
            utr.ModifiedOn = DateTime.Now;

            ProxyUserSetting userInfo = new ProxyUserSetting();
            userInfo.ID = Guid.NewGuid().ToString();
            userInfo.ClientID = Guid.Parse("BD48A7C1-1C47-4209-9DAA-4C5FCF03D298").ToString();

            UserTransferCUDResult updateRet = UserTransferDAL.UpdateTransferRequest(utr, userInfo);

            Assert.IsTrue(updateRet != null, "add a user transfer request");
        }

        [TestMethod]
        public void RetrieveUserTransferTestMethod()
        {
            Guid targetClient = Guid.Parse("85AE2A63-0816-459A-98EF-7E8D3ED1C009");

            ProxyUserSetting userInfo = new ProxyUserSetting();
            userInfo.ID = Guid.NewGuid().ToString();
            userInfo.ClientID = targetClient.ToString();
            List<ProxyRole> roles = new List<ProxyRole>();
            ProxyRole vmrole = new ProxyRole() { RoleMember = "VRent Manager" };
            roles.Add(vmrole);
            userInfo.AllRoles = roles;

            UserTransferRResult updateRet = UserTransferDAL.RetrieveTransferRequests(userInfo);

            Assert.IsTrue(updateRet != null, "add a user transfer request");
        }

        [TestMethod]
        public void RetrievePendingUserTransferTestMethod()
        {
            Guid userID = Guid.Parse("D5344C20-FC63-452A-89DF-A92A59A85D70");

            UserTransferRResult updateRet = UserTransferDAL.RetrievePendingTransferRequests(userID);

            Assert.IsTrue(updateRet != null, "add a user transfer request");
        }
    }
}
