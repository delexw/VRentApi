using CF.VRent.Common.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.PaymentService;
using CF.VRent.UPSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface ITransaction : IBLL
    {
        /// <summary>
        /// Get all bookings that have a retry flag
        /// </summary>
        /// <returns></returns>
        IEnumerable<RetryBooking> GetPreRetryBookings();
        /// <summary>
        /// Retry do transaction
        /// </summary>
        void RetryTransactionOfBookings();

        /// <summary>
        /// Enable retry flag
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        PaymentExchangeMessage UpdateExchangeMessageEnableRetry(int paymentId);

        /// <summary>
        /// Disable retry flag
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        PaymentExchangeMessage UpdateExchangeMessageDisableRetry(int paymentId);

        /// <summary>
        /// Modify status
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        PaymentExchangeMessage UpdateExchangeMessageStatus(int paymentId, PaymentStatusEnum status);

        /// <summary>
        /// Enable retry flag
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        ReturnResult UpdateExchangeMessageEnableRetryByBooking(int bookingId);

        /// <summary>
        /// Disable retry flag
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        ReturnResult UpdateExchangeMessageDisableRetryByBooking(int bookingId);
    }
}
