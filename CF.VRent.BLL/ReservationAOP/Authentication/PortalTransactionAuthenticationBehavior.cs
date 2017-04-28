using CF.VRent.BLL.ReservationAOP.AuthenticationValidator;
using CF.VRent.BLL.ReservationAOP.CertificationValidator;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP.Authentication
{
    public class PortalTransactionAuthenticationBehavior : ProtalAuthentication, IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var ret = this.Authenticate(input,
                new TransactionValidator(),
                PortalIdentity.Transaction,
                new TransactionAuthenticationValidator());

            return ret == null ? getNext()(input, getNext) : ret;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
