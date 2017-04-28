using CF.VRent.WCFExtension.MessageFormatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace CF.VRent.WCFExtension.Behavior
{
    public class GlobalWebHttpBehavior : WebHttpBehavior
    {
        public GlobalWebHttpBehavior(bool logMessageInfoEnabled, bool logErrorEnabled, bool logDebugEnabled, bool showStackTrace, IRawFormaterConfiguration raw)
        {
            _logMessageInfoEnabled = logMessageInfoEnabled;
            _logErrorEnabled = logErrorEnabled;
            _logDebugEnabled = logDebugEnabled;
            _showStackTrace = showStackTrace;
            _raw = raw;
        }

        public override void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            base.ApplyDispatchBehavior(endpoint, endpointDispatcher);

            foreach (OperationDescription od in endpoint.Contract.Operations)
            {
                if (od.SyncMethod != null)
                {
                    od.Behaviors.Add(new GlobalOperationInvokerBehavior(_logMessageInfoEnabled, _raw));
                }
            }
        }

        protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {

            //endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();
            //endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new VrentExceptionHandling());

            IErrorHandler vrentHandler = endpointDispatcher.ChannelDispatcher.ErrorHandlers.FirstOrDefault(m => m.GetType().Equals(typeof(VrentExceptionHandling)));
            if (vrentHandler == null)
            {
                endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();
                endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new VrentExceptionHandling(_logErrorEnabled, _logDebugEnabled, _showStackTrace));
            }
        }

        private bool _logMessageInfoEnabled;

        private bool _logErrorEnabled;

        private bool _logDebugEnabled;

        private bool _showStackTrace;

        private IRawFormaterConfiguration _raw;
    }
}
