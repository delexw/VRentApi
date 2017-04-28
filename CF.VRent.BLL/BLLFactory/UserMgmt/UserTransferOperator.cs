using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public class UserTransferOperator : IUserTransferDataAccessChannel
    {
        private ProxyUserSetting _sessionUser;
        public UserTransferOperator(ProxyUserSetting sessionUser)
        {
            _sessionUser = sessionUser;
        }
        /// <summary>
        /// Get all
        /// </summary>
        /// <param name="userClientID"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public IEnumerable<UserTransferRequest> GetUserTransfersByUserClientID(Guid userClientID, Common.UserContracts.ProxyUserSetting userInfo)
        {
            var userTransferBLL = new UserTransferBLL(_sessionUser);
            return userTransferBLL.LoadUserTransfer().AsEnumerable();
        }

        /// <summary>
        /// Update one
        /// </summary>
        /// <param name="transfer"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public UserTransferRequest UpdateUserTransfer(UserTransferRequest transfer, Common.UserContracts.ProxyUserSetting userInfo)
        {
            var userTransferBLL = new UserTransferBLL(_sessionUser);
            var approve = transfer.TransferResult == UserTransferResult.Approve ? true : false;



            return userTransferBLL.DetermineUserTransfer(transfer.UserID.ToString(), approve);
        }

        /// <summary>
        /// Add one
        /// </summary>
        /// <param name="transfer"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public UserTransferRequest AddUserTransfer(UserTransferRequest transfer, Common.UserContracts.ProxyUserSetting userInfo)
        {
            var userTransferBLL = new UserTransferBLL(_sessionUser);
            IDataService be = new DataAccessProxyManager();
            var result = be.AddTransferRequest(transfer, userInfo);
            if (result.Success != 0)
            {
                throw new VrentApplicationException(result);
            }

            return result.Data;
        }


        /// <summary>
        /// Get one
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserTransferRequest GetUserTransferByUserID(Guid userID)
        {
            var userTransferBLL = new UserTransferBLL(_sessionUser);
            return userTransferBLL.LoadPendingUserTransferRequest(userID.ToString());
        }

        /// <summary>
        /// Get one with VRent Manager Only
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="currentUserRole"></param>
        /// <returns></returns>
        public UserTransferRequest GetUserTransferByUserID(Guid userID, UserRoleEntityCollection currentUserRole)
        {
            //Only for VRent Manager
            if (currentUserRole.IsVRentManagerUser() &&
                !currentUserRole.IsServiceCenterUser() &&
                !currentUserRole.IsOperationManagerUser() &&
                !currentUserRole.IsAdministrationUser())
            {
                return this.GetUserTransferByUserID(userID);
            }

            return new UserTransferRequest();
        }
    }
}
