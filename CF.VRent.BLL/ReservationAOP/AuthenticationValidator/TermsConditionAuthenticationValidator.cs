using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP.AuthenticationValidator
{
    public class TermsConditionAuthenticationValidator : IPortalAuthenticationValidator
    {
        public bool Validate(IMethodInvocation input, IPortalCertification Cert)
        {
            if (input.MethodBase.Name == "AddOrUpgradeTC")
            {
                if (Cert.User.RoleEntities.IsServiceCenterUser() &&
                    !Cert.User.RoleEntities.IsVRentManagerUser() &&
                    !Cert.User.RoleEntities.IsAdministrationUser() &&
                    !Cert.User.RoleEntities.IsOperationManagerUser())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
