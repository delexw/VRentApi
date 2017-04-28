using CF.VRent.Entities;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP.CertificationValidator
{
    public class UserMgmtCertificationValidator:IPortalCertificationValidator
    {
        private string[] _apiMethodNames { get {
            return new string[] {
 
            };
        } }

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
                    //Override the last argument 
                    input.Arguments[input.Arguments.Count - 1] = user.RoleEntities;
                    return true;
                }
            }
            return false;
        }
    }
}
