using CF.VRent.BLL.ReservationAOP.AuthenticationValidator;
using CF.VRent.BLL.ReservationAOP.CertificationValidator;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.EntityFactory;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace CF.VRent.BLL.ReservationAOP
{
    /// <summary>
    /// This is used to authenticate SC or VM
    /// </summary>
    public class PortalUserMgmtAuthenticationBehavior : ProtalAuthentication, IInterceptionBehavior
    {

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var ret = this.Authenticate(input,
                new UserMgmtCertificationValidator(),
                PortalIdentity.UserMgmt,
                new UserMgmtAuthenticationValidator());

            return ret == null ? getNext()(input, getNext) : ret;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
