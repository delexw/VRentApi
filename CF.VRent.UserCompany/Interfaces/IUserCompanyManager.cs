
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UserCompany.Interfaces
{
    public interface IUserCompanyManager
    {
        UserComanyEntityCollection Companies { get; }
    }
}
