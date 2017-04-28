using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Log;
using CF.VRent.WCFExtension.Behavior;
using CF.VRent.WCFExtension.MessageFormatter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Web;

namespace CF.VRent.WCFExtension
{
    public class VrentBehaviorExtention : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(GlobalWebHttpBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new GlobalWebHttpBehavior(LogDebugEnabled, LogErrorEnabled, LogDebugEnabled, ShowStackTrace, Raw);
        }

        [ConfigurationProperty("logErrorEnabled", DefaultValue = true)]
        public bool LogErrorEnabled
        {
            get
            {
                return (bool)base["logErrorEnabled"];
            }
            set
            {
                base["logErrorEnabled"] = value;
            }
        }

        [ConfigurationProperty("logDebugEnabled", DefaultValue = false)]
        public bool LogDebugEnabled
        {
            get
            {
                return (bool)base["logDebugEnabled"];
            }
            set
            {
                base["logDebugEnabled"] = value;
            }
        }

        [ConfigurationProperty("showStackTrace", DefaultValue = false)]
        public bool ShowStackTrace
        {
            get
            {
                return (bool)base["showStackTrace"];
            }
            set
            {
                base["showStackTrace"] = value;
            }
        }

        [ConfigurationProperty("messageTransfer")]
        public RawFormatterConfiguration Raw
        {
            get
            {
                return (RawFormatterConfiguration)base["messageTransfer"];
            }
            set
            {
                base["messageTransfer"] = value;
            }
        }
    }

    public class VrentExceptionHandling : IErrorHandler
    {
        public VrentExceptionHandling(bool logErrorEnabled, bool logDebugEnabled, bool showStackTrace)
        {
            _logErrorEnabled = logErrorEnabled;
            _logDebugEnabled = logDebugEnabled;
            _showStackTrace = showStackTrace;
        }

        public bool HandleError(Exception error)
        {
            //logging first
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            ReturnResult ret = ConvertExceptionToWebFault(error);

            HttpStatusCode code = HttpStatusCode.BadRequest;
            if (ret.Type == ResultType.OTHER)
            {
                code = HttpStatusCode.InternalServerError;
            }
            if (ret.Code == MessageCode.CVCE000002.ToString())
            {
                code = HttpStatusCode.Unauthorized;
            }

            WebFaultException<ReturnResult> finaRet = new WebFaultException<ReturnResult>(ret, code);

            ////// Create message
            fault = Message.CreateMessage(version, "", finaRet.Detail, new DataContractJsonSerializer(typeof(ReturnResult)));
            // Tell WCF to use JSON encoding rather than default XML
            var wbf = new WebBodyFormatMessageProperty(WebContentFormat.Json);
            fault.Properties.Add(WebBodyFormatMessageProperty.Name, wbf);

            WebOperationContext woc = WebOperationContext.Current;

            if (woc != null)
            {
                var response = woc.OutgoingResponse;
                response.ContentType = "application/json";
                ReWriteHttpStatus(response, finaRet);
            }


            //ReWriteHttpStatus(response,finaRet);

        }

        private ReturnResult ConvertExceptionToWebFault(Exception error)
        {
            ReturnResult ret = null;
            if (error is VrentApplicationException)
            {
                VrentApplicationException vae = error as VrentApplicationException;
                ret = new ReturnResult(false)
                {
                    Code = vae.ErrorCode,
                    Message = vae.ErrorMessage,
                    Type = vae.Category,
                    Success = vae.ReturnCode,
                    StackFrameMethod = vae.StackTrace
                };
                ret.Message = _formatMessage(ret.Message, ret.MessageArgs);
                this._logBusinessError(ret);
            }
            else if (error is WebFaultException)
            {
                WebFaultException internRet = error as WebFaultException;
                ret = new ReturnResult(false)
                {
                    Code = internRet.StatusCode.ToString(),
                    Message = internRet.Message
                };
                this._logBusinessError(ret);
            }
            else if (error is WebFaultException<ReturnResult>)
            {
                ret = (error as WebFaultException<ReturnResult>).Detail;
                ret.Message = _formatMessage(ret.Message, ret.MessageArgs);
                this._logBusinessError(ret);
            }
            else if (error is WebFaultException<string>)
            {
                WebFaultException<string> internRet = error as WebFaultException<string>;
                ret = new ReturnResult(false)
                {
                    Code = internRet.Code.ToString(),
                    Message = internRet.Detail,
                    Type = ResultType.OTHER
                };
                this._logBusinessError(ret);
            }
            //always throw by dataaccess
            else if (error is FaultException<ReturnResult>)
            {
                ret = (error as FaultException<ReturnResult>).Detail;
                ret.Message = _formatMessage(ret.Message, ret.MessageArgs);

                if (this._logErrorEnabled)
                {
                    if (ret.Type == ResultType.DATAACCESSProxy)
                    {
                        LogInfor.WriteError(ret.Code, ret.Message, _getUserId());
                    }
                }
            }
            else
            {
                ret = new ReturnResult(false)
                {
                    Code = MessageCode.CVCE000000.ToString(),
                    Message = MessageCode.CVCE000000.GetDescription(),
                    Type = MessageCode.CVCE000000.GetMessageType()
                };

                if (this._logErrorEnabled)
                {
                    string id = _getUserId();
                    LogInfor.WriteError(ret.Code, String.Format("Identified msg:{0}; Exception msg:{1}", ret.Message, error.ToString()), id);
                    LogHelper h = new LogHelper();
                    h.WriteLog(LogType.EXCEPTION, error.ToString());
                }
            }

            if (_showStackTrace)
            {
                ret.StackTrace = _getExceptionTrace(error);
            }

            return ret;
        }

        private void ReWriteHttpStatus(OutgoingWebResponseContext response, WebFaultException<ReturnResult> finaRet)
        {
            response.StatusCode = finaRet.StatusCode;
            response.SuppressEntityBody = false;
        }

        #region Private
        private void _logBusinessError(ReturnResult rr)
        {
            if (this._logErrorEnabled)
            {
                //var user
                LogInfor.WriteDebug(rr.Code, String.Format("Code:{0},Message:{1},Type:{2}", rr.Code.ToString(), rr.Message, rr.Type.ToString()), _getUserId());
            }
        }

        private string _getUserId()
        {
            try
            {
                return OperationContext.Current.IncomingMessageProperties.Keys.Contains("_user") ?
                                OperationContext.Current.IncomingMessageProperties["_user"].ToString() : "";
            }
            catch
            {
                //in some casese, exceptions will happen
                //i.e a exception occured in a api have parameters of Stream type
                return "System";
            }
        }

        private string _formatMessage(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    return String.Format(format, args);
                }
                catch
                {
                    return format;
                }
            }
            else
            {
                return format;
            }
        }

        private List<string> _getExceptionTrace(Exception error, int level = 1)
        {
            var e = error;
            List<string> estr = new List<string>();
            while (e != null)
            {
                estr.Add(String.Format("Error Lever {0} : {1}", level, error.ToStr()));
                e = e.InnerException;
            }
            return estr;
        }
        #endregion

        #region Property
        private bool _logErrorEnabled;


        private bool _logDebugEnabled;

        private bool _showStackTrace;
        #endregion
    }

    //public class ExceptionHandlingAttribute :Attribute, IServiceBehavior
    //{
    //    private Type _eceptionHandlerType = null;

    //    public ExceptionHandlingAttribute(Type t)
    //    {
    //        _eceptionHandlerType = t;
    //    }

    //    public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
    //    {
    //    }

    //    public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
    //    {

    //        //foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
    //        //{
    //        //    ChannelDispatcher cd = cdb as ChannelDispatcher;

    //        //    VrentExceptionHandling ehe = Activator.CreateInstance(_eceptionHandlerType) as VrentExceptionHandling;
    //        //    cd.ErrorHandlers.Add(ehe);
    //        //}
    //    }

    //    public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
    //    {;
    //    }
    //}

}