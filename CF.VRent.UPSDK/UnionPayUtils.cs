using CF.VRent.Common.Entities;
using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.UPSDK.SDK;
using System.Runtime.Serialization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Configuration;
using CF.VRent.Log;
using Microsoft.Practices.Unity;

namespace CF.VRent.UPSDK
{
    /// <summary>
    /// Do unionpay operations
    /// Added by Adam Liu 2015-5
    /// </summary>
    [DataContract]
    public class UnionPayUtils : IUnionPayUtils
    {
        #region Delegation
        /// <summary>
        /// Log request delegation
        /// </summary>

        public Func<UnionPay, bool> LogRequest { get; set; }

        /// <summary>
        /// Log response delegation
        /// </summary>
        public Func<UnionPay, bool> LogResponse { get; set; }

        /// <summary>
        /// Log payment success information delegation 
        /// </summary>
        public Func<UnionPay, bool> LogMerInform { get; set; }

        /// <summary>
        /// Send request to up
        /// </summary>
        private Func<Dictionary<string, string>, string, string> _sendRequestProxy;
        public Func<Dictionary<string, string>, string, string> SendRequestProxy 
        {
            get
            {
                if (_sendRequestProxy == null)
                {
                    _sendRequestProxy = (Dictionary<string, string> param, string url) => {
                        try
                        {

                            var client = new HttpClient(url);
                            var result = client.Send(param, Encoding.UTF8);
                            if (result == 200)
                            {
                                return client.Result;
                            }
                        }
                        catch(Exception ex)
                        {
                            new LogHelper().WriteLog(LogType.EXCEPTION, ex.ToString());
                        }
                        return null;
                    };
                }
                return _sendRequestProxy;
            }
            set
            {
                _sendRequestProxy = value;
            }
        }
        #endregion

        #region Properties

        #region Inner Properties

        private UnionPay _unionPay;
        /// <summary>
        /// Always return the latest Request
        /// </summary>
        public UnionPay UnionPayRequest
        {
            get
            {
                return _unionPay;
            }
            private set
            {
                _unionPay = value;
            }
        }

        private UnionPay _unionPayResponse;
        /// <summary>
        /// Response Obj
        /// </summary>
        public UnionPay UnionPayResponse
        {
            get
            {
                return _unionPayResponse;
            }
            private set
            {
                _unionPayResponse = value;
            }
        }

        private UnionPay _unionPayMerInform;
        /// <summary>
        /// Mer Information Obj
        /// </summary>
        public UnionPay UnionPayMerInform
        {
            get
            {
                return _unionPayMerInform;
            }
            private set
            {
                _unionPayMerInform = value;
            }
        }

        private UnionPayCustomInfo _upc;
        public UnionPayCustomInfo UPCustomerInfo
        {
            get
            {
                if (_upc == null)
                    _upc = new UnionPayCustomInfo();
                return _upc;
            }
            set
            {
                _upc = value;
            }
        }

        private UnionPayTokenPay _upt;
        public UnionPayTokenPay UPTokenPay
        {
            get
            {
                return _upt;
            }
            set
            {
                _upt = value;
            }
        }

        private UnionPayState _ups;
        public UnionPayState UPStateTime
        {
            get
            {
                return _ups;
            }
            set
            {
                _ups = value;
            }
        }
        #endregion

        #region Exposing Properties, values from FE
        /// <summary>
        /// BackUrl
        /// </summary>
        public string BackUrl { get; set; }

        /// <summary>
        /// TxnAmt
        /// </summary>
        private string _txnAmt;
        public string TxnAmt
        {
            get
            {
                var _txnAmtFen = UnionPayUtils.YuanToFen(_txnAmt);
                return _txnAmtFen;
            }
            set
            {
                _txnAmt = value;
            }
        }

        /// <summary>
        /// CurrencyCode
        /// </summary>
        private string _currencyCode;
        public string CurrencyCode {
            get {
                if (String.IsNullOrWhiteSpace(_currencyCode))
                {
                    _currencyCode = "156";
                }
                return _currencyCode;
            }
            set {
                _currencyCode = value;
            } 
        }

