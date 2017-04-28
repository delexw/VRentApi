using CF.VRent.Common.Entities;
using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UPSDK
{
    public interface IUnionPayUtils : IUnionPayUtilsMethod
    {
        /// <summary>
        /// Log request delegation
        /// </summary>
        Func<UnionPay, bool> LogRequest { get; set; }

        /// <summary>
        /// Log response delegation
        /// </summary>
        Func<UnionPay, bool> LogResponse { get; set; }

        /// <summary>
        /// Log payment success information delegation 
        /// </summary>
        Func<UnionPay, bool> LogMerInform { get; set; }

        /// <summary>
        /// Response Obj
        /// </summary>
        UnionPay UnionPayResponse { get; }

        /// <summary>
        /// Mer Information Obj
        /// </summary>
        UnionPay UnionPayMerInform { get; }

        /// <summary>
        /// Card Info
        /// </summary>
        UnionPayCustomInfo UPCustomerInfo
        { get; set; }

        /// <summary>
        /// Token Info
        /// </summary>
        UnionPayTokenPay UPTokenPay { get; set; }

        /// <summary>
        /// Retry strategy Info
        /// </summary>
        UnionPayState UPStateTime { get; set; }

        // <summary>
        /// BackUrl
        /// </summary>
        string BackUrl { get; set; }

        /// <summary>
        /// TxnAmt
        /// </summary>
        string TxnAmt
        { get; set; }

        /// <summary>
        /// CurrencyCode
        /// </summary>
        string CurrencyCode { get; set; }

        /// <summary>
        /// OrderId
        /// </summary>
        string OrderId { get; set; }

        /// <summary>
        /// TxnTime
        /// </summary>
        string TxnTime { get; set; }

        /// <summary>
        /// OrigQryId
        /// </summary>
        string OrigQryId { get; set; }

        /// <summary>
        /// (Keep the value unique in every transaction of unionpay request)
        /// </summary>
        string ReqReserved { get; set; }

        /// <summary>
        /// Send request to up
        /// </summary>
        Func<Dictionary<string, string>, string, string> SendRequestProxy { get; set; }



    }
}
