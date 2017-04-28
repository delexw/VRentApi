using CF.VRent.Entities.PaymentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.UPSDK;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.KemasWrapper;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using CF.VRent.Common.Entities;
using CF.VRent.Log;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public class RetryStrategy:IRetryStrategy
    {
        private ProxyUserSetting _sessionUser;

        public RetryStrategy(ProxyUserSetting sessionUser)
        {
            _sessionUser = sessionUser;
        }

        public void Retry(IEnumerable<RetryBooking> booking)
        {
            //Indirect fee
            var indirectFeeFilters = booking
                .Where(r => r.Operation == PayOperationEnum.IndirectFeeDeduction);

            var rentalFeeFilters = booking.Where(r => r.Operation == PayOperationEnum.FeeDeduction);

            //Run parallelly
            Parallel.Invoke(() => {
                Parallel.ForEach(indirectFeeFilters, r => {
                    try
                    {
                        if (r.OldCard.HasValue)
                        {
                            _retryViaUnionPay(r);
                            _disableRetry(r);
                        }
                        else
                        {
                            LogInfor.WriteInfo(MessageCode.CVB000062.ToStr(),
                                String.Format(MessageCode.CVB000062.GetDescription(), r.UserID, r.PaymentId), _sessionUser.ID);
                        }
                    }
                    catch (WebFaultException<ReturnResult> ex)
                    {
                        LogInfor.WriteError(MessageCode.CVB000061.ToStr(), ex.Detail.ObjectToJson(), _sessionUser.ID);
                    }
                    catch (Exception ex)
                    {
                        LogInfor.WriteError(MessageCode.CVB000061.ToStr(), ex.ObjectToJson(), _sessionUser.ID);
                    }
                });
            }, () => {
                Parallel.ForEach(rentalFeeFilters, r =>
                {
                    try
                    {
                        if (r.OldCard.HasValue)
                        {
                            _retryViaUnionPay(r);
                            _disableRetry(r);
                        }
                        else
                        {
                            LogInfor.WriteInfo(MessageCode.CVB000062.ToStr(),
                                String.Format(MessageCode.CVB000062.GetDescription(), r.UserID, r.PaymentId), _sessionUser.ID);
                        }
                    }
                    catch (WebFaultException<ReturnResult> ex)
                    {
                        LogInfor.WriteError(MessageCode.CVB000061.ToStr(), ex.Detail.ObjectToJson(), _sessionUser.ID);
                    }
                    catch (Exception ex)
                    {
                        LogInfor.WriteError(MessageCode.CVB000061.ToStr(), ex.ObjectToJson(), _sessionUser.ID);
                    }
                });
            });
        }

        /// <summary>
        /// Disable the retry flag
        /// </summary>
        /// <param name="booking"></param>
        private void _disableRetry(RetryBooking booking)
        {
            var dataManager = new DataAccessProxyManager();
            dataManager.UpdatePaymentMessageExchangeRetry(booking.PaymentId, VRentDataDictionay.TransactionRetry.Default.GetValue(), _sessionUser.ID);
        }

        /// <summary>
        /// Retry do the payment
        /// </summary>
        /// <param name="booking"></param>
        private void _retryViaUnionPay(RetryBooking booking)
        {
            var daManager = new DataAccessProxyManager();

            var paymentManager = ServiceImpInstanceFactory.CreatePaymentInstance(_sessionUser);

            var user = KemasAccessWrapper.CreateKemasUserAPI2Instance().findUser2(booking.UserID.ToStr(), _sessionUser.SessionID);

            var proxyUser = new ProxyUserSetting()
            {
                VName = user.UserData.VName,
                Name = user.UserData.Name,
                Mail = user.UserData.Mail,
                ID = user.UserData.ID
            };

            var unionPayTempOrder = UnionPayUtils.GenerateTempOrder();

            switch (booking.State.ToStr().ToEnum<PaymentStatusEnum>())
            {
                //Retry deduction
                case PaymentStatusEnum.DeductionFailed:
                case PaymentStatusEnum.Deducting:
                case PaymentStatusEnum.DeductionRetryLongTime:
                case PaymentStatusEnum.DeductionRetryShortTime:
                    try
                    {
                        //Indirect fee
                        if (booking.Operation == PayOperationEnum.IndirectFeeDeduction)
                        {
                            var itemIds = booking.OrderItemId.Split(',');
                            List<int> intItemIds = new List<int>();
                            itemIds.ToList().ForEach(r =>
                            {
                                intItemIds.Add(r.ToInt());
                            });

                            daManager.DeductionOnce(PayOperationEnum.IndirectFeeDeduction.ToStr(),
                               booking.DeductionPrice,
                               booking.OldCard.ToStr(),
                               booking.BookingId,
                               intItemIds.ToArray(),
                               proxyUser.ToKeyValueString(false),
                               _sessionUser.ToKeyValueString(false),
                               "",
                               unionPayTempOrder.Item1,
                               unionPayTempOrder.Item2, true);
                        }
                        //Rental fee
                        else if (booking.Operation == PayOperationEnum.FeeDeduction)
                        {
                            daManager.DeductionOnce(PayOperationEnum.FeeDeduction.ToStr(),
                               booking.DeductionPrice,
                               booking.OldCard.ToStr(),
                               booking.BookingId,
                               null,
                               proxyUser.ToKeyValueString(false),
                               _sessionUser.ToKeyValueString(false),
                               "",
                               unionPayTempOrder.Item1,
                               unionPayTempOrder.Item2, true);
                        }
                    }
                    catch (WebFaultException<ReturnResult> ex)
                    {
                        paymentManager.BlockUser(_sessionUser, proxyUser, booking.DeductionPrice);
                        this._disableRetry(booking);
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        paymentManager.BlockUser(_sessionUser, proxyUser, booking.DeductionPrice);
                        this._disableRetry(booking);
                        throw ex;
                    }
                    break;
                //Retry preauth cancellation
                case PaymentStatusEnum.PreAuthCancelFailed:
                case PaymentStatusEnum.PreAuthCancelling:
                case PaymentStatusEnum.PreAuthCancelRetryLongTime:
                case PaymentStatusEnum.PreAuthCancelRetryShortTime:
                    try
                    {
                        daManager.CancelPreauthOnce(booking.PaymentId, booking.PreAuthPrice, proxyUser.ToKeyValueString(false), booking.BookingId);
                    }
                    catch (WebFaultException<ReturnResult> ex)
                    {
                        this._disableRetry(booking);
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        this._disableRetry(booking);
                        throw ex;
                    }
                    break;
                //Retry preauth completion
                case PaymentStatusEnum.PreAuthCompleteFailed:
                case PaymentStatusEnum.PreAuthCompleting:
                case PaymentStatusEnum.PreAuthCompleteRetryLongTime:
                case PaymentStatusEnum.PreAuthCompleteRetryShortTime:
                    try
                    {
                        daManager.FinishAndDeduction(
                            PayOperationEnum.FeeDeduction.ToStr(),
                            booking.RealPreAuthPrice.ToDouble() + booking.DeductionPrice.ToDouble(),
                            "",
                            booking.BookingId,
                            booking.UserID.ToStr(),
                            booking.PaymentId,
                            booking.OldCard.ToStr(),
                            proxyUser.ToKeyValueString(false),
                            _sessionUser.ToKeyValueString(false),
                            "",
                            "",
                            unionPayTempOrder.Item1,
                            unionPayTempOrder.Item2);
                    }
                    catch (WebFaultException<ReturnResult> ex)
                    {
                        paymentManager.BlockUser(_sessionUser, proxyUser, (booking.RealPreAuthPrice.ToDouble() + booking.DeductionPrice.ToDouble()).ToStr());
                        this._disableRetry(booking);
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        paymentManager.BlockUser(_sessionUser, proxyUser, (booking.RealPreAuthPrice.ToDouble() + booking.DeductionPrice.ToDouble()).ToStr());
                        this._disableRetry(booking);
                        throw ex;
                    }
                    break;
            }
        }
    }
}