        private string _orderId;
        /// <summary>
        /// OrderId
        /// </summary>
        public string OrderId
        {
            get
            {
                if (_orderId.ToStr() == "")
                    _orderId = DateTime.Now.ToString("yyyyMMddHHmmss") + (new Random().Next(900) + 100).ToString().Trim();
                return _orderId;
            }
            set
            {
                _orderId  =value;
            }
        }

        private string _txnTime;
        /// <summary>
        /// TxnTime
        /// </summary>
        public string TxnTime
        {
            get
            {
                if (_txnTime.ToStr() == "")
                    _txnTime = DateTime.Now.ToString("yyyyMMddhhmmss");
                return _txnTime;
            }
            set{
                _txnTime = value; 
            }
        }

        /// <summary>
        /// Equal to QueryId
        /// </summary>
        public string OrigQryId { get; set; }

        /// <summary>
        /// (Keep the value unique in every transaction of unionpay request)
        /// </summary>
        public string ReqReserved { get; set; }

        #endregion

        #endregion

        private WaitCallback _logCallBack;


        private string MERACCOUNT = ConfigurationManager.AppSettings["MERACCOUNT"];
        private string MERTRID = ConfigurationManager.AppSettings["MERTRID"];
        private string MERTOKENTYPE = ConfigurationManager.AppSettings["MERTOKENTYPE"];

