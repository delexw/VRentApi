using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP.CertificationValidator
{
    public class TransactionValidator : IPortalCertificationValidator
    {
        public bool Validate(IMethodInvocation input, PortalIdentity type, Entities.UserExtension user)
        {
            return true;
        }
    }
}
