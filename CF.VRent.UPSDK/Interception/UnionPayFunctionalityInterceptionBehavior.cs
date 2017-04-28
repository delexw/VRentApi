using CF.VRent.Log;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Common.Entities;

namespace CF.VRent.UPSDK.Interception
{
    public class UnionPayFunctionalityInterceptionBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var ret = getNext()(input, getNext);
            if (ret.Exception != null)
            {
                LogInfor.WriteError(MessageCode.CVB000025.ToStr(), ret.Exception.ToStr(), "Union Pay SDK");
            }
            return ret;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
