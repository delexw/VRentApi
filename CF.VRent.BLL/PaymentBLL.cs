using CF.VRent.Entities.PaymentService;
using CF.VRent.UPSDK.Entities;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.Net;
using CF.VRent.UPSDK;
using CF.VRent.UPSDK.SDK;
using System.Threading;
using System.Configuration;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using System.Collections.Specialized;
using CF.VRent.Entities;
using System.Globalization;
using System.ServiceModel;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Log;
using CF.VRent.Contract;
using CF.VRent.Entities.DataAccessProxy;
using PME = CF.VRent.Entities.PaymentService;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net.Mime;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.BLL.BLLFactory.Payment;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Common.UserContracts;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Email.EmailSender;
using CF.VRent.Email.EmailSender.Payment;
using System.Threading.Tasks;

namespace CF.VRent.BLL
{
    /// <summary>
    /// Business logic for unionpay payment
    /// </summary>
    public class PaymentBLL : AbstractBLL, IPayment
    {
        private DataAccessProxyManager client;
        private DataAccessProxyManager dclient;


        public string ReservedMessage { get; set; }

        //public const string UnionPayUtils.ReservedMessageKey2 = "UserId";

        public PaymentBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        {
            client = new DataAccessProxyManager();
            dclient = new DataAccessProxyManager();
            ReservedMessage = Guid.NewGuid().ToString();
        }
        public PaymentBLL()
            : this(null)
        {
        }

        /// <summary>
        /// Log payment
        /// </summary>
        /// <param name="upObj"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool LogPayment(UnionPay upObj, string uid, UnionPayEnum logType)
        {
            var r = client.LogPayment(upObj.ToKeyValueString(false).ToBase64(Encoding.UTF8), logType.GetValue().ToString(), uid);
            return r == 1 ? true : false;
        }

        /// <summary>
        /// Get user's all binding cards
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public IEnumerable<PaymentCard> GetUserCreditCard(string uid)
        {
            var cards = client.GetUserCreditCard(uid);

            if (cards != null)
            {
                List<PaymentCard> avaliableCards = new List<PaymentCard>();

                foreach (PaymentCard card in cards)
                {
                    //if card token is expired, exclude it from the array
                    var token = this.GetUserCardToken(card.CardId, uid);
                    if (!String.IsNullOrWhiteSpace(token))
                    {
                        avaliableCards.Add(card);
        }
                }

                return avaliableCards;
            }
            return new List<PaymentCard>();
        }

        /// <summary>
        /// Get the token of user's card
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string GetUserCardToken(string Id, string uid)
        {
            return client.GetCardToken(Id, uid);
        }

