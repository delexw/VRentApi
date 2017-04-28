using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.PaymentService;
using CF.VRent.UPSDK;
using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PME = CF.VRent.Entities.PaymentService;

namespace CF.VRent.Contract
{
    public interface IPayment
    {
        bool LogPayment(UnionPay upObj, string uid, UnionPayEnum logType);

        IEnumerable<PaymentCard> GetUserCreditCard(string uid);

        string GetUserCardToken(string Id, string uid);

        string AddUPBindingCard(UnionPayCustomInfo cusObj, string uid, UnionPay returnUP = null, Action exceptionCallBack = null);

        Payment PreAuthorize(string price,
            string cardId,
            string smsCode,
            ProxyUserSetting user,
            int proxyBooking,
            string tempOrderId = null, string tempOrderTime = null);

        bool DeleteUPBindingCard(string cardId, string uid);

        int UpdateExchangeMessage(PME.PaymentExchangeMessage message, string uid);

        int CheckPaymentStatus(string Id, string userId);

        bool CancelCardBinding(string token, string uid);

        Payment SendBindingSMSCode(UnionPayCustomInfo cardObject, string uid);

        Payment SendPreauthorizationSMSCode(UnionPayCustomInfo cardObject, string price, string uid);

        PME.PaymentExchangeMessage GetPaymentExchangeInfo(int Id, string uid);

        int UpdateCreditCardStatus(string cardId, int state, string uid);

        bool CancelCreditCard(string cardId, string uid);

        bool CheckCreditCardTokenAvailable(string cardId, string uid);

        int CreateBookingOrder(ProxyOrder order, string uid);

        int UpdateBookingStatusAfterPayment(string kmId, string state, string userId);

        int GetPaymentStatusByBookingId(int bookingId);

        ReturnResult GetPaymentStatus(int paymentId, string uid);

        bool CancelPreauth(int bookingId, int paymentId, string price,
            ProxyUserSetting bookingUserSetting);

        bool CancelPreauth(int bookingId, ProxyUserSetting bookingUserSetting);

        int RedoPreauth(int bookingId,
            string price,
            string cardId, string smsCode, ProxyUserSetting user,
            string tempOrderId = null, string tempOrderTime = null);

        bool FeeDeduction(int bookingId, ProxyUserSetting user);

        IEnumerable<ProxyBookingPayment> GetBookingPaymentByBookingID(int bookingId);

        void ScheduleJobCompletedBookings(ProxyUserSetting user, string userPwd);

        void BlockUser(ProxyUserSetting admin, ProxyUserSetting bookingUser, string price);

        void SendPaymentEmail(EmailParameterEntity paras, EmailType etype, params string[] emailAddress);

        void ScheduleJobIndirectFeeBookings(ProxyUserSetting user, string userPwd);

        void AddPaymentExchangeMessageHistory(PaymentExchangeMessage message);
    }
}
