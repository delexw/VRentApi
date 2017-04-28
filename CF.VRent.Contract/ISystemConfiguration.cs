using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.UserRole;
using CF.VRent.UserStatus;

namespace CF.VRent.Contract
{
    public interface ISystemConfiguration:IBLL
    {
        SystemConfig GetSystemConfiguration();

        IEnumerable<UserStatusEntity> GetAllUserStatus(UserRoleEntityCollection currentUserRoleKey = null);
    }
}
