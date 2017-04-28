using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.UPSDK;
using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.DataAccessProxy.Payment.UnionPayProxy
{
    public interface ITransactionInvoker
    {
        /// <summary>
        /// Payment entity
        /// </summary>
        PaymentExchangeMessage Payment { get; }

        /// <summary>
        /// Message in a transaction
        /// </summary>
        UnionPayExchangeMessage TMessage { get; }

        /// <summary>
        /// Status Code
        /// </summary>
        ReturnResult TransactionResCode { get; }

        /// <summary>
        /// Get a flag showing current transction's state
        /// </summary>
        bool IsFailed();

        /// <summary>
        /// Preauthorization transaction process
        /// </summary>
        /// <returns></returns>
        ITransactionInvoker PreAuthTransaction(PaymentExchangeMessage preMsg);
        /// <summary>
        /// Call UnionPayInvoker to do preauth
        /// </summary>
        /// <returns></returns>
        ITransactionInvoker PreAuth();
        /// <summary>
        /// Send email when preauth is failed
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        ITransactionInvoker PreAuthEmail(ProxyUserSetting userInfo);

        /// <summary>
        /// Preauth cancelation process
        /// </summary>
        /// <param name="calMeg"></param>
        /// <param name="pys"></param>
        /// <param name="orginPay"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        ITransactionInvoker CancelTransaction(PaymentExchangeMessage calMeg, UnionPayExchangeMessage message);

        /// <summary>
        /// Call UnionPayInvoker to do cancellation
        /// </summary>
        /// <param name="callBackApiMethod"></param>
        /// <returns></returns>
        ITransactionInvoker Cancel(string callBackApiMethod);

        /// <summary>
        /// Send email when cancellation is failed
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        ITransactionInvoker CancelEmail(ProxyUserSetting userInfo);

        /// <summary>
        /// Deduction fee process for rental fee
        /// </summary>
        /// <param name="dedcutMsg"></param>
        /// <param name="pys"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        ITransactionInvoker DeductionTransaction(PaymentExchangeMessage dedcutMsg, UnionPayExchangeMessage message);
        /// <summary>
        /// Deduction fee procee for indirect fee
        /// </summary>
        /// <param name="dedcutMsg"></param>
        /// <param name="pys"></param>
        /// <param name="message"></param>
        /// <param name="orderItemIds"></param>
        /// <returns></returns>
        ITransactionInvoker DeductionTransaction(PaymentExchangeMessage dedcutMsg, UnionPayExchangeMessage message, int[] orderItemIds);
        /// <summary>
        /// Call UnionPayInvoker to do deduction
        /// </summary>
        /// <param name="bookingUserId"></param>
        /// <returns></returns>
        ITransactionInvoker Deduction(string bookingUserId);
        /// <summary>
        /// Send email when deduction is failed
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="opType"></param>
        /// <returns></returns>
        ITransactionInvoker DeductionEmail(ProxyUserSetting userInfo, PayOperationEnum opType);

        ITransactionInvoker CompleteTransaction(PaymentExchangeMessage finMeg, UnionPayExchangeMessage message);

        ITransactionInvoker Complete();

        ITransactionInvoker CompleteEmail(ProxyUserSetting userInfo);

        ITransactionInvoker CheckPaymentStatus(PaymentExchangeMessage checkPayment, string resCode);
    }
}
