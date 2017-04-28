using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.DataAccessProxy.Payment;
using CF.VRent.DataAccessProxy.Payment.UnionPayProxy;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Email.EmailSender.Payment;
using CF.VRent.Log;
using CF.VRent.UPSDK;
using CF.VRent.UPSDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PaymentService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PaymentService.svc or PaymentService.svc.cs at the Solution Explorer and start debugging.
    public class PaymentService : IPaymentService
    {
        //private PaymentDAL _dal = new PaymentDAL();
        //private UnionPay _response = new UnionPay();

        /// <summary>
        /// Add a card
        /// </summary>
        /// <param name="cusWrapperBase64">a base64 key/value string of UnionPayCustomerInfo</param>
        /// <returns></returns>
        public int AddUPBindingCard(string cusWrapperBase64, string tokenBase64, string uid)
        {
            var cusObj = cusWrapperBase64.ToBase64Origin(Encoding.UTF8).ToDictionary().ToEntity<UnionPayCustomInfo, string, string>();

            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.AddCreditCardInfo(cusObj, tokenBase64.ToBase64Origin(Encoding.UTF8), uid));
        }


        /// <summary>
        /// Return user's credit cards
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public IEnumerable<PaymentCard> GetUserCreditCard(string uid)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<IEnumerable<PaymentCard>>(
                () => PaymentDAL.GetUserCreditCards(uid));
        }

        /// <summary>
        /// Return all bookings that have a retry flag
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RetryBooking> GetAllRetryBookings()
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<IEnumerable<RetryBooking>>(() => PaymentDAL.GetRetryBookings());
        }

        /// <summary>
        /// Log payment,e.g.request,response
        /// </summary>
        /// <param name="upWrapperBase64"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int LogPayment(string upWrapperBase64, string logType, string uid)
        {
            //Convert
            var up = upWrapperBase64.ToBase64Origin(Encoding.UTF8).ToDictionary().ToEntity<UnionPay, string, string>();

            return _logPayment(up, uid, logType.ToEnum<UnionPayEnum>());
        }

        /// <summary>
        /// Delete card from db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteUPBindingCard(string id, string uid)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.DeleteUserCreditCard(id, uid));
        }

        /// <summary>
        /// Get token of card
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string GetCardToken(string id, string uid)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<string>(
                () =>
                {
                    var eToken = PaymentDAL.GetUserCardToken(id, uid);
                    var entityToken = UnionPayUtils.TokenDeserialize(eToken).ToDictionary().ToEntity<UnionPayTokenPay, string, string>();
                    if (entityToken.TokenEnd.ToDateWithFormatter() < DateTime.Now)
                    {
                        //Disable the card
                        this.UpdateCreditCardState(id, VRentDataDictionay.UnionCardState.Disable.GetValue(), uid);
                        return null;
                    }
                    return eToken;
                });
        }


        public int AddPaymentMessageExchange(PaymentExchangeMessage message, string userId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.AddPaymentExchangeMessage(message, userId));
        }

        public int GetPaymentMessageState(string Id, string userId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.GetPaymentExchangeState(Id, userId));
        }


        public int UpdatePaymentMessageExchange(PaymentExchangeMessage message, string userId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.UpdatePaymentExchangeMessage(message, userId));
        }

        /// <summary>
        /// Update retry flag
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int UpdatePaymentMessageExchangeRetry(int paymentId, int retryFlag, string userId)
        {
            var upMessage = this.GetPaymentExchangeInfo(paymentId, userId);

            var originalStatus = upMessage.State.ToStr().ToEnum<PaymentStatusEnum>();

            //Update retry flag if the status is failed or processing 
            if (!originalStatus.IsSuccessStatus())
            {
                return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                               () => PaymentDAL.UpdatePaymentExchangeMessageRetry(new PaymentExchangeMessage()
                               {
                                   PaymentID = paymentId,
                                   Retry = retryFlag,
                                   UserID = userId
                               }));
            }
            return 0;
        }

        /// <summary>
        /// Update transaction status
        /// </summary>
        /// <param name="paymentStatus"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int UpdatePaymentMessageExchangeStatus(int paymentId, PaymentStatusEnum paymentStatus, string userId)
        {
            var upMessage = this.GetPaymentExchangeInfo(paymentId, userId);

            var originalStatus = upMessage.State.ToStr().ToEnum<PaymentStatusEnum>();

            if (originalStatus.IsFailedStatus())
            {
                return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                                              () => PaymentDAL.UpdatePaymentExchangeMessageState(new PaymentExchangeMessage()
                                              {
                                                  PaymentID = paymentId,
                                                  UserID = userId,
                                                  State = paymentStatus.GetValue(),
                                                  Operation = paymentStatus.ToStr()
                                              }));
            }

            return 0;
        }

        public PaymentExchangeMessage GetPaymentExchangeInfo(int Id, string userId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<PaymentExchangeMessage>(
                () => PaymentDAL.GetPaymentExchangeInfo(Id, userId));
        }

        public int UpdateCreditCardState(string cardId, int state, string userId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.UpdateCreditCardState(cardId, state, userId));
        }

        public int UpdateBookingStatusAfterPayment(string kmId, string state, string userId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.UpdateBookingStatusAfterPayment(kmId, state, userId));
        }

        public int GetPaymentStatusByBookingId(int bookingId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<int>(
                () => PaymentDAL.GetPaymentStatusByBookingId(bookingId));
        }

        public IEnumerable<PaymentExchangeMessage> GetFailedTransactionByBooking(int bookingId)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<IEnumerable<PaymentExchangeMessage>>(
                 () => PaymentDAL.GetFailedTransactionByBooking(bookingId));
        }


        public void AddPaymentExchangeMessageHistory(PaymentExchangeMessage message)
        {
            DataAccessProxyConstantRepo.DataAccessExceptionGuard(
                () => PaymentDAL.AddPaymentExchangeHistory(message));
        }

        private int _logPayment(UnionPay up, string userId, UnionPayEnum type)
        {
            return PaymentFactory.GetInstance<UnionPayInvoker>(userId).Log(up, type);
        }

        /// <summary>
        /// Bind card
        /// </summary>
        /// <param name="customInfoJson"></param>
        /// <param name="reservedMsg"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string AddUPBindingCardViaUP(string customInfoJson, string reservedMsg, string uid)
        {
            UnionPayCustomInfo cusObj = customInfoJson.JsonDeserialize<UnionPayCustomInfo>();

            var paymentInvoker = PaymentFactory.GetInstance<UnionPayInvoker>(uid);

            return paymentInvoker.AddUPBindingCardViaUP(cusObj, reservedMsg, () =>
            {
                this.AddUPBindingCard(cusObj.ToKeyValueString(false).ToBase64(Encoding.UTF8),
                    paymentInvoker.Response.TokenPayData.ToBase64(Encoding.UTF8), uid);

            }).ObjectToJson();
        }

        /// <summary>
        /// Check payment status
        /// </summary>
        /// <param name="resCode"></param>
        /// <param name="queryId"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public ReturnResult CheckPaymentStatusViaUP(string resCode, int paymentId, string uid)
        {
            var paymentMessage = this.GetPaymentExchangeInfo(paymentId, uid);
            paymentMessage.UserID = uid;

            var transaction =  PaymentFactory.GetTranInstance<TransactionInvoker>(uid)
                .CheckPaymentStatus(
                paymentMessage, resCode);

            return transaction.TransactionResCode;
        }

        /// <summary>
        /// Cancel card binding
        /// </summary>
        /// <param name="token"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string CancelCardBindingViaUP(string token, string uid)
        {
            return PaymentFactory.GetInstance<UnionPayInvoker>(uid, token).CancelCardBindingViaUP().ObjectToJson();
        }

        /// <summary>
        /// SendBinding sms code
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string SendBindingSMSCodeViaUP(string cardObjectJson, string uid)
        {
            UnionPayCustomInfo cardObject = cardObjectJson.JsonDeserialize<UnionPayCustomInfo>();

            return PaymentFactory.GetInstance<UnionPayInvoker>(uid).SendBindingSMSCodeViaUP(cardObject).ObjectToJson();
        }

        /// <summary>
        /// Send preauth sms 
        /// </summary>
        /// <param name="cardObjectJson"></param>
        /// <param name="price"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string SendPreauthorizationSMSCodeViaUP(string cardObjectJson, string price, string uid, string tempOrderId, string tempOrderTime)
        {
            UnionPayCustomInfo cardObject = cardObjectJson.JsonDeserialize<UnionPayCustomInfo>();


            return PaymentFactory.GetInstance<UnionPayInvoker>(uid, GetCardToken(cardObject.CardId, uid)).SendPreauthorizationSMSCodeViaUP(
                cardObject,
                new PaymentExchangeMessage()
                {
                    PreAuthPrice = price,
                    PreAuthDateTime = tempOrderTime,
                    PreAuthTempOrderID = tempOrderId
                }
                ).ObjectToJson();
        }


        /// <summary>
        /// Cancel and preauth transaction
        /// </summary>
        /// <param name="price"></param>
        /// <param name="cardId"></param>
        /// <param name="smsCode"></param>
        /// <param name="uid"></param>
        /// <param name="paymentUniqueID"></param>
        /// <param name="bookingId"></param>
        /// <param name="reservedMsg"></param>
        /// <param name="tempOrderId"></param>
        /// <param name="tempOrderTime"></param>
        public int CancelAndPreauthOnce(string price, string cardId, string smsCode, int paymentId, int bookingId,
            string bookingUserSetting,
            string tempOrderId = null, string tempOrderTime = null)
        {
            //Transcation
            TransactionOptions to = new TransactionOptions();
            to.IsolationLevel = IsolationLevel.ReadCommitted;

            var bookingUserSettingObj = bookingUserSetting.ToDictionary().ToEntity<ProxyUserSetting, string, string>();

            ITransactionInvoker transInvokerCancel = PaymentFactory.GetTranInstance<TransactionInvoker>(bookingUserSettingObj.ID);
            ITransactionInvoker transInvokerPreauth = PaymentFactory.GetTranInstance<TransactionInvoker>(bookingUserSettingObj.ID);

            TransactionUPManager.Run(() =>
            {
                //Params
                PaymentExchangeMessage calMeg = new PaymentExchangeMessage() { PaymentID = paymentId };
                PaymentExchangeMessage preMeg;
                //UnionPay orginPay = new UnionPay();
                var PaymentUniqueID = Guid.NewGuid().ToString();

                //Ready to cancel preauthorizaion
                var exchange = new UnionPayExchangeMessage()
                {
                    userId = bookingUserSettingObj.ID,
                    userName = bookingUserSettingObj.Name,
                    userVName = bookingUserSettingObj.VName,
                    userMail = bookingUserSettingObj.Mail
                };

                //var pys = new ProxyBookingPayment()
                //{
                //    UPPaymentID = paymentId,
                //    ProxyBookingID = bookingId
                //};

                transInvokerCancel = transInvokerCancel.CancelTransaction(calMeg, exchange);

                preMeg = new PaymentExchangeMessage()
                {
                    PaymentID = paymentId,
                    Operation = PaymentStatusEnum.PreAuthorizing.ToString(),
                    State = PaymentStatusEnum.PreAuthorizing.GetValue(),
                    CardID = cardId,
                    SmsCode = smsCode,
                    PreAuthDateTime = tempOrderTime,
                    PreAuthTempOrderID = tempOrderId,
                    PreAuthPrice = price,
                    ProxyBookingID = bookingId
                };

                transInvokerPreauth.PreAuthTransaction(preMeg);
            });

            try
            {
                //Cancel
                transInvokerCancel = transInvokerCancel.Cancel("ListenService/ListenCancelPreauthOfRedoPreauth");
                //Preauth
                transInvokerPreauth = transInvokerPreauth.PreAuth();
            }
            catch
            {
                if (transInvokerCancel.IsFailed())
                {
                    transInvokerCancel.CancelEmail(bookingUserSettingObj);
                }
                if (transInvokerPreauth.IsFailed())
                {
                    transInvokerPreauth.PreAuthEmail(bookingUserSettingObj);
                }
                throw;
            }

            return transInvokerPreauth.Payment.PaymentID;
        }


        /// <summary>
        /// Preauth transaction
        /// </summary>
        /// <param name="price"></param>
        /// <param name="cardId"></param>
        /// <param name="smsCode"></param>
        /// <param name="uid"></param>
        /// <param name="paymentID"></param>
        /// <param name="reservedMsg"></param>
        /// <param name="bookingUserSetting"></param>
        /// <param name="tempOrderId"></param>
        /// <param name="tempOrderTime"></param>
        public int PreauthOnce(string price, string cardId, string smsCode, string bookingUserSetting, int proxyBookingId = 0, string tempOrderId = null, string tempOrderTime = null)
        {
            ITransactionInvoker tranInvoker = null;
            var bookingUserSettingObj = bookingUserSetting.ToDictionary().ToEntity<ProxyUserSetting, string, string>();

            TransactionUPManager.Run(() =>
            {

                //Params
                PaymentExchangeMessage preMeg;
                UnionPay orginPay = new UnionPay();
                var PaymentUniqueID = Guid.NewGuid().ToString();

                tranInvoker = PaymentFactory.GetTranInstance<TransactionInvoker>(bookingUserSettingObj.ID);

                preMeg = new PaymentExchangeMessage()
                {
                    UniqueID = PaymentUniqueID,
                    CreatedOn = DateTime.Now,
                    Operation = PaymentStatusEnum.PreAuthorizing.ToString(),
                    State = PaymentStatusEnum.PreAuthorizing.GetValue(),
                    CardID = cardId,
                    SmsCode = smsCode,
                    PreAuthDateTime = tempOrderTime,
                    PreAuthTempOrderID = tempOrderId,
                    PreAuthPrice = price,
                    ProxyBookingID = proxyBookingId
                };
                //var pys = new ProxyBookingPayment()
                //{
                //    ProxyBookingID = proxyBookingId
                //};

                //Persist informantion with a db transaction around
                tranInvoker = tranInvoker.PreAuthTransaction(preMeg);
            });

            if (tranInvoker != null)
            {
                try
                {
                    tranInvoker = tranInvoker.PreAuth();
                    return tranInvoker.Payment.PaymentID;
                }
                catch
                {
                    if (tranInvoker.IsFailed())
                    {
                        tranInvoker.PreAuthEmail(bookingUserSettingObj);
                    }
                    throw;
                }
            }
            return 0;
        }

        /// <summary>
        /// Cancel preauth once
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="bookingUserSetting"></param>
        /// <returns></returns>
        public void CancelPreauthOnce(int paymentId, string price, string bookingUserSetting, int bookingId = 0)
        {
            //Transcation
            TransactionOptions to = new TransactionOptions();
            to.IsolationLevel = IsolationLevel.ReadCommitted;

            var bookingUserSettingObj = bookingUserSetting.ToDictionary().ToEntity<ProxyUserSetting, string, string>();

            ITransactionInvoker transInvoker = PaymentFactory.GetTranInstance<TransactionInvoker>(bookingUserSettingObj.ID);

            TransactionUPManager.Run(() =>
            {
                UnionPay orginPay = new UnionPay();
                //Params
                PaymentExchangeMessage calMeg = new PaymentExchangeMessage();

                //Ready to cancel preauthorizaion
                var exchange = new UnionPayExchangeMessage()
                {
                    priceTotal = price,
                    userName = bookingUserSettingObj.Name,
                    userVName = bookingUserSettingObj.VName,
                    userMail = bookingUserSettingObj.Mail
                };

                //Dictionary<string, string> message = new Dictionary<string, string>();
                //message.Add("priceTotal", price);
                //message.Add("userName", bookingUserSettingObj.Name);
                //message.Add("userVName", bookingUserSettingObj.VName);
                //message.Add("userMail", bookingUserSettingObj.Mail);

                //var pys = new ProxyBookingPayment()
                //{
                //    UPPaymentID = paymentId,
                //    ProxyBookingID = bookingId
                //};

                calMeg.PaymentID = paymentId;
                transInvoker = transInvoker.CancelTransaction(calMeg, exchange);
            });


            try
            {
                transInvoker = transInvoker.Cancel("ListenService/ListenCancelPreauth");
            }
            catch
            {
                if (transInvoker.IsFailed())
                {
                    transInvoker.CancelEmail(bookingUserSettingObj);
                }
                throw;
            }
        }

        /// <summary>
        /// Dedcution fee
        /// (curretly Indirect fee)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="price"></param>
        /// <param name="cardId"></param>
        /// <param name="bookingId"></param>
        /// <param name="orderItemIds"></param>
        /// <param name="bookingUserSetting"></param>
        /// <param name="userSetting"></param>
        /// <param name="userPwd"></param>
        /// <param name="tempOrderId"></param>
        /// <param name="tempOrderTime"></param>
        public void DeductionOnce(string type, string price, string cardId, int bookingId, int[] orderItemIds,
            string bookingUserSetting, string userSetting, string userPwd = "",
            string tempOrderId = null,
            string tempOrderTime = null, bool retry = false)
        {
            var bookingUserSettingObj = bookingUserSetting.ToDictionary().ToEntity<ProxyUserSetting, string, string>();
            var userSettingObj = userSetting.ToDictionary().ToEntity<ProxyUserSetting, string, string>();
            var operationType = (PayOperationEnum)Enum.Parse(typeof(PayOperationEnum), type);
            ITransactionInvoker transInvoker = PaymentFactory.GetTranInstance<TransactionInvoker>(userSettingObj.ID);

            TransactionUPManager.Run(() =>
            {
                //Dedcution obj
                PaymentExchangeMessage dedcutMsg;
                // BookingIndirectFeePayment bifp;
                var PaymentUniqueID = Guid.NewGuid().ToString();


                //Ready to dedction
                var exchange = new UnionPayExchangeMessage()
                {
                    type = type,
                    priceTotal = price,
                    userName = bookingUserSettingObj.Name,
                    userVName = bookingUserSettingObj.VName,
                    userMail = bookingUserSettingObj.Mail
                };

                //Add payment msg
                //Message
                dedcutMsg = new PaymentExchangeMessage()
                {
                    Operation = PaymentStatusEnum.PreDeduction.ToString(),
                    State = PaymentStatusEnum.PreDeduction.GetValue(),
                    CardID = cardId,
                    PreAuthDateTime = tempOrderTime,
                    PreAuthTempOrderID = tempOrderId,
                    DeductionPrice = price
                };

                //Indirect fee
                if (operationType == PayOperationEnum.IndirectFeeDeduction)
                {
                    if (retry)
                    {
                        var bookingPayments = new DataService().GetBookingIndirectFeePaymentByBookingID(bookingId);
                        //Get the first one, because the PaymentID of all orderItemIds are same
                        var orderItemId = orderItemIds[0];
                        var payment = bookingPayments.Where(r => r.State.ToStr().ToEnum<VRentDataDictionay.BookingIndirectFeePaymentState>() == VRentDataDictionay.BookingIndirectFeePaymentState.Enable &&
                            r.OrderItemID == orderItemId).FirstOrDefault();
                        if (payment != null)
                        {
                            dedcutMsg.PaymentID = payment.UPPaymentID;
                            dedcutMsg.ProxyBookingID = bookingId;
                        }
                    }
                    //bifp = new BookingIndirectFeePayment()
                    //{
                    //    BookingID = bookingId
                    //};
                    transInvoker = transInvoker.DeductionTransaction(dedcutMsg, exchange, orderItemIds);
                    //this._DeductionTransaction(ref bifp, orderItemIds, userSettingObj.ID, ref dedcutMsg, ref exchange);
                }
                //rental fee
                else if (operationType == PayOperationEnum.FeeDeduction)
                {
                    if (retry)
                    {
                        var bookingPayments = new DataService().GetBookingPaymentByBookingId(bookingId);
                        var payment = bookingPayments.Where(r => r.State.ToStr().ToEnum<VRentDataDictionay.BookingPaymentState>() == VRentDataDictionay.BookingPaymentState.Enable).FirstOrDefault();
                        if (payment != null)
                        {
                            dedcutMsg.PaymentID = payment.UPPaymentID;
                            dedcutMsg.ProxyBookingID = bookingId;
                            //var pys = new ProxyBookingPayment()
                            //{
                            //    ProxyBookingID = bookingId
                            //};

                            transInvoker = transInvoker.DeductionTransaction(dedcutMsg, exchange);
                            //this._DeductionTransaction(ref pys, userSettingObj.ID, ref dedcutMsg, ref exchange);
                        }
                    }
                }

            });

            try
            {
                transInvoker = transInvoker.Deduction(bookingUserSettingObj.ID);
            }
            catch
            {
                if (transInvoker.IsFailed())
                {
                    transInvoker.DeductionEmail(bookingUserSettingObj, operationType);
                }
                throw;
            }
        }

        /// <summary>
        /// Finish the preauthorization and dedcut fee once
        /// (currently rental fee and cancel fee)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="priceDetails"></param>
        /// <param name="kemasBookingId"></param>
        /// <param name="proxyBookingId"></param>
        /// <param name="bookingUserId"></param>
        /// <param name="paymentId"></param>
        /// <param name="cardId"></param>
        /// <param name="bookingUserSetting"></param>
        /// <param name="userSetting"></param>
        /// <param name="priceStructure"></param>
        /// <param name="userPwd"></param>
        public void FinishAndDeduction(string type,
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
            string kemasState = "")
        {
            var userSettingObj = userSetting.ToDictionary().ToEntity<ProxyUserSetting, string, string>();
            var bookingUserSettingObj = bookingUserSetting.ToDictionary().ToEntity<ProxyUserSetting, string, string>();
            var operationType = (PayOperationEnum)Enum.Parse(typeof(PayOperationEnum), type);
            var admingUserId = userSettingObj.ID;

            ITransactionInvoker complInvoker = PaymentFactory.GetTranInstance<TransactionInvoker>(admingUserId);
            ITransactionInvoker canclInvoker = PaymentFactory.GetTranInstance<TransactionInvoker>(admingUserId);
            ITransactionInvoker deductInvoker = PaymentFactory.GetTranInstance<TransactionInvoker>(admingUserId);

            TransactionUPManager.Run(() =>
            {

                var PaymentUniqueID = Guid.NewGuid().ToString();
                PaymentExchangeMessage finMeg = new PaymentExchangeMessage() { PaymentID = paymentId };
                //Ready to cancel preauthorizaion
                var exchange = new UnionPayExchangeMessage()
                {
                    type = type,
                    priceTotal = price.ToStr(),
                    kemasBookingId = kemasBookingId,
                    bookingId = proxyBookingId.ToStr(),
                    userId = bookingUserId,
                    userName = bookingUserSettingObj.Name,
                    userVName = bookingUserSettingObj.VName,
                    userMail = bookingUserSettingObj.Mail,
                    adminUserId = admingUserId,
                    kemasState = kemasState,
                    feeTypeDescription = ((PayOperationEnum)Enum.Parse(typeof(PayOperationEnum), type)).GetDescription()
                };

                //No additional fee
                if (price < 0)
                {
                    canclInvoker = canclInvoker.CancelTransaction(finMeg, exchange);
                }
                else
                {
                    finMeg.RealPreAuthPrice = price.ToStr();
                    finMeg.ProxyBookingID = proxyBookingId;

                    complInvoker = complInvoker.CompleteTransaction(finMeg, exchange);

                    //Try to deduct additional fee
                    if (complInvoker.Payment.State == PaymentStatusEnum.PreDeduction.GetValue())
                    {
                        deductInvoker = deductInvoker.DeductionTransaction(complInvoker.Payment, complInvoker.TMessage);
                    }
                }
            });

            //No additional fee
            if (price < 0)
            {
                try
                {
                    canclInvoker.Cancel("ListenService/ListenCancelPreauth");
                }
                catch
                {
                    //Failed, send email
                    if (canclInvoker.IsFailed())
                    {
                        canclInvoker.CancelEmail(bookingUserSettingObj);
                    }
                    throw;
                }
            }
            else
            {
                try
                {
                    complInvoker = complInvoker.Complete();
                }
                catch
                {
                    //Failed, send email
                    if (complInvoker.IsFailed())
                    {
                        complInvoker.CompleteEmail(bookingUserSettingObj);
                    }
                    throw;
                }
                //Try to deduct additional fee
                if (complInvoker.Payment.State == PaymentStatusEnum.PreDeduction.GetValue())
                {
                    try
                    {
                        deductInvoker.Deduction(bookingUserId);
                    }
                    catch
                    {
                        //Failed, send email
                        if (deductInvoker.IsFailed())
                        {
                            deductInvoker.DeductionEmail(bookingUserSettingObj, operationType);
                        }
                        throw;
                    }
                }
            }
        }

    }
}
