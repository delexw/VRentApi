using CF.VRent.Common.Entities;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Log;
using CF.VRent.UPSDK;
using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using CF.VRent.Common;
using CF.VRent.DAL;
using System.Configuration;
using CF.VRent.UPSDK.SDK;
using System.Text;

namespace CF.VRent.DataAccessProxy.Payment.UnionPayProxy
{
    public class UnionPayInvoker : IUnionPayInvoker
    {
        private string _uid;
        private string _token;

        public UnionPay Response
        {
            get;
            private set;
        }

        public UnionPayInvoker(string uid,string token)
        {
            _uid = uid;
            _token = token;
        }

        public ReturnResult PreAuthorizeViaUP(PaymentExchangeMessage message)
        {
            var token = "{" + UnionPayUtils.TokenDeserialize(_token) + "}";

            #region Real Code
            UnionPayCustomInfo cus = new UnionPayCustomInfo()
            {
                SmsCode = message.SmsCode
            };
            var tools = UPSDKFactory.CreateUtls(cus);
            tools.ReqReserved = message.ReservedMsg;
            //Log request
            tools.LogRequest = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Resquest) == 1;
            };
            //log response
            tools.LogResponse = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Response) == 1;
            };
            tools.SendRequestProxy = (Dictionary<string, string> param, string url) =>
            {
                return this.SendUnionPayRequest(param, url);
            };
            tools.TxnAmt = message.PreAuthPrice;
            tools.CurrencyCode = "156";
            tools.BackUrl = String.Format("{0}/{1}", ConfigurationManager.AppSettings["UnionPayListenerAddress"], "ListenService/ListenPreauth");
            if (message.PreAuthTempOrderID != null)
            {
                tools.OrderId = message.PreAuthTempOrderID;
            }
            if (message.PreAuthDateTime != null)
            {
                tools.TxnTime = message.PreAuthDateTime;
            }

            //Unionpay accepted preauthorization
            //But not sure the process whether is success or not
            var rResult = tools.PreAuthorize(token);

            this.Response = tools.UnionPayResponse;

            return rResult;

            #endregion
        }

        #region Utilities

        public int Log(UnionPay entity, UnionPayEnum type)
        {
            StackTrace st = new StackTrace(true);
            LogInfor.PayLogWriter.WriteInfo(st.GetFrame(1).GetMethod().Name + " " + type.ToStr(), entity.ObjectToJson(), _uid);

            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                           () => PaymentDAL.AddPaymentLog(entity, _uid, type.GetValue().ToString()));
        }

        public string SendUnionPayRequest(Dictionary<string, string> param, string url)
        {
            try
            {
                var hc = new HttpClient(url);
                if (hc.Send(param, Encoding.UTF8) == 200)
                {
                    return hc.Result;
                }
            }
            catch (Exception ex)
            {
                LogInfor.WriteError(MessageCode.CVB000023.ToStr(), ex.ToString(), "");
            }
            return null;
        } 

        #endregion


        public ReturnResult AddUPBindingCardViaUP(UnionPayCustomInfo customInfo, string reservedMsg, Action callBack = null)
        {
            UnionPayCustomInfo cusObj = customInfo;

            //cusObj.CardId = Guid.NewGuid().ToString();

            var tools = UPSDKFactory.CreateUtls(cusObj);
            tools.ReqReserved = reservedMsg;
            //Log request
            tools.LogRequest = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Resquest) == 1;
            };
            //log response
            tools.LogResponse = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Response) == 1;
            };
            //Request sending proxy
            tools.SendRequestProxy = (Dictionary<string, string> param, string url) =>
            {
                return this.SendUnionPayRequest(param, url);
            };

            var unionRet = tools.OpenUnionPayCard();

            this.Response = tools.UnionPayResponse;

            if (unionRet.Success == 1)
            {
                cusObj.Bank = tools.UnionPayResponse.IssInsCode;
                if (callBack != null)
                {
                    callBack();
                }
            }

            return unionRet;
        }


        public ReturnResult CheckPaymentStatusViaUP(PaymentExchangeMessage message, string resCode)
        {
            var tools = UPSDKFactory.CreateUtls();
            tools.UPStateTime = new UnionPayState(resCode);
            //Log request
            tools.LogRequest = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Resquest) == 1;
            };
            //log response
            tools.LogResponse = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Response) == 1;
            };
            tools.SendRequestProxy = (Dictionary<string, string> param, string url) =>
            {
                return this.SendUnionPayRequest(param, SDKConfig.SingleQueryUrl);
            };
            tools.OrigQryId = message.PreAuthQueryID;
            tools.TxnTime = message.PreAuthDateTime;
            tools.OrderId = message.PreAuthTempOrderID;

            var ret = tools.CheckPaymentStatus();

            this.Response = tools.UnionPayResponse;

            return ret;
        }


        public ReturnResult CancelCardBindingViaUP()
        {
            var res = new ReturnResult();
            var tools = UPSDKFactory.CreateUtls();
            //Log request
            tools.LogRequest = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Resquest) == 1;
            };
            //log response
            tools.LogResponse = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Response) == 1;
            };
            tools.SendRequestProxy = (Dictionary<string, string> param, string url) =>
            {
                return this.SendUnionPayRequest(param, url);
            };

            res = tools.CancelUnionPayCardBinding(_token);

            this.Response = tools.UnionPayResponse;


            return res;
        }


        public ReturnResult CancelPreauthorizationViaUP(PaymentExchangeMessage message, string callBackApiMethod)
        {
            //UnionPay orginPre = orginPreJson;

            var res = new ReturnResult() { Success = 0 };

            var tools = UPSDKFactory.CreateUtls();
            tools.ReqReserved = message.ReservedMsg;
            //Log request
            tools.LogRequest = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Resquest) == 1;
            };
            //log response
            tools.LogResponse = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Response) == 1;
            };
            tools.SendRequestProxy = (Dictionary<string, string> param, string url) =>
            {
                return this.SendUnionPayRequest(param, url);
            };
            tools.BackUrl = String.Format("{0}/{1}", ConfigurationManager.AppSettings["UnionPayListenerAddress"], callBackApiMethod);
            tools.OrigQryId = message.PreAuthQueryID;
            tools.TxnAmt = message.PreAuthPrice;

            res = tools.CancelPreAuthorization();

            this.Response = tools.UnionPayResponse;

            return res;
        }


        public ReturnResult SendBindingSMSCodeViaUP(UnionPayCustomInfo cardObjectJson)
        {
            UnionPayCustomInfo cardObject = cardObjectJson;

            var tools = UPSDKFactory.CreateUtls(cardObject);
            //Log request
            tools.LogRequest = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Resquest) == 1;
            };
            //log response
            tools.LogResponse = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Response) == 1;
            };
            tools.SendRequestProxy = (Dictionary<string, string> param, string url) =>
            {
                return this.SendUnionPayRequest(param, url);
            };
            var res = tools.SendVerificationSMS();

            this.Response = tools.UnionPayResponse;

            return res;
        }


        public ReturnResult SendPreauthorizationSMSCodeViaUP(UnionPayCustomInfo cardObjectJson, PaymentExchangeMessage message)
        {
            UnionPayCustomInfo cardObject = cardObjectJson;

            var tools = UPSDKFactory.CreateUtls(cardObject);

            var token = "{" + UnionPayUtils.TokenDeserialize(_token) + "}";

            //Log request
            tools.LogRequest = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Resquest) == 1;
            };
            //log response
            tools.LogResponse = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Response) == 1;
            };
            tools.SendRequestProxy = (Dictionary<string, string> param, string url) =>
            {
                return this.SendUnionPayRequest(param, url);
            };

            tools.CurrencyCode = "156";
            tools.TxnAmt = message.PreAuthPrice;
            tools.OrderId = message.PreAuthTempOrderID;
            tools.TxnTime = message.PreAuthDateTime;

            var res = tools.SendPreauthorizaitonSMS(token);

            this.Response = tools.UnionPayResponse;

            return res;
        }


        public ReturnResult CompletePreauthorizaionViaUP(PaymentExchangeMessage message)
        {
            //var mes = this.GetPaymentExchangeInfo(paymentId, uid);

            var tools = UPSDKFactory.CreateUtls();
            tools.ReqReserved = message.ReservedMsg;
            //Log request
            tools.LogRequest = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Resquest) == 1;
            };
            //log response
            tools.LogResponse = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Response) == 1;
            };
            tools.SendRequestProxy = (Dictionary<string, string> param, string url) =>
            {
                return this.SendUnionPayRequest(param, url);
            };

            tools.BackUrl = String.Format("{0}/{1}", ConfigurationManager.AppSettings["UnionPayListenerAddress"], "ListenService/ListenCompletePreauth");
            tools.OrigQryId = message.PreAuthQueryID;
            tools.TxnAmt = message.RealPreAuthPrice;

            var ret = tools.FinishPreAuthorization();

            this.Response = tools.UnionPayResponse;

            return ret;
        }


        public ReturnResult DeductionViaUP(PaymentExchangeMessage message)
        {
            var token = "{" + UnionPayUtils.TokenDeserialize(_token) + "}";

            var tools = UPSDKFactory.CreateUtls();
            tools.ReqReserved = message.ReservedMsg;
            //Log request
            tools.LogRequest = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Resquest) == 1;
            };
            //log response
            tools.LogResponse = (UnionPay u) =>
            {
                return Log(u, UnionPayEnum.Response) == 1;
            };
            tools.SendRequestProxy = (Dictionary<string, string> param, string url) =>
            {
                return this.SendUnionPayRequest(param, url);
            };
            tools.BackUrl = String.Format("{0}/{1}", ConfigurationManager.AppSettings["UnionPayListenerAddress"], "ListenService/ListenCompleteConsuming");
            tools.TxnAmt = message.DeductionPrice;
            tools.CurrencyCode = "156";

            if (!String.IsNullOrWhiteSpace(message.PreAuthTempOrderID))
            {
                tools.OrderId = message.PreAuthTempOrderID;
            }
            if (!String.IsNullOrWhiteSpace(message.PreAuthDateTime))
            {
                tools.TxnTime = message.PreAuthDateTime;
            }

            var r = tools.Consume(token);

            this.Response = tools.UnionPayResponse;

            return r;
        }
    }
}