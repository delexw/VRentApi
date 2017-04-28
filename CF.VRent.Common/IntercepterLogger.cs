using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common
{
    public class IntercepterLogger : IInterceptionBehavior
    {
        public virtual IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public virtual IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            IntercepterUtility.MethodIn(input);
            var ret = getNext()(input, getNext);
            IntercepterUtility.MethodOut(input, ret);
            return ret;
        }

        public virtual bool WillExecute
        {
            get { return true; }
        }
    }
}
