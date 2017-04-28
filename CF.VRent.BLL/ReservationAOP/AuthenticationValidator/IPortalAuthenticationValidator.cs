using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP
{
    public interface IPortalAuthenticationValidator
    {
        bool Validate(IMethodInvocation input, IPortalCertification Cert);
    }
}
