using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities.PaymentService;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP
{
    public class TransactionInterceptionBehavior : IInterceptionBehavior
    {
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var ret = getNext()(input, getNext);

            //No exception is threw
            if (ret.Exception == null)
            {
                if (ret.ReturnValue != null && ret.ReturnValue.GetType() == typeof(PaymentExchangeMessage))
                {
                    //Filter the retrun entity to avoid security issues
                    var obj = ret.ReturnValue as PaymentExchangeMessage;
                    ret.ReturnValue = new PaymentFactory().CreateFilterEntity(obj);
                }
            }

            return ret;
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }
}
