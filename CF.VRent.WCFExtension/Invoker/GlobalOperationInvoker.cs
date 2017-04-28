using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

namespace CF.VRent.WCFExtension.Invoker
{
    public class GlobalOperationInvoker : IOperationInvoker
    {
        private IOperationInvoker _invoker;
        //private string _methodName;
        //private LogTrace _trace;
        //private LogHelper _logHelper;
        //AutoResetEvent _exceptionEvent;

        public GlobalOperationInvoker(IOperationInvoker invoker, string methodName)
        {
            _invoker = invoker;
            //_methodName = methodName;
            //_trace = new LogTrace() { MethodName = _methodName };
            //_logHelper = new LogHelper();
        }

        public object[] AllocateInputs()
        {
            return _invoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            var r = _invoker.Invoke(instance, inputs, out outputs);
            WebOperationContext.Current.OutgoingResponse.Headers.Add("X-Frame-Options", "DENY");
            return r;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return _invoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return _invoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get { return _invoker.IsSynchronous; }
        }
    }
}
