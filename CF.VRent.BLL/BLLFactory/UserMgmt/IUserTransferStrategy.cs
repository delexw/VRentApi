using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.UserRole;
using CF.VRent.UserStatus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public interface IUserTransferStrategy
    {
        UserExtension Run(UserExtension user, UserRoleEntityCollection currentUserRoleKey, ref IUserStatusManager statusManager);
    }
}
