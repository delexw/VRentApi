using CF.VRent.Entities;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP.CertificationValidator
{
    /// <summary>
    /// Terms condition certification validation
    /// </summary>
    public class TermsConditionValidator:IPortalCertificationValidator
    {
        public bool Validate(IMethodInvocation input, PortalIdentity type, UserExtension user)
        {
            //Always return true

            if (user != null &&
                 user.RoleEntities != null &&
                 user.RoleEntities.Count > 0)
            {
                input.Arguments[input.Arguments.Count - 1] = user.RoleEntities;
            }
            //return true
            return true;
        }
    }
}
