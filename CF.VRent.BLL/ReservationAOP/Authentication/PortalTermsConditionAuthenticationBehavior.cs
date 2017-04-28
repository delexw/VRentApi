using CF.VRent.BLL.ReservationAOP.AuthenticationValidator;
using CF.VRent.BLL.ReservationAOP.CertificationValidator;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP
{
    public class PortalTermsConditionAuthenticationBehavior : ProtalAuthentication, IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var ret = this.Authenticate(input, 
                new TermsConditionValidator(), 
                PortalIdentity.TermsCondition, 
                new TermsConditionAuthenticationValidator());

            return ret == null ? getNext()(input, getNext) : ret;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
