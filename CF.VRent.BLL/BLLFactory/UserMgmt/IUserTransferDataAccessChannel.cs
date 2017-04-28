using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public interface IUserTransferDataAccessChannel
    {
        IEnumerable<UserTransferRequest> GetUserTransfersByUserClientID(Guid userClientID, ProxyUserSetting userInfo);
        UserTransferRequest GetUserTransferByUserID(Guid userID);
        UserTransferRequest GetUserTransferByUserID(Guid userID, UserRoleEntityCollection currentUserRole);
        UserTransferRequest UpdateUserTransfer(UserTransferRequest transfer, ProxyUserSetting userInfo);
        UserTransferRequest AddUserTransfer(UserTransferRequest transfer, ProxyUserSetting userInfo);
    }
}
