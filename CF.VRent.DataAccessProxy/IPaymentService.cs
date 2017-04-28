using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CF.VRent.UPSDK;
using CF.VRent.UPSDK.Entities;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPaymentService" in both code and config file together.
    [ServiceContract]
    public interface IPaymentService
    {
        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int LogPayment(string upWrapperBase64, string logType, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int AddUPBindingCard(string cusWrapperBase64, string tokenBase64, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        IEnumerable<PaymentCard> GetUserCreditCard(string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int DeleteUPBindingCard(string id, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        string GetCardToken(string id, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int AddPaymentMessageExchange(PaymentExchangeMessage message, string userId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int GetPaymentMessageState(string Id, string userId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int UpdatePaymentMessageExchange(PaymentExchangeMessage message, string userId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int UpdatePaymentMessageExchangeRetry(int paymentId, int retryFlag, string userId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int UpdatePaymentMessageExchangeStatus(int paymentId,PaymentStatusEnum paymentStatus, string userId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        PaymentExchangeMessage GetPaymentExchangeInfo(int Id, string userId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        IEnumerable<RetryBooking> GetAllRetryBookings();

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int UpdateCreditCardState(string cardId, int state, string userId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int UpdateBookingStatusAfterPayment(string kmId, string state, string userId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int GetPaymentStatusByBookingId(int bookingId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        string AddUPBindingCardViaUP(string customInfoJson, string reservedMsg, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        ReturnResult CheckPaymentStatusViaUP(string resCode,int paymentId, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        string CancelCardBindingViaUP(string token, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        string SendBindingSMSCodeViaUP(string cardObjectJson, string uid);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        string SendPreauthorizationSMSCodeViaUP(string cardObjectJson, string price, string uid, string tempOrderId, string tempOrderTime);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int CancelAndPreauthOnce(string price,
            string cardId,
            string smsCode,
            int paymentId,
            int bookingId,
            string bookingUserSetting,
            string tempOrderId = null,
            string tempOrderTime = null);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        int PreauthOnce(string price,
            string cardId,
            string smsCode,
            string bookingUserSetting,
            int proxyBookingId = 0,
            string tempOrderId = null,
            string tempOrderTime = null);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        void CancelPreauthOnce(int paymentId, string price, string bookingUserSetting, int bookingId = 0);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        void DeductionOnce(string type, string price, string cardId,
            int bookingId, int[] orderItemIds, string bookingUserSetting, string userSetting, string userPwd = "", string tempOrderId = null,
            string tempOrderTime = null, bool retry = false);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        void FinishAndDeduction(
            string type,
            double price,
            string kemasBookingId,
            int proxyBookingId,
            string bookingUserId,
            int paymentId,
            string cardId,
            string bookingUserSetting,
            string userSetting,
            string priceStructure = "",
            string userPwd = "",
            string tempOrderId = null,
            string tempOrderTime = null,
            string kemasState = "");

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        IEnumerable<PaymentExchangeMessage> GetFailedTransactionByBooking(int bookingId);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        void AddPaymentExchangeMessageHistory(PaymentExchangeMessage message);
    }
}
