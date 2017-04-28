
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UserRole.Interfaces
{
    public interface IUserRoleManager
    {
        UserRoleEntityCollection Roles { get; }
    }
}
