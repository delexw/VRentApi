using CF.VRent.Common.Entities.UserExt;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public interface IUserMgmtPermissionStrategy
    {
        string GetUserRoleKeyInTransfer(UserRoleEntityCollection loginUserRole, UserMgmtNavigation.Operation currentOperation);
    }
}
