using CF.VRent.Common.Entities.UserExt;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public class UserMgmtPermissionStrategy : IUserMgmtPermissionStrategy
    {
        public string GetUserRoleKeyInTransfer(UserRoleEntityCollection loginUserRole, 
            UserMgmtNavigation.Operation currentOperation)
        {
            //For transfer navigation in portal
            if (currentOperation == UserMgmtNavigation.Operation.TransferUser)
            {
                if (loginUserRole.IsVRentManagerUser())
                {
                    return UserRoleConstants.VRentManagerKey;
                }

                if (loginUserRole.IsAdministrationUser())
                {
                    return UserRoleConstants.AdministratorKey;
                }

                if (loginUserRole.IsServiceCenterUser())
                {
                    return UserRoleConstants.ServiceCenterKey;
                }
            }

            //For user navigation in portal
            if (currentOperation == UserMgmtNavigation.Operation.UpdateUser)
            {
                if (loginUserRole.IsOperationManagerUser())
                {
                    return UserRoleConstants.OperationManagerKey;
                }

                if (loginUserRole.IsAdministrationUser())
                {
                    return UserRoleConstants.AdministratorKey;
                }

                if (loginUserRole.IsServiceCenterUser())
                {
                    return UserRoleConstants.ServiceCenterKey;
                }

                if (loginUserRole.IsVRentManagerUser())
                {
                    return UserRoleConstants.VRentManagerKey;
                }
            }

            return null;
        }
    }
}
