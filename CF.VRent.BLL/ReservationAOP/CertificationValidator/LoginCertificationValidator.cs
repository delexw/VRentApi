using CF.VRent.Entities;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP.CertificationValidator
{
    /// <summary>
    /// Validate the permission of login portal
    /// </summary>
    public class LoginCertificationValidator:IPortalCertificationValidator
    {
        public LoginCertificationValidator()
        {
        }

        public bool Validate(IMethodInvocation input, PortalIdentity type, UserExtension user)
        {
            if (user != null &&
                user.RoleEntities != null &&
                user.RoleEntities.Count > 0)
            {
                //User Role
                if (user.RoleEntities.IsVRentManagerUser() ||
                    user.RoleEntities.IsServiceCenterUser() ||
                    user.RoleEntities.IsOperationManagerUser() ||
                    user.RoleEntities.IsAdministrationUser())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
