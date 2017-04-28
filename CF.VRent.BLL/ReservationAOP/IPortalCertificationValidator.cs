using CF.VRent.Entities;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP
{
    public interface IPortalCertificationValidator
    {
        bool Validate(IMethodInvocation input, PortalIdentity type, UserExtension user);
    }
}
