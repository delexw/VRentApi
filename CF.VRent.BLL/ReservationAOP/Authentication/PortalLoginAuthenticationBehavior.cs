using CF.VRent.BLL.ReservationAOP.CertificationValidator;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP
{
    public class PortalLoginAuthenticationBehavior :ProtalAuthentication, IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var returnObj = getNext()(input, getNext);

            IMethodReturn ret = null;
            if (returnObj.Exception == null)
            {
                ret = this.Authenticate(input, new LoginCertificationValidator(), PortalIdentity.Login, null);
            }

            return ret == null ? returnObj : ret;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