        /// <summary>
        /// Bind card and add card info into db
        /// </summary>
        /// <param name="cusObj">UnionPayCustomInfo</param>
        /// <param name="uid">UserID</param>
        /// <param name="returnUP">ResponseObj</param>
        /// <returns>CardId or null</returns>
        public string AddUPBindingCard(UnionPayCustomInfo cusObj, string uid, UnionPay returnUP = null, Action exceptionCallBack = null)
        {
            var exception = new ReturnResult()
            {
                Code = MessageCode.CVB000015.ToString(),
                Message = MessageCode.CVB000015.GetDescription(),
                Type = MessageCode.CVB000015.GetMessageType()
            };
            try
            {
                cusObj.CardId = Guid.NewGuid().ToString();
                //DMZ2
                var unionRet = client.AddUPBindingCardViaUP(cusObj.ObjectToJson(), this.ReservedMessage, uid).JsonDeserialize<ReturnResult>();
                if (unionRet.Success == 0)
                {
                    if (exceptionCallBack != null)
                    {
                        exceptionCallBack.Invoke();
                    }
                    if (unionRet.Code == "10" || unionRet.Code == "13" || unionRet.Code == "99")
                    {
                        throw new WebFaultException<ReturnResult>(exception, HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        throw new WebFaultException<ReturnResult>(unionRet, HttpStatusCode.BadRequest);
                    }
                }
            }
            catch(Exception ex)
            {
                LogInfor.WriteError(MessageCode.CVB000025.ToStr(), ex.ToStr(), uid);
                //error from da
                if (exceptionCallBack != null)
                {
                    exceptionCallBack.Invoke();
                }
                throw new WebFaultException<ReturnResult>(exception, HttpStatusCode.BadRequest);
            }

            return cusObj.CardId;
        }

        /// <summary>
        /// Pre-authorize
        /// </summary>
        /// <param name="price">pre price</param>
        /// <param name="token">card token</param>
        /// <param name="uid">UserID</param>
        /// <param name="returnUP">ResponseObj</param>
        /// <returns></returns>
        public Payment PreAuthorize(string price,
            string cardId,
            string smsCode,
            ProxyUserSetting user,
            int proxyBooking,
            string tempOrderId = null, string tempOrderTime = null)
        {
            var paymentId = client.PreauthOnce(price, cardId, smsCode, user.ToKeyValueString(false), proxyBooking, tempOrderId, tempOrderTime);

            var paymentSuccess = new Payment()
            {
                PaymentID = paymentId
            };

            return paymentSuccess;
        }


        /// <summary>
        /// Unbind card and delete card info from db
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool DeleteUPBindingCard(string cardId, string uid)
        {
            //var res = -1;

            //string token = "{" + this.GetUserCardToken(cardId, uid) + "}";

            //if (this.CancelCardBinding(token, uid))
            //{
            //    res = client.DeleteUPBindingCard(cardId, uid);
            //}

            return true;
        }

        /// <summary>
        /// Update exchange message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int UpdateExchangeMessage(PME.PaymentExchangeMessage message, string uid)
        {
            return client.UpdatePaymentMessageExchange(message, uid);
        }

        public int CheckPaymentStatus(string Id, string userId)
        {
            var state = client.GetPaymentMessageState(Id, userId);
            if (state == -2)
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000019.ToString(),
                    Message = MessageCode.CVB000019.GetDescription(),
                    Type = MessageCode.CVB000019.GetMessageType(),
                    MessageArgs = new object[] { "NULL", Id }
                }, HttpStatusCode.BadRequest);
            }
            return state;
        }

        /// <summary>
        /// Unbind card
        /// </summary>
        /// <param name="token"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool CancelCardBinding(string token, string uid)
        {
            var res = new ReturnResult();
            try
            {
                res = client.CancelCardBindingViaUP(token, uid).JsonDeserialize<ReturnResult>();
            }
            catch (Exception ex)
            {
                LogInfor.WriteError(MessageCode.CVB000025.ToStr(), ex.ToStr(), uid);
                return false;
            }

            if (res.Success == 1)
            {
                return true;
            }
            else
            {
                throw new WebFaultException<ReturnResult>(res, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Send card binding sms code request
        /// </summary>
        /// <param name="cardObject"></param>
        /// <param name="uid"></param>
        public Payment SendBindingSMSCode(UnionPayCustomInfo cardObject, string uid)
        {
            var res = new ReturnResult();

            try
            {
                res = client.SendBindingSMSCodeViaUP(cardObject.ObjectToJson(), uid).JsonDeserialize<ReturnResult>();
            }
            catch (Exception ex)
            {
                LogInfor.WriteError(MessageCode.CVB000025.ToStr(), ex.ToStr(), uid);
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000011.ToString(),
                    Message = MessageCode.CVB000011.GetDescription(),
                    Type = MessageCode.CVB000011.GetMessageType()
                }, HttpStatusCode.BadRequest);
            }

            if (res.Success == 0)
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000011.ToString(),
                    Message = MessageCode.CVB000011.GetDescription(),
                    Type = MessageCode.CVB000011.GetMessageType()
                }, HttpStatusCode.BadRequest);
            }

            return new Payment();
        }

        /// <summary>
        /// Get preauthorizaiton sms
        /// </summary>
        /// <param name="cardObject"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Payment SendPreauthorizationSMSCode(UnionPayCustomInfo cardObject, string price, string uid)
        {
            //Get phone
            var card = this.GetUserCreditCard(uid).Where(r => r.CardId == cardObject.CardId).FirstOrDefault();
            if ((card != null && card.PhoneNo.Trim() != cardObject.PhoneNo.Trim()) || card == null)
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000014.ToString(),
                    Message = MessageCode.CVB000014.GetDescription(),
                    Type = MessageCode.CVB000014.GetMessageType()
                }, HttpStatusCode.BadRequest);
            }

            var uptools = UnionPayUtils.GenerateTempOrder();
            var orderId = uptools.Item1;
            var orderTime = uptools.Item2;

            var res = new ReturnResult();
            try
            {
                res = client.SendPreauthorizationSMSCodeViaUP(cardObject.ObjectToJson(), price, uid, orderId, orderTime).JsonDeserialize<ReturnResult>();
            }
            catch (Exception ex)
            {
                LogInfor.WriteError(MessageCode.CVB000025.ToStr(), ex.ToStr(), uid);
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000011.ToString(),
                    Message = MessageCode.CVB000011.GetDescription(),
                    Type = MessageCode.CVB000011.GetMessageType()
                }, HttpStatusCode.BadRequest);
            }
            if (res.Success == 0)
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000011.ToString(),
                    Message = MessageCode.CVB000011.GetDescription(),
                    Type = MessageCode.CVB000011.GetMessageType()
                }, HttpStatusCode.BadRequest);
            }

            return new Payment() { TempOrderId = orderId, TempOrderTime = orderTime };
        }

        /// <summary>
        /// Get the preauthorization relative info
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public PME.PaymentExchangeMessage GetPaymentExchangeInfo(int Id, string uid)
        {
            return client.GetPaymentExchangeInfo(Id, uid);
        }

        /// <summary>
        /// Update the credit card
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="state"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int UpdateCreditCardStatus(string cardId, int state, string uid)
        {
            return client.UpdateCreditCardState(cardId, state, uid);
        }

        /// <summary>
        /// Cancel the credit card
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool CancelCreditCard(string cardId, string uid)
        {
            if (this.UpdateCreditCardStatus(cardId, VRentDataDictionay.UnionCardState.Disable.GetValue(), uid) == 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check whether the token is overdue or not
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool CheckCreditCardTokenAvailable(string cardId, string uid)
        {
            var token = client.GetCardToken(cardId, uid);

            if (null != token)
            {
                string endTime = UnionPayUtils.TokenSerializeSegment(token, 3);
                DateTime et;
                if (DateTime.TryParseExact(endTime.Trim(), "yyyyMMddHHmmss", null, DateTimeStyles.None, out et))
                {
                    if (DateTime.Now <= et)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Generator order after pay
        /// </summary>
        /// <param name="order"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int CreateBookingOrder(ProxyOrder order, string uid)
        {
            return dclient.AddOrderAfterPayment(order, uid);
        }

        public int UpdateBookingStatusAfterPayment(string kmId, string state, string userId)
        {
            return client.UpdateBookingStatusAfterPayment(kmId, state, userId);
        }

        public int GetPaymentStatusByBookingId(int bookingId)
        {
            var state = client.GetPaymentStatusByBookingId(bookingId);
            return state;
        }

        /// <summary>
        /// Get the payment status through up api
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public ReturnResult GetPaymentStatus(int paymentId, string uid)
        {
            //var paymentMessage = client.GetPaymentExchangeInfo(paymentId, uid);

            var result = client.CheckPaymentStatusViaUP("", paymentId, uid);

            return result;
        }

        /// <summary>
        /// Cancel Preauth
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="paymentId"></param>
        /// <param name="price"></param>
        /// <param name="bookingUserSetting"></param>
        /// <returns></returns>
        public bool CancelPreauth(int bookingId, int paymentId, string price,
            ProxyUserSetting bookingUserSetting)
        {
            if (bookingId > 0)
            {
                var bookingPayments = this.GetBookingPaymentByBookingID(bookingId.ToInt());
                if (bookingPayments.Count() > 0)
                {
                    var bookingPayment = bookingPayments
                        .Where(r => r.State == VRentDataDictionay.BookingPaymentState.Enable.GetValue())
                        .OrderByDescending(r => r.CreatedOn).FirstOrDefault();
                    if (bookingPayment != null)
                    {
                        paymentId = bookingPayment.UPPaymentID;
                    }
                    else
                    {
                        throw new WebFaultException<ReturnResult>(new ReturnResult()
                        {
                            Code = MessageCode.CVB000017.ToString(),
                            Message = MessageCode.CVB000017.GetDescription(),
                            Type = MessageCode.CVB000017.GetMessageType(),
                            MessageArgs = new object[] { bookingId, "NULL" }
                        }, HttpStatusCode.BadRequest);
                    }
                }
            }

            Task.Factory.StartNew(() => {
            client.CancelPreauthOnce(paymentId, price, bookingUserSetting.ToKeyValueString(false), bookingId);
            });
            
            return true;
        }

        /// <summary>
        /// Cancel Preauth
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="bookingUserSetting"></param>
        /// <returns></returns>
        public bool CancelPreauth(int bookingId, ProxyUserSetting bookingUserSetting)
        {
            if (bookingId.ToInt() <= 0)
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = ErrorConstants.BookingNodeExistCode,
                    Message = ErrorConstants.BookingNodeExistMessage + "(Booking:" + bookingId + ")",
                    Type = ResultType.VRENT
                }, HttpStatusCode.BadRequest);
            }
            var bookingPayments = this.GetBookingPaymentByBookingID(bookingId.ToInt());
            if (bookingPayments.Count() > 0)
            {
                var bookingPayment = bookingPayments.Where(r => r.State == VRentDataDictionay.BookingPaymentState.Enable.GetValue()).OrderByDescending(r => r.CreatedOn).FirstOrDefault();
                if (bookingPayment != null)
                {
                    var payment = this.GetPaymentExchangeInfo(bookingPayment.UPPaymentID, bookingUserSetting.ID);
                    client.CancelPreauthOnce(bookingPayment.UPPaymentID, payment.PreAuthPrice, bookingUserSetting.ToKeyValueString(false), bookingId);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Redo preauth
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="userId"></param>
        /// <param name="uSessionId"></param>
        /// <param name="price"></param>
        /// <param name="cardId"></param>
        /// <param name="smsCode"></param>
        /// <param name="tempOrderId"></param>
        /// <param name="tempOrderTime"></param>
        /// <returns></returns>
        public int RedoPreauth(int bookingId,
            string price,
            string cardId, string smsCode, ProxyUserSetting user,
            string tempOrderId = null, string tempOrderTime = null)
        {
            if (bookingId.ToInt() <= 0)
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = ErrorConstants.BookingNodeExistCode,
                    Message = ErrorConstants.BookingNodeExistMessage + "(Booking:" + bookingId + ")",
                    Type = ResultType.VRENT
                }, HttpStatusCode.BadRequest);
            }

            var bookingPayments = this.GetBookingPaymentByBookingID(bookingId.ToInt());

            if (bookingPayments.Count() > 0)
            {
                var bookingPayment = bookingPayments
                    .Where(r => r.State == VRentDataDictionay.BookingPaymentState.Enable.GetValue())
                    .OrderByDescending(r => r.CreatedOn).FirstOrDefault();
                if (bookingPayment != null)
                {
                    //if the booking has not associated transaction
                    //only do preauth
                    if (bookingPayment.UPPaymentID > 0)
                    {
                        client.CancelAndPreauthOnce(price, cardId, smsCode, bookingPayment.UPPaymentID, bookingId,
                             user.ToKeyValueString(false), tempOrderId, tempOrderTime);
                        return bookingPayment.UPPaymentID;
                    }
                    else
                    {
                        return this.PreAuthorize(price, cardId, smsCode, user, bookingId.ToInt(), tempOrderId, tempOrderTime).PaymentID;
                    }
                }
                else
                {
                    throw new WebFaultException<ReturnResult>(new ReturnResult()
                    {
                        Code = MessageCode.CVB000017.ToString(),
                        Message = MessageCode.CVB000017.GetDescription(),
                        Type = MessageCode.CVB000017.GetMessageType(),
                        MessageArgs = new object[] { bookingId, "NULL" }
                    }, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return client.PreauthOnce(price, cardId, smsCode, user.ToKeyValueString(false), bookingId.ToInt(), tempOrderId, tempOrderTime);
            }
        }

        /// <summary>
        /// Fee deduction (only by private)
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool FeeDeduction(int bookingId, ProxyUserSetting user)
        {
            var cards = this.GetUserCreditCard(user.ID);
            if (cards.Count() > 0)
            {
                //Cancel
                var payments = this.GetBookingPaymentByBookingID(bookingId);

                var payment = payments
                    .Where(r => r.State == VRentDataDictionay.BookingPaymentState.Enable.GetValue())
                    .OrderByDescending(r => r.CreatedOn).FirstOrDefault();

                if (payment != null)
                {
                    Task.Factory.StartNew(() => {
                    var cardId = cards.First().CardId;

                    KemasReservationAPI krapi = new KemasReservationAPI();
                    var bookingObj = dclient.RetrieveReservationByBookingID(bookingId);
                    var feelObj = krapi.getCancelReservationFees(bookingObj.KemasBookingID.ToString(), user.SessionID);

                        PrincingInfoFactory factory = new PrincingInfoFactory(feelObj.PriceDetails);
                        factory.Process();

                    var orderStruct = UnionPayUtils.GenerateTempOrder();

                    try
                    {
                        client.FinishAndDeduction(PayOperationEnum.CancelFeeDeduction.ToString(),
                                    factory.Price.Total.ToDouble(),
                                bookingObj.KemasBookingID.ToString(),
                                bookingObj.ProxyBookingID,
                                bookingObj.UserID.ToString(),
                                payment.UPPaymentID,
                                cardId, user.ToKeyValueString(false), user.ToKeyValueString(false), "", "", orderStruct.Item1, orderStruct.Item2);
                    }
                    catch (FaultException<ReturnResult> ex)
                    {
                        this.BlockUser(user, user, factory.Price.Total.ToStr());
                        throw ex;
                    }
                    });

                    return true;
                }
                else
                {
                    throw new WebFaultException<ReturnResult>(new ReturnResult()
                    {
                        Code = MessageCode.CVB000017.ToString(),
                        Message = MessageCode.CVB000017.GetDescription(),
                        Type = MessageCode.CVB000017.GetMessageType(),
                        MessageArgs = new object[] { bookingId, "NULL" }
                    }, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000016.ToString(),
                    Message = MessageCode.CVB000016.GetDescription(),
                    Type = MessageCode.CVB000016.GetMessageType()
                }, HttpStatusCode.BadRequest);
            }
        }

        public IEnumerable<ProxyBookingPayment> GetBookingPaymentByBookingID(int bookingId)
        {
            return dclient.GetBookingPaymentByBookingId(bookingId);
        }

        /// <summary>
        /// Get the completed bookings and trigger fee deduction
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void ScheduleJobCompletedBookings(ProxyUserSetting user, string userPwd)
        {
            try
            {
                var bookingSync = ServiceImpInstanceFactory.CreateBookingStatusSyncInstance(user);
                var getPriceDetail = ServiceImpInstanceFactory.CreatePriceDetailsInstance(user);

                KemasReservationAPI kri = new KemasReservationAPI();
                var bookings = dclient.GetWaitingPayDUBBookings();
                //Sync status
                bookings = bookingSync.BookingSync(bookings.ToList()).ToArray();
                //Filter dub/ccb booking
                var dubBooking = bookings.Where(r => r.BookingType == BookingType.Private.GetValue() && !String.IsNullOrWhiteSpace(r.CardID) && r.PaymentID > 0);
                var ccbBooking = bookings.Where(r => r.BookingType == BookingType.Business.GetValue());

                Parallel.Invoke(() => {
                #region CCB Bookings
                //CCB booking
                    Parallel.ForEach<ProxyReservationPayment>(ccbBooking, pr => {
                    try
                    {
                        if (BookingUtility.TransformToProxyBookingState("completed") == pr.KemasState ||
                                BookingUtility.TransformToProxyBookingState("autocanceled") == pr.KemasState)
                        {
                            var priceObj = getPriceDetail.Get(pr.KemasBookingID, pr.ProxyBookingID);
                            pr.PriceStructure = priceObj.ObjectToJson();
                            pr.PriceDetials = getPriceDetail.PriceDetails;
                            //Record price details
                            var result = dclient.SavingPrincingItems(priceObj);
                            if (result.ID <= 0)
                            {
                                LogInfor.WriteError(MessageCode.CVB000053.ToStr(), String.Format("BookingNumber:{0},PriceXml:{1},PriceObject:{2}",
                                    pr.KemasBookingNumber, pr.PriceDetials, pr.PriceStructure), user.ID);
                            }

                            //Add order
                            this.CreateBookingOrder(new ProxyOrder()
                            {
                                ProxyBookingID = pr.ProxyBookingID,
                                BookingUserID = pr.UserID,
                                State = CommonState.Active.GetValue(),
                                CreatedBy = user.ID.ToGuidNull()
                            }, user.ID);
                        }
                        }
                        catch (FaultException<ReturnResult> ex)
                        {
                            LogInfor.WriteInfo("(Direct Fee)Booking", pr.ObjectToJson(), user.ID);
                            LogInfor.WriteError(MessageCode.CVB000054.ToStr(), ex.Detail.ObjectToJson(), user.ID);
                    }
                    catch (Exception ex)
                    {
                        LogInfor.WriteInfo("(Direct Fee)CCB Booking", pr.ObjectToJson(), user.ID);
                        LogInfor.WriteError(MessageCode.CVB000054.ToStr(), ex.ToString(), user.ID);
                    }
                    });
                #endregion
                }, () => {
                #region DUB Bookings
                    Parallel.ForEach<ProxyReservationPayment>(dubBooking, pr => {
                    try
                    {
                        if (BookingUtility.TransformToProxyBookingState("completed") == pr.KemasState ||
                            BookingUtility.TransformToProxyBookingState("autocanceled") == pr.KemasState)
                        {
                            var paymentId = pr.PaymentID;

                            var priceObj = getPriceDetail.Get(pr.KemasBookingID, pr.ProxyBookingID);
                            pr.PriceStructure = priceObj.ObjectToJson();
                            pr.PriceDetials = getPriceDetail.PriceDetails;

                                PrincingInfoFactory factory = new PrincingInfoFactory(pr.PriceDetials);
                                factory.Process();

                            KemasUserAPI api = new KemasUserAPI();
                            var kemasUser = api.findUser2(pr.UserID, user.SessionID);
                            var bookingUserObj = new ProxyUserSetting()
                            {
                                ID = kemasUser.UserData.ID,
                                VName = kemasUser.UserData.VName,
                                Name = kemasUser.UserData.Name,
                                Mail = kemasUser.UserData.Mail
                            };

                            var orderStruct = UnionPayUtils.GenerateTempOrder();

                            try
                            {
                                dclient.FinishAndDeduction(PayOperationEnum.FeeDeduction.ToString(),
                                        factory.Price.Total.ToDouble(),
                                    pr.KemasBookingID,
                                    pr.ProxyBookingID,
                                    pr.UserID,
                                    pr.PaymentID,
                                    pr.CardID,
                                    bookingUserObj.ToKeyValueString(false),
                                    user.ToKeyValueString(false),
                                    pr.PriceStructure,
                                    userPwd, orderStruct.Item1, orderStruct.Item2, pr.KemasState);
                            }
                                catch (FaultException<ReturnResult> ex)
                            {
                                    LogInfor.WriteInfo("(Direct Fee)Booking", pr.ObjectToJson(), user.ID);
                                    LogInfor.WriteError(MessageCode.CVB000054.ToStr(), ex.Detail.ObjectToJson(), user.ID);
                                //Block user
                                this.BlockUser(user, bookingUserObj, getPriceDetail.PriceTotal);
                                throw ex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogInfor.WriteInfo("(Direct Fee)DUB Booking", pr.ObjectToJson(), user.ID);
                        LogInfor.WriteError(MessageCode.CVB000054.ToStr(), ex.ToString(), user.ID);
                    }
                    });
                #endregion
                });
            }
            catch (Exception ex)
            {
                LogInfor.WriteError(MessageCode.CVB000054.ToStr(), ex.ToString(), user.ID);
            }
        }

        /// <summary>
        /// Block user
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="users"></param>
        public void BlockUser(ProxyUserSetting admin, ProxyUserSetting bookingUser, string price)
        {
            //Block
            UserFactory uf = new UserFactory();

            IUserBlocker blocker = ServiceImpInstanceFactory.CreateUserBlockInstance(uf.CreateEntity(admin), uf.CreateEntity(bookingUser));

            blocker.DeactiveDUB();

            //Send email
            IFeeDeductionFailedSender emailSender = EmailSenderFactory.CreateFeeDeductionFailedSender();
            emailSender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
            {
                this.SendPaymentEmail(arg1, arg2, arg3);
            };

            emailSender.Send(new EmailParameterEntity(){
                FirstName = bookingUser.VName,
                LastName = bookingUser.Name,
                Price = price
            }, bookingUser.Mail);
        }


        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="bookRes"></param>
        /// <param name="user"></param>
        public void SendPaymentEmail(EmailParameterEntity paras, EmailType etype, params string[] emailAddress)
        {
            dclient.SendPaymentEmail(paras, etype.ToString(), emailAddress);
        }

        /// <summary>
        /// Get all bookings and their unpaid indirect fee and trigger indirect fee deduction
        /// </summary>
        /// <returns></returns>
        public void ScheduleJobIndirectFeeBookings(ProxyUserSetting user, string userPwd)
        {
            try
            {
                var indirectFees = dclient.GetTotalIndirectFeeForAll();
                //only for business and private 
                Parallel.ForEach<BookingIndirectFee>(indirectFees, pr => {
                    try
                    {
                        KemasUserAPI api = new KemasUserAPI();
                        var kemasUser = api.findUser2(pr.UserID, user.SessionID);
                        var bookingUserObj = new ProxyUserSetting()
                        {
                            ID = kemasUser.UserData.ID,
                            VName = kemasUser.UserData.VName,
                            Name = kemasUser.UserData.Name,
                            Mail = kemasUser.UserData.Mail
                        };

                        var orderStruct = UnionPayUtils.GenerateTempOrder();

                        try
                        {
                            client.DeductionOnce(PayOperationEnum.IndirectFeeDeduction.ToString(),
                                pr.Fee.ToStr(),
                                pr.CardID,
                                pr.BookingID,
                                pr.OrderItemIDs,
                                bookingUserObj.ToKeyValueString(false),
                                user.ToKeyValueString(false),
                                userPwd, orderStruct.Item1, orderStruct.Item2);
                        }
                        catch (FaultException<ReturnResult> ex)
                        {
                            //Block user
                            this.BlockUser(user, bookingUserObj, pr.Fee.ToStr());
                            throw ex;
                        }
                    }
                    catch (FaultException<ReturnResult> ex)
                    {
                        LogInfor.WriteInfo("(Indirect Fee)Booking", pr.ObjectToJson(), user.ID);
                        LogInfor.WriteError(MessageCode.CVB000055.ToStr(), ex.Detail.ObjectToJson(), user.ID);
                    }
                    catch (Exception ex)
                    {
                        LogInfor.WriteInfo("(Indirect Fee)Booking", pr.ObjectToJson(), user.ID);
                        LogInfor.WriteError(MessageCode.CVB000055.ToStr(), ex.ToString(), user.ID);
                    }
                });
                 
            }
            catch (Exception ex)
            {
                LogInfor.WriteError(MessageCode.CVB000055.ToStr(), ex.ToString(), user.ID);
            }
        }


        public void AddPaymentExchangeMessageHistory(PaymentExchangeMessage message)
        {
            client.AddPaymentExchangeMessageHistory(message);
        }
    }
}