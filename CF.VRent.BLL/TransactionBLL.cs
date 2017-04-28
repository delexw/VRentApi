using CF.VRent.BLL.BLLFactory;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.PaymentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.UPSDK;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.DataAccessProxyWrapper;
using System.ServiceModel.Web;
using System.Net;
using CF.VRent.Log;
using System.Threading.Tasks;

namespace CF.VRent.BLL
{
    public class TransactionBLL : AbstractBLL, ITransaction
    {

        public TransactionBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        { }

        public IEnumerable<RetryBooking> GetPreRetryBookings()
        {
            return new DataAccessProxyManager().GetAllRetryBookings();
        }

        public void RetryTransactionOfBookings()
        {
            var bookings = this.GetPreRetryBookings();
            var retry = ServiceImpInstanceFactory.CreateRetryInstance(this.UserInfo);
            retry.Retry(bookings);
        }

        /// <summary>
        /// Enable the retry flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public PaymentExchangeMessage UpdateExchangeMessageEnableRetry(int paymentId)
        {
            if (paymentId > 0)
            {
                var payment = ServiceImpInstanceFactory.CreatePaymentInstance(this.UserInfo);
                var dataManager = new DataAccessProxyManager();

                var upMessage = payment.GetPaymentExchangeInfo(paymentId, this.UserInfo.ID);

                var originalStatus = upMessage.State.ToStr().ToEnum<PaymentStatusEnum>();

                //Update retry flag if the status is failed or processing 
                if (!originalStatus.IsSuccessStatus())
                {
                    dataManager.UpdatePaymentMessageExchangeRetry(paymentId, VRentDataDictionay.TransactionRetry.Retry.GetValue(), this.UserInfo.ID);
                    return payment.GetPaymentExchangeInfo(paymentId, this.UserInfo.ID);
                }
                else
                {
                    throw new WebFaultException<ReturnResult>(new ReturnResult()
                    {
                        Code = MessageCode.CVB000061.ToStr(),
                        Message = MessageCode.CVB000061.GetDescription(),
                        Type = MessageCode.CVB000061.GetMessageType(),
                        MessageArgs = new object[] { paymentId, upMessage.Operation }
                    }, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000019.ToStr(),
                    Message = MessageCode.CVB000019.GetDescription(),
                    Type = MessageCode.CVB000019.GetMessageType(),
                    MessageArgs = new object[] { "", paymentId }
                }, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Disable the retry flag
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public PaymentExchangeMessage UpdateExchangeMessageDisableRetry(int paymentId)
        {
            if (paymentId > 0)
            {
                var payment = ServiceImpInstanceFactory.CreatePaymentInstance(this.UserInfo);
                var dataManager = new DataAccessProxyManager();
                dataManager.UpdatePaymentMessageExchangeRetry(paymentId, VRentDataDictionay.TransactionRetry.Default.GetValue(), this.UserInfo.ID);
                return payment.GetPaymentExchangeInfo(paymentId, this.UserInfo.ID);
            }
            else
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000019.ToStr(),
                    Message = MessageCode.CVB000019.GetDescription(),
                    Type = MessageCode.CVB000019.GetMessageType(),
                    MessageArgs = new object[] { "", paymentId }
                }, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Modified the transaction status
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="status"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public PaymentExchangeMessage UpdateExchangeMessageStatus(int paymentId, PaymentStatusEnum status)
        {
            var payment = ServiceImpInstanceFactory.CreatePaymentInstance(this.UserInfo);
            var dataManager = new DataAccessProxyManager();

            var message = payment.GetPaymentExchangeInfo(paymentId, this.UserInfo.ID);

            //only when current status is belong to failed status group
            if (message.State.ToStr().ToEnum<PaymentStatusEnum>().IsFailedStatus())
            {
                dataManager.UpdatePaymentMessageExchangeStatus(paymentId, status, this.UserInfo.ID);
                return payment.GetPaymentExchangeInfo(paymentId, this.UserInfo.ID);
            }
            else
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000060.ToStr(),
                    Message = MessageCode.CVB000060.GetDescription(),
                    Type = MessageCode.CVB000060.GetMessageType(),
                    MessageArgs = new object[] { paymentId, message.Operation }
                }, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Enable the retry flag by booking
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        public ReturnResult UpdateExchangeMessageEnableRetryByBooking(int bookingId)
        {
            if (bookingId > 0)
            {
                var dataManager = new DataAccessProxyManager();
                Parallel.ForEach<PaymentExchangeMessage>(dataManager.GetFailedTransactionByBooking(bookingId).Where(r => r.Retry == VRentDataDictionay.TransactionRetry.Default.GetValue()), r =>
                {
                    this.UpdateExchangeMessageEnableRetry(r.PaymentID);
                });
            }

            return new ReturnResult(false) { Success = 1 };
        }

        /// <summary>
        /// Disable the retry flag by booking
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        public ReturnResult UpdateExchangeMessageDisableRetryByBooking(int bookingId)
        {
            if (bookingId > 0)
            {
                var dataManager = new DataAccessProxyManager();
                Parallel.ForEach<PaymentExchangeMessage>(dataManager.GetFailedTransactionByBooking(bookingId).Where(r => r.Retry == VRentDataDictionay.TransactionRetry.Retry.GetValue()), r =>
                {
                    this.UpdateExchangeMessageDisableRetry(r.PaymentID);
                });
            }

            return new ReturnResult(false) { Success = 1 };
        }
    }
}
