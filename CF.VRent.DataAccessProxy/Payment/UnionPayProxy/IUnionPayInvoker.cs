using CF.VRent.Common.Entities;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.UPSDK;
using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.DataAccessProxy.Payment.UnionPayProxy
{
    /// <summary>
    /// Use UnionPaySDK to do work
    /// Added by Adam
    /// </summary>
    public interface IUnionPayInvoker
    {
        /// <summary>
        /// Get the synchronized response from UnionPay
        /// </summary>
        UnionPay Response { get; } 

        /// <summary>
        /// Do preauth with UnionPaySDK
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        ReturnResult PreAuthorizeViaUP(PaymentExchangeMessage message);

        /// <summary>
        /// Bind card with UnionPaySDK
        /// </summary>
        /// <param name="customInfo"></param>
        /// <param name="reservedMsg"></param>
        /// <returns></returns>
        ReturnResult AddUPBindingCardViaUP(UnionPayCustomInfo customInfo, string reservedMsg, Action callBack = null);

        /// <summary>
        /// Check payment status with UnionPaySDK
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        ReturnResult CheckPaymentStatusViaUP(PaymentExchangeMessage message, string resCode);

        /// <summary>
        /// Add transaction log
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        int Log(UnionPay entity, UnionPayEnum type);

        /// <summary>
        /// Send request to UnionPaySDK
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        string SendUnionPayRequest(Dictionary<string, string> param, string url);


        /// <summary>
        /// Unbind card with UnionPaySDK
        /// </summary>
        /// <returns></returns>
        ReturnResult CancelCardBindingViaUP();

        /// <summary>
        /// Cancel preauth with UnionPaySDK
        /// </summary>
        /// <param name="orginPreJson"></param>
        /// <param name="reservedMsg"></param>
        /// <param name="callBackApiMethod"></param>
        /// <returns></returns>
        ReturnResult CancelPreauthorizationViaUP(PaymentExchangeMessage message, string callBackApiMethod);

        /// <summary>
        /// Send request to get validation code with UnionPaySDK
        /// </summary>
        /// <param name="cardObjectJson"></param>
        /// <returns></returns>
        ReturnResult SendBindingSMSCodeViaUP(UnionPayCustomInfo cardObjectJson);

        /// <summary>
        /// Send request to get validation code with UnionPaySDK
        /// </summary>
        /// <param name="cardObjectJson"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        ReturnResult SendPreauthorizationSMSCodeViaUP(UnionPayCustomInfo cardObjectJson, PaymentExchangeMessage message);

        /// <summary>
        /// Finish preauth with UnionPaySDK
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        ReturnResult CompletePreauthorizaionViaUP(PaymentExchangeMessage message);

        /// <summary>
        /// Deduct fee with UnionPaySDK
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        ReturnResult DeductionViaUP(PaymentExchangeMessage message);
    }
}
