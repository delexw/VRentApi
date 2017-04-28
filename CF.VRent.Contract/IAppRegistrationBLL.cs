using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface IAppRegistrationBLL : IBLL
    {
        UserExtension UserRegistration(UserExtension feReg, string lang);
        UserExtension UpdateProfile(UserExtension feUpdate, string lang);
    }
}
