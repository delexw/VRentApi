using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UserStatus.Interfaces
{
    public interface IUserStatusManager
    {
        UserStatusEntityCollection Status { get; }
    }
}
