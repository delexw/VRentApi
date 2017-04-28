using CF.VRent.Log;
using CF.VRent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using CF.VRent.WCFExtension.Behavior;

namespace CF.VRent.WCFExtension.MessageInspector
{
    public class GlobalMessageInspector : IParameterInspector
    {
        public GlobalMessageInspector(bool logMessageInfoEnabled)
        {
            _logMessageInfoEnabled = logMessageInfoEnabled;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            if (_logMessageInfoEnabled)
            {
                var logEntry = correlationState as LogEntry;
                if (logEntry != null)
                {
                    logEntry.OperationDirection = String.Format("Service -- {0} Out", operationName);
                    //logEntry.ReturnValue = returnValue.ObjectToJson();
                    LogInfor.WriteInfo(logEntry.OperationDirection, logEntry.ObjectToJson(), logEntry.OperationUser);
                    LogInfor.WriteInfo(logEntry.OperationDirection + " outputs", returnValue.ObjectToJson(), logEntry.OperationUser);
                }
                logEntry = null;
            }
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            try
            {
                if (_logMessageInfoEnabled)
                {
                    var remote = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    if (remote != null)
                    {
                        var logEntry = new LogEntry()
                        {
                            OperationDirection = String.Format("Service -- {0} In", operationName),
                            RequestClient = String.Join(":", remote.Address, remote.Port),
                            OperationName = operationName,
                            OperationUser = OperationContext.Current.IncomingMessageProperties.Keys.Contains("_user") ?
                            OperationContext.Current.IncomingMessageProperties["_user"].ToString() : "",
                            Format = OperationContext.Current.IncomingMessageProperties.Keys.Contains(WebBodyFormatMessageProperty.Name) ?
                            ((WebBodyFormatMessageProperty)OperationContext.Current.IncomingMessageProperties[WebBodyFormatMessageProperty.Name]).Format.ToString() : "",
                            IsUrlMatched = OperationContext.Current.IncomingMessageProperties["UriMatched"].ToString(),
                            Uri = OperationContext.Current.IncomingMessageProperties["Via"].ToString(),
                            RequestHttpMethod = ((HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name]).Method
                        };
                        LogInfor.WriteInfo(logEntry.OperationDirection, logEntry.ObjectToJson(), logEntry.OperationUser);
                        LogInfor.WriteInfo(logEntry.OperationDirection + " inputs", inputs.ObjectToJson(), logEntry.OperationUser);
                        return logEntry;
                    }
                }
                return null;
                //LogTrace _trace = new LogTrace() { MethodName = _methodName };
                //_trace.StepIn2();
            }
            catch
            {
                return null;
            }
        }

        private bool _logMessageInfoEnabled;
    }
}