        public const string ReservedMessageKey1 = "UPUniqueId";
        public const string ReservedMessageKey2 = "UserId";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="upc">UP api -- CustomerInfo</param>
        /// <param name="upt">Up apt -- TokenInfo</param>
        public UnionPayUtils(UnionPayCustomInfo upc, UnionPayTokenPay upt)
        {
            _upc = upc;
            _upt = upt;
            _logCallBack = new WaitCallback(_LogAsync);
            if (upt == null)
            {
                _upt = new UnionPayTokenPay()
                {
                    TrId = MERTRID,
                    TokenType = MERTOKENTYPE
                };
            }
            ReqReserved = Guid.NewGuid().ToString();

            _unionPayResponse = new UnionPay();
            _unionPayMerInform = new UnionPay();
            _unionPay = new UnionPay();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="upc">UP api -- CustomerInfo</param>
        [InjectionConstructor]
        public UnionPayUtils(UnionPayCustomInfo upc)
            : this(upc, null)
        { }

        public UnionPayUtils()
            : this(null, null)
        { }

        #region Public
        /// <summary>
        /// Get token from unionpay (card binding)
        /// </summary>
        /// <returns></returns>
        public ReturnResult OpenUnionPayCard()
        {
            _BuildCommonParam();

            _unionPay.TxnType = "79";
            _unionPay.TxnSubType = "00";
            _unionPay.AccNo = _upc.CardNo;

            //Modified by adam
            //Exclude the CustomerNm & CertifId when the certType is 03 (passport)
            if (_upc.CertifTp == "03")
            {
                _upc.CustomerNm = null;
                _upc.CertifId = null;
            }

            var dic = _upc.ToDictionary<string, string>(true, "CardNo", "CardId", "Bank");

            if (dic.Keys.Count > 0)
            {
                _unionPay.CustomerInfo = SDK.SecurityUtil.EncodeBase64(Encoding.UTF8, "{" + SDK.SDKUtil.CoverDictionaryToString(dic) + "}");
            }

            _unionPay.TokenPayData = "{" + _upt.ToKeyValueString() + "}";

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                if (_unionPayResponse.RespCode == "00" && _unionPayResponse.ActivateStatus == "1")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Cancel token from unionpay(card unbinding)
        /// </summary>
        /// <returns></returns>
        public ReturnResult CancelUnionPayCardBinding(string token)
        {
            _BuildCommonParam();

            _unionPay.TxnType = "74";
            _unionPay.TxnSubType = "01";
            _unionPay.TokenPayData = token;

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                if (_unionPayResponse.RespCode == "00")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }
            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Send sms to Unionpay to get the code
        /// </summary>
        /// <returns></returns>
        public ReturnResult SendVerificationSMS()
        {
            _BuildCommonParam();
            _unionPay.TxnType = "77";
            _unionPay.TxnSubType = "00";
            _unionPay.AccNo = _upc.CardNo;

            var dic = _upc.ToDictionary<string, string>(true, "CardNo", "CardId", "CertifTp", "CertifId", "CustomerNm", "Expired", "Cvn2", "SmsCode", "Bank");
            if (dic.Keys.Count > 0)
            {
                _unionPay.CustomerInfo = SDK.SecurityUtil.EncodeBase64(Encoding.UTF8, "{" + SDK.SDKUtil.CoverDictionaryToString(dic) + "}");
            }

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                if (_unionPayResponse.RespCode == "00")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Send sms to unionpay to get preauthorizaiton code
        /// </summary>
        /// <returns></returns>
        public ReturnResult SendPreauthorizaitonSMS(string token)
        {
            _BuildCommonParam();
            _unionPay.TxnType = "77";
            _unionPay.TxnSubType = "04";

            var dic = _upc.ToDictionary<string, string>(true, "CardNo", "CardId", "CertifTp", "CertifId", "CustomerNm", "Expired", "Cvn2", "SmsCode", "Bank");
            if (dic.Keys.Count > 0)
            {
                _unionPay.CustomerInfo = SDK.SecurityUtil.EncodeBase64(Encoding.UTF8, "{" + SDK.SDKUtil.CoverDictionaryToString(dic) + "}");
            }

            _unionPay.TokenPayData = token;
            _unionPay.CurrencyCode = this.CurrencyCode;
            _unionPay.TxnAmt = this.TxnAmt;
            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                if (_unionPayResponse.RespCode == "00")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };

        }

        /// <summary>
        /// do pre-authorization
        /// </summary>
        /// <returns></returns>
        public ReturnResult PreAuthorize(string token)
        {
            _BuildCommonParam();
            _unionPay.TxnType = "02";
            _unionPay.TxnSubType = "01";

            //TODO: For test
            //_unionPay.BizType = "000301";
            //_unionPay.AccNo = _upc.CardNo;

            var dic = _upc.ToDictionary<string, string>(true, "CardNo", "CardId", "CertifTp", "CertifId", "CustomerNm", "Expired", "Cvn2", "Bank", "PhoneNo");
            if (dic.Keys.Count > 0)
            {
                _unionPay.CustomerInfo = SDK.SecurityUtil.EncodeBase64(Encoding.UTF8, "{" + SDK.SDKUtil.CoverDictionaryToString(dic) + "}");
            }

            //BackUrl
            _unionPay.BackUrl = this.BackUrl;

            //TODO: For production
            _unionPay.TokenPayData = token;

            //txnAmt & currencyCode
            _unionPay.TxnAmt = this.TxnAmt;
            _unionPay.CurrencyCode = this.CurrencyCode;

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                _ups = new UnionPayState(_unionPayResponse.RespCode);
                if (_unionPayResponse.RespCode == "00" || 
                    _unionPayResponse.RespCode == "A6")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
                if (_unionPayResponse.RespCode == "03" ||
                    _unionPayResponse.RespCode == "04" ||
                    _unionPayResponse.RespCode == "05")
                {
                    return new ReturnResult(false) { Success = 2, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Cancel pre-authorization
        /// </summary>
        /// <returns></returns>
        public ReturnResult CancelPreAuthorization()
        {
            _BuildCommonParam();
            _unionPay.TxnType = "32";
            _unionPay.TxnSubType = "00";
            //BackUrl
            _unionPay.BackUrl = this.BackUrl;
            _unionPay.OrderId = this.OrderId;
            _unionPay.OrigQryId = this.OrigQryId;
            _unionPay.TxnTime = this.TxnTime;
            _unionPay.TxnAmt = this.TxnAmt;

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                _ups = new UnionPayState(_unionPayResponse.RespCode);
                if (_unionPayResponse.RespCode == "00" || 
                    _unionPayResponse.RespCode == "A6")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
                if (_unionPayResponse.RespCode == "03" ||
                    _unionPayResponse.RespCode == "04" ||
                    _unionPayResponse.RespCode == "05")
                {
                    return new ReturnResult(false) { Success = 2, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Finish the pre-authorization
        /// </summary>
        /// <returns></returns>
        public ReturnResult FinishPreAuthorization()
        {
            _BuildCommonParam();
            _unionPay.TxnType = "03";
            _unionPay.TxnSubType = "00";
            _unionPay.BackUrl = this.BackUrl;
            _unionPay.OrderId = this.OrderId;
            _unionPay.OrigQryId = this.OrigQryId;
            _unionPay.TxnTime = this.TxnTime;
            _unionPay.TxnAmt = this.TxnAmt;

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                _ups = new UnionPayState(_unionPayResponse.RespCode);
                if (_unionPayResponse.RespCode == "00" || 
                    _unionPayResponse.RespCode == "A6")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg };
                }
                if (_unionPayResponse.RespCode == "03" ||
                    _unionPayResponse.RespCode == "04" ||
                    _unionPayResponse.RespCode == "05")
                {
                    return new ReturnResult(false) { Success = 2, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Refund after settlement day in 11 months
        /// </summary>
        /// <returns></returns>
        public ReturnResult ReturnGoodsConsume()
        {
            _BuildCommonParam();
            _unionPay.TxnType = "33";
            _unionPay.TxnSubType = "00";
            _unionPay.BackUrl = this.BackUrl;
            _unionPay.OrderId = this.OrderId;
            _unionPay.OrigQryId = this.OrigQryId;
            _unionPay.TxnTime = this.TxnTime;
            _unionPay.TxnAmt = this.TxnAmt;

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");
            if (_SendRequestAndValidate(unionDic))
            {
                _ups = new UnionPayState(_unionPayResponse.RespCode);
                if (_unionPayResponse.RespCode == "00" ||
                    _unionPayResponse.RespCode == "A6")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg };
                }
                if (_unionPayResponse.RespCode == "03" ||
                    _unionPayResponse.RespCode == "04" ||
                    _unionPayResponse.RespCode == "05")
                {
                    return new ReturnResult(false) { Success = 2, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Refund before settlement day
        /// </summary>
        /// <returns></returns>
        public ReturnResult CancelConsume()
        {
            return new ReturnResult(false);
        }

        /// <summary>
        /// Deduct directly
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ReturnResult Consume(string token)
        {
            _BuildCommonParam();
            _unionPay.TxnType = "01";
            _unionPay.TxnSubType = "01";
            _unionPay.BackUrl = this.BackUrl;
            _unionPay.OrderId = this.OrderId;
            _unionPay.TxnTime = this.TxnTime;
            _unionPay.TxnAmt = this.TxnAmt;
            _unionPay.CurrencyCode = this.CurrencyCode;
            _unionPay.TokenPayData = token;

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                _ups = new UnionPayState(_unionPayResponse.RespCode);
                if (_unionPayResponse.RespCode == "00" || 
                    _unionPayResponse.RespCode == "A6")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg };
                }
                if (_unionPayResponse.RespCode == "03" ||
                    _unionPayResponse.RespCode == "04" ||
                    _unionPayResponse.RespCode == "05")
                {
                    return new ReturnResult(false) { Success = 2, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Check whether the card is binding or not
        /// </summary>
        /// <returns></returns>
        public ReturnResult CheckUnionPayCardStatus()
        {
            _BuildCommonParam();
            _unionPay.TxnType = "78";
            _unionPay.TxnSubType = "02";

            var dic = _upc.ToDictionary<string, string>(true, "CardNo", "CardId", "CertifTp", "CertifId", "CustomerNm", "Expired", "Cvn2", "SmsCode", "Bank");
            if (dic.Keys.Count > 0)
            {
                _unionPay.CustomerInfo = SDK.SecurityUtil.EncodeBase64(Encoding.UTF8, "{" + SDK.SDKUtil.CoverDictionaryToString(dic) + "}");
            }

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                if (_unionPayResponse.RespCode == "00" &&
                    _unionPayResponse.ActivateStatus != null &&
                    _unionPayResponse.ActivateStatus == "1")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Check whether the payment is sccess or not
        /// </summary>
        /// <param name="everyCheckCallBack">a delegation to be invoked after every checking finished.</param>
        /// <returns></returns>
        public ReturnResult CheckPaymentStatus()
        {
            var count = this._ups.TimeSpan.Length;

            _BuildCommonParam();
            _unionPay.TxnType = "00";
            _unionPay.TxnSubType = "00";
            if (String.IsNullOrWhiteSpace(this.OrigQryId))
            {
                _unionPay.TxnTime = this.TxnTime;
                _unionPay.OrderId = this.OrderId;
            }
            else
            {
                _unionPay.QueryId = this.OrigQryId;
            }

            var unionDic = _unionPay.ToDictionary<string, string>(true, "UniqueID");

            if (_SendRequestAndValidate(unionDic))
            {
                if ((_unionPayResponse.RespCode == "00" || _unionPayResponse.RespCode == "A6")
                    && _unionPayResponse.OrigRespCode == "00")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }

            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
        }

        /// <summary>
        /// Extenal send
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ReturnResult SendRequest(Dictionary<string, string> param)
        {
            if (_SendRequestAndValidate(param))
            {
                if (_unionPayResponse.RespCode == "00")
                {
                    return new ReturnResult(false) { Success = 1, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };
                }
            }
            return new ReturnResult(false) { Success = 0, Code = _unionPayResponse.RespCode, Message = _unionPayResponse.RespMsg, Type = ResultType.UNIONPAY };

        }
        #endregion


        #region Private
        private bool _SendRequestAndValidate(Dictionary<string, string> param)
        {
            param["reqReserved"] = ReqReserved;
            if (SDK.SDKUtil.Sign(param, Encoding.UTF8))
            {
                return _InnerSendRequestAndValidate(param);
            }
            return false;
        }

        private bool _InnerSendRequestAndValidate(Dictionary<string, string> param)
        {
            //Get signature
            _unionPay = param.ToEntity<UnionPay, string, string>();
            //HttpClient hc = new HttpClient(SDKConfig.BackTransUrl);
            //Log the paymentLog table
            if (LogRequest != null)
            {
                LogRequest(_unionPay);
                //ThreadPool.QueueUserWorkItem(_logCallBack, new KeyValuePair<int, UnionPay>(0, _unionPay));
            }
            _unionPay.UniqueID = _unionPay.ReqReserved;
            var result = SendRequestProxy(param, SDKConfig.BackTransUrl);
            //int status = hc.Send(param, Encoding.UTF8);
            if (result != null)
            {
                var tokenStr = _GetInnerObjectString(result, "tokenPayData");
                result = tokenStr.Trim().Length > 0 ? result.Replace(tokenStr, "") : result;
                Dictionary<string, string> resData = SDKUtil.CoverstringToDictionary(result);
                _unionPayResponse = resData.ToEntity<UnionPay, string, string>();
                //Get token
                _unionPayResponse.TokenPayData = tokenStr;
                if (tokenStr.Trim().Length > 0)
                {
                    resData["tokenPayData"] = tokenStr;
                }

                _unionPayResponse.UniqueID = _unionPay.UniqueID;

                //Log the paymentLog table
                if (LogResponse != null)
                {
                    LogResponse(_unionPayResponse);
                    //ThreadPool.QueueUserWorkItem(_logCallBack, new KeyValuePair<int, UnionPay>(1, _unionPayResponse));
                }
                if (SDKUtil.Validate(resData, Encoding.UTF8))
                {
                    return true;
                }
            }
            return false;
        }

        private void _BuildCommonParam()
        {
            _unionPay = new Entities.UnionPay();
            _unionPay.Version = "5.0.0";
            _unionPay.Encoding = "UTF-8";
            _unionPay.SignMethod = "01";
            _unionPay.CertId = SDK.CertUtil.GetSignCertId();
            _unionPay.BizType = "000902";
            _unionPay.AccessType = "0";
            _unionPay.ChannelType = "07";
            //TODO: For test
            _unionPay.MerId = MERACCOUNT;//898111153990125
            //TODO: For production
            //_unionPay.MerId = "898111475190101";

            _unionPay.OrderId = this.OrderId;
            _unionPay.TxnTime = this.TxnTime;
            //_unionPay.AccNo = _upc.CardNo;
        }

        private void _LogAsync(object state)
        {
            //Log.StepIn();
            var o = (KeyValuePair<int, UnionPay>)state;
            if (o.Key == 0)
            {
                LogRequest.Invoke(o.Value);
            }
            else if (o.Key == 1)
            {
                LogResponse.Invoke(o.Value);
            }
            //Log.StepOut();
        }

        private string _GetInnerObjectString(string sourceStr, string key)
        {
            int keyStart = sourceStr.IndexOf(key);
            int keyEnd = keyStart + key.Length + 1;
            string pattern = @"\{((?<Open>\{)|(?<-Open>\})|[^{}]+)*(?(Open)(?!))\}";
            Regex regx = new Regex(pattern);
            MatchCollection mc = regx.Matches(sourceStr);
            foreach (Match m in mc)
            {
                if (m.Index == keyEnd)
                {
                    return m.Value;
                }
            }

            return "";
        }
        #endregion

        /// <summary>
        /// Get the {Token_string} from response and serialize it
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string TokenSerialize(string token)
        {
            UnionPayTokenPay tokenObj = token.Replace("{", "").Replace("}", "").ToDictionary().ToEntity<UnionPayTokenPay, string, string>();
            StringBuilder tokenFormat = new StringBuilder();
            tokenFormat.Append(tokenObj.Token);
            tokenFormat.Append(";");
            tokenFormat.Append(tokenObj.TrId);
            tokenFormat.Append(";");
            tokenFormat.Append(tokenObj.TokenType);

            StringBuilder tokenFormatSuffix = new StringBuilder();
            tokenFormatSuffix.Append(";");
            tokenFormatSuffix.Append(tokenObj.TokenLevel);
            tokenFormatSuffix.Append(";");
            tokenFormatSuffix.Append(tokenObj.TokenBegin);
            tokenFormatSuffix.Append(";");
            tokenFormatSuffix.Append(tokenObj.TokenEnd);

            return SecurityUtil.EncryptDataByPublicKey(tokenFormat.ToString()) + tokenFormatSuffix.ToString();
        }

        /// <summary>
        /// deserialize the {Token_string} from datatable and return the orginal string
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string TokenDeserialize(string token)
        {
            //eXMzTxOv84xcWU7WE+676K7r4ZYegXRvnfyI07gk6j/E9tRKXC+0OJlmVG7XFCgHW4loJJPG3Hd3nTW1PhwZwUXYHbTxmDvL5fhwNcKN4oxui3TVnf+NLWfUYlhsQsRDU3lLpuRZf2wK0IsLJ158VSNajmrQ34Y2m4oaKTzC3dQ=;80;20150528192850;20150826132143;
            if (!String.IsNullOrEmpty(token))
            {
                UnionPayTokenPay tokenObj = new UnionPayTokenPay();
                string[] segments = token.Split(';');
                if (segments.Count() == 4)
                {
                    string sensitiveData = SecurityUtil.DecryptDataByPrivateKey(segments[0]);
                    string[] sensitiveSegments = sensitiveData.Split(';');

                    tokenObj.TokenLevel = segments[1];
                    tokenObj.TokenBegin = segments[2];
                    tokenObj.TokenEnd = segments[3];
                    tokenObj.Token = sensitiveSegments[0];
                    tokenObj.TrId = sensitiveSegments[1];
                    tokenObj.TokenType = sensitiveSegments[2];
                }

                return tokenObj.ToKeyValueString();
            }
            else
            {
                return "";
            }
        }

        public static string TokenSerializeSegment(string token, int index)
        {
            string[] segments = token.Split(';');
            if (segments.Count() == 4)
            {
                if (index > 0)
                {
                    return segments[index];
                }
                else
                {
                    string sensitiveData = SecurityUtil.DecryptDataByPrivateKey(segments[0]);
                    return sensitiveData.Split(';')[index];
                }
            }
            return "";
        }

        /// <summary>
        /// Item1 is orderId, Item2 is orderTime
        /// </summary>
        /// <returns></returns>
        public static Tuple<string,string> GenerateTempOrder()
        {
            var t = new Tuple<string, string>(DateTime.Now.ToString("yyyyMMddHHmmss") + (new Random().Next(900) + 100).ToString().Trim(),
                DateTime.Now.ToString("yyyyMMddhhmmss"));
            return t;
        }

        public static string YuanToFen(object yuan)
        {
            return (yuan.ToDouble() * 100).ToLong().ToStr();
        }

        public static string FenToYuan(object fen)
        {
            return (fen.ToDouble() / 100).ToStr();
        }
    }
}
