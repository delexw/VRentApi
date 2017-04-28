using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities;
using CF.VRent.UserRole;
using CF.VRent.UserStatus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public interface ICreateCorporateUserStrategy
    {
        UserExtension Run(UserExtension inputUser, UserRoleEntityCollection currentUserRoleKey, ref IUserStatusManager statusManager);
    }
}
