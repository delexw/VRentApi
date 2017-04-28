using CF.VRent.WCFExtension.Invoker;
using CF.VRent.WCFExtension.MessageFormatter;
using CF.VRent.WCFExtension.MessageInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;

namespace CF.VRent.WCFExtension.Behavior
{
    public class GlobalOperationInvokerBehavior : IOperationBehavior
    {
        public GlobalOperationInvokerBehavior(bool logMessageInfoEnabled, IRawFormaterConfiguration raw)
        {
            _logMessageInfoEnabled = logMessageInfoEnabled;
            _raw = raw;
        }

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
           
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
        {
           
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new GlobalOperationInvoker(dispatchOperation.Invoker, operationDescription.Name);
            dispatchOperation.ParameterInspectors.Add(new GlobalMessageInspector(_logMessageInfoEnabled));
            if (_raw != null && _raw.Enabled)
            {
                dispatchOperation.Formatter = new SoupMessageFormatter(dispatchOperation.Formatter, operationDescription, _raw.JsonFormatter, _raw.OutGoingFormatter);
            }
        }

        public void Validate(OperationDescription operationDescription)
        {
           
        }

        private bool _logMessageInfoEnabled;

        private IRawFormaterConfiguration _raw;
    }
}
