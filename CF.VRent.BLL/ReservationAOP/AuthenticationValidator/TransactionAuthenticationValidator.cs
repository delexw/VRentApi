using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP.AuthenticationValidator
{
    public class TransactionAuthenticationValidator : IPortalAuthenticationValidator
    {
        public bool Validate(IMethodInvocation input, IPortalCertification Cert)
        {
            if (input.MethodBase.Name == "UpdateExchangeMessageEnableRetry" ||
                input.MethodBase.Name == "UpdateExchangeMessageDisableRetry" ||
                input.MethodBase.Name == "UpdateExchangeMessageEnableRetryByBooking" ||
                input.MethodBase.Name == "UpdateExchangeMessageDisableRetryByBooking")
            {
                //Only service center or operation manager can go in these metohd
                if (Cert.User.RoleEntities.IsServiceCenterUser() ||
                    Cert.User.RoleEntities.IsOperationManagerUser())
                {
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}
