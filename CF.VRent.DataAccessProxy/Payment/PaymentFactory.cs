using CF.VRent.DataAccessProxy.Payment.UnionPayProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CF.VRent.DataAccessProxy.Payment
{
    public class PaymentFactory
    {
        public static IUnionPayInvoker GetInstance<T>(string uid, string token = null) where T : IUnionPayInvoker
        {
            return Activator.CreateInstance(typeof(T), uid, token) as IUnionPayInvoker;
        }

        public static ITransactionInvoker GetTranInstance<T>(string uid) where T : ITransactionInvoker
        {
            return Activator.CreateInstance(typeof(T), uid) as ITransactionInvoker;
        }
    }
}