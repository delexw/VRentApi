using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CF.VRent.Common;
using System.Transactions;
using CF.VRent.Common.Entities;
using CF.VRent.DAL;
using CF.VRent.UPSDK;
using System.ServiceModel;
using System.Threading.Tasks;
using CF.VRent.Email.EmailSender.Payment;
using CF.VRent.Email.EmailSender;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Log;
using CF.VRent.UPSDK.Entities;
using System.Threading;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Factory;

namespace CF.VRent.DataAccessProxy.Payment.UnionPayProxy
{
    public class TransactionInvoker : ITransactionInvoker
    {
        public PaymentExchangeMessage Payment
        {
            get;
            private set;
        }

        public ReturnResult TransactionResCode
        {
            get;
            private set;
        }

        public UnionPayExchangeMessage TMessage
        {
            get { return _exchangeMessage; }
        }

        private string _uid;

        private UnionPayExchangeMessage _exchangeMessage;

        public TransactionInvoker(string uid)
        {
            _uid = uid;
        }

        public ITransactionInvoker PreAuthTransaction(PaymentExchangeMessage preMsg)
        {
            TransactionUPManager.RunPart(() =>
            {
                var addNewRelation = false;
                if (preMsg.PaymentID == 0)
                {
                    preMsg.UniqueID = Guid.NewGuid().ToStr();
                    preMsg.CreatedOn = DateTime.Now;
                    preMsg.PaymentID = ServiceInstanceSingleton.PaymentService.AddPaymentMessageExchange(preMsg, _uid);
                    addNewRelation = true;
                }
                else
                {
                    var originalMsg = ServiceInstanceSingleton.PaymentService.GetPaymentExchangeInfo(preMsg.PaymentID, _uid);
                    originalMsg.Operation = preMsg.Operation;
                    originalMsg.State = preMsg.State;
                    originalMsg.PreAuthDateTime = preMsg.PreAuthDateTime;
                    originalMsg.PreAuthTempOrderID = preMsg.PreAuthTempOrderID;
                    originalMsg.PreAuthPrice = preMsg.PreAuthPrice;
                    originalMsg.ProxyBookingID = preMsg.ProxyBookingID;
                    preMsg = originalMsg;
                }

                //>0 means the preauth payment and the previous payment belong to same transaction  
                if (preMsg.LastPaymentID > 0)
                {
                    var preTrans = ServiceInstanceSingleton.PaymentService.GetPaymentExchangeInfo(preMsg.LastPaymentID, _uid);
                    preTrans.UniqueID = preMsg.UniqueID;
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(preTrans, _uid);
                }

                //already have booking
                if (preMsg.ProxyBookingID > 0 && addNewRelation)
                {
                    ProxyBookingPayment pys = new ProxyBookingPayment();
                    pys.UPPaymentID = preMsg.PaymentID;
                    pys.State = VRentDataDictionay.BookingPaymentState.Enable.GetValue();
                    pys.ProxyBookingID = pys.ProxyBookingID;
                    pys.CreatedBy = _uid.ToGuidNull();
                    pys.CreatedOn = DateTime.Now;
                    ReservationDAL.UpdateUpPaymentForBooking(pys);
                }
            });

            Payment = preMsg;

            return this;
        }

        public ITransactionInvoker PreAuth()
        {
            //Invalid price
            if (Payment.PreAuthPrice.ToDouble() <= 0)
            {
                var mes = PaymentDAL.GetPaymentExchangeInfo(Payment.PaymentID, _uid);
                mes.State = PaymentStatusEnum.NoFee.GetValue();
                mes.Operation = PaymentStatusEnum.NoFee.ToString();
                ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(mes, _uid);
                throw new FaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000018.ToString(),
                    Message = MessageCode.CVB000018.GetDescription(),
                    Type = MessageCode.CVB000018.GetMessageType(),
                    MessageArgs = new object[] { Payment.PreAuthPrice }
                }, MessageCode.CVB000018.GetDescription());
            }

            var exception = new ReturnResult()
            {
                Code = MessageCode.CVB000010.ToString(),
                Message = MessageCode.CVB000010.GetDescription(),
                Type = MessageCode.CVB000010.GetMessageType()
            };

            //Define one up reserved value
            Dictionary<string, string> reserved = new Dictionary<string, string>();
            reserved.Add(UnionPayUtils.ReservedMessageKey1, Payment.PaymentID.ToString());
            reserved.Add(UnionPayUtils.ReservedMessageKey2, _uid);

            Payment.ReservedMsg = reserved.ObjectToJson();
            //var preRet = this.PreAuthorizeViaUP(price, cardId, smsCode, modifyUserId, paymentId, reserved.ObjectToJson(), tempOrderId, tempOrderTime).JsonDeserialize<ReturnResult>();
            var paymentInvoker = PaymentFactory.GetInstance<UnionPayInvoker>(_uid, ServiceInstanceSingleton.PaymentService.GetCardToken(Payment.CardID, _uid));
            var preRet = paymentInvoker.PreAuthorizeViaUP(Payment);
            TransactionResCode = preRet;

            if (preRet.Success == 2)
            {
                //var originalMessage = PaymentDAL.GetPaymentExchangeInfo(Payment.PaymentID, _uid);
                if (preRet.Code == "03")
                {
                    Payment.State = PaymentStatusEnum.PreAuthRetryShortTime.GetValue();
                    Payment.Operation = PaymentStatusEnum.PreAuthRetryShortTime.ToStr();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
                else if (preRet.Code == "05")
                {
                    Payment.State = PaymentStatusEnum.PreAuthRetryLongTime.GetValue();
                    Payment.Operation = PaymentStatusEnum.PreAuthRetryLongTime.ToStr();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
            }
            else if (preRet.Success == 1)
            {
                //preauth is successful, persist its queryId
                //var originalMessage = this.GetPaymentExchangeInfo(paymentId, modifyUserId);
                if (!String.IsNullOrWhiteSpace(paymentInvoker.Response.QueryId))
                {
                    Payment.PreAuthQueryID = paymentInvoker.Response.QueryId;
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
            }
            else if (preRet.Success == 0)
            {
                //var originalMessage = this.GetPaymentExchangeInfo(paymentId, modifyUserId);
                Payment.State = PaymentStatusEnum.PreAuthFailed.GetValue();
                Payment.Operation = PaymentStatusEnum.PreAuthFailed.ToStr();
                ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);

                //Send preauth failed
                //Task.Factory.StartNew(() =>
                //{
                //    try
                //    {
                //        //Email
                //        var ds = new DataService();
                //        //Email Sender
                //        IPreauthFailedSender sender = EmailSenderFactory.CreatePreauthFailedSender();
                //        sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                //        {
                //            ds.SendPaymentEmail(arg1, arg2.ToStr(), arg3);
                //        };
                //        sender.Send(new EmailParameterEntity()
                //        {
                //            Price = price,
                //            FirstName = bookingUserSetting.VName,
                //            LastName = bookingUserSetting.Name
                //        }, bookingUserSetting.Mail);
                //    }
                //    catch (Exception ex)
                //    {
                //        //Email
                //        LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                //            String.Format("Exception:{0}", ex.ToStr()), modifyUserId);
                //    }

                //}, TaskCreationOptions.PreferFairness);

                if (preRet.Code == "10" || preRet.Code == "13" || preRet.Code == "99")
                {
                    throw new FaultException<ReturnResult>(exception, exception.Message);
                }
                else
                {
                    throw new FaultException<ReturnResult>(preRet, preRet.Message);
                }
            }
            return this;
        }

        public ITransactionInvoker PreAuthEmail(ProxyUserSetting userInfo)
        {
            if (userInfo != null)
            {
                //Send email
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        //Email
                        //Email Sender
                        IPreauthFailedSender sender = EmailSenderFactory.CreatePreauthFailedSender();
                        sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                        {
                            ServiceInstanceSingleton.DataService.SendPaymentEmail(arg1, arg2.ToStr(), arg3);
                        };
                        sender.Send(new EmailParameterEntity()
                        {
                            Price = Payment.PreAuthPrice,
                            FirstName = userInfo.VName,
                            LastName = userInfo.Name
                        }, userInfo.Mail);
                    }
                    catch (Exception ex)
                    {
                        //Email
                        LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                            String.Format("Exception:{0}", ex.ToStr()), _uid);
                    }

                }, TaskCreationOptions.PreferFairness);
            }
            return this;
        }


        public ITransactionInvoker CancelTransaction(PaymentExchangeMessage calMeg, UnionPayExchangeMessage message)
        {
            TransactionUPManager.RunPart(() =>
            {
                //Cancel preauthorizaion
                //calMeg = this.GetPaymentExchangeInfo(pys.UPPaymentID, modifyUserId);
                calMeg = this._checkTransactionQueryID(calMeg.PaymentID);
                //reset it with preauth pirce
                message.priceTotal = calMeg.PreAuthPrice;
                calMeg.Message = message.ObjectToJson();
                //Update message
                //this.UpdatePaymentMessageExchange(calMeg, modifyUserId);

                //cancel preauthorizaiton
                //orginPay.OrigQryId = calMeg.PreAuthQueryID;
                //orginPay.TxnAmt = calMeg.PreAuthPrice;

                //Canceling
                calMeg.State = PaymentStatusEnum.PreAuthCancelling.GetValue();
                calMeg.Operation = PaymentStatusEnum.PreAuthCancelling.ToString();
                ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(calMeg, _uid);
            });


            Payment = calMeg;
            _exchangeMessage = message;

            return this;
        }

        public ITransactionInvoker Cancel(string callBackApiMethod)
        {
            Dictionary<string, string> reserved = new Dictionary<string, string>();
            reserved.Add(UnionPayUtils.ReservedMessageKey1, Payment.PaymentID.ToString());
            reserved.Add(UnionPayUtils.ReservedMessageKey2, _uid);

            var paymentInvoker = PaymentFactory.GetInstance<UnionPayInvoker>(_uid);
            Payment.ReservedMsg = reserved.ObjectToJson();
            var calRet = paymentInvoker.CancelPreauthorizationViaUP(Payment, callBackApiMethod);
            //var calRet = this.CancelPreauthorizationViaUP(orginPay.ObjectToJson(), reserved.ObjectToJson(), modifyUserId, callBackMethod).JsonDeserialize<ReturnResult>();

            if (calRet.Success == 2)
            {
                if (calRet.Code == "03")
                {
                    Payment.State = PaymentStatusEnum.PreAuthCancelRetryShortTime.GetValue();
                    Payment.Operation = PaymentStatusEnum.PreAuthCancelRetryShortTime.ToString();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
                else if (calRet.Code == "05")
                {
                    Payment.State = PaymentStatusEnum.PreAuthCancelRetryLongTime.GetValue();
                    Payment.Operation = PaymentStatusEnum.PreAuthCancelRetryLongTime.ToString();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
            }
            else if (calRet.Success == 0)
            {
                Payment.State = PaymentStatusEnum.PreAuthCancelFailed.GetValue();
                Payment.Operation = PaymentStatusEnum.PreAuthCancelFailed.ToString();
                ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
            }

            TransactionResCode = calRet;

            this._retryPaymentStatus(Payment.PaymentID, PaymentStatusEnum.PreAuthCanceled, paymentInvoker.Response);

            return this;
        }

        public ITransactionInvoker CancelEmail(ProxyUserSetting userInfo)
        {
            if (userInfo != null)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        //Send email
                        IPreauthCancelFailedSender sender = EmailSenderFactory.CreatePreauthCancelFailedSender();
                        sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                        {
                            ServiceInstanceSingleton.DataService.SendPaymentEmail(arg1, arg2.ToStr(), arg3);
                        };
                        sender.Send(new EmailParameterEntity()
                        {
                            Price = Payment.PreAuthPrice,
                            FirstName = userInfo.VName,
                            LastName = userInfo.Name
                        }, userInfo.Mail);
                    }
                    catch (Exception ex)
                    {
                        //Email
                        LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                            String.Format("Exception:{0}", ex.ToStr()), _uid);
                    }
                }, TaskCreationOptions.PreferFairness);
            }

            return this;
        }

        /// <summary>
        /// Check the queryId is existed or not after every specified interval time
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        private PaymentExchangeMessage _checkTransactionQueryID(int paymentId)
        {
            var intervalStrategy = new UnionPayState();
            while (true)
            {
                var payment = ServiceInstanceSingleton.PaymentService.GetPaymentExchangeInfo(paymentId, _uid);
                if (!String.IsNullOrWhiteSpace(payment.PreAuthQueryID))
                {
                    return payment;
                }
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Retry
        /// </summary>
        /// <param name="operationUserId"></param>
        /// <param name="paymentId"></param>
        private void _retryPaymentStatus(int paymentId, PaymentStatusEnum successStatus, UnionPay response)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var i = 1;
                    var intervalStrategy = new UnionPayState();
                    while (true)
                    {
                        try
                        {
                            Thread.Sleep(intervalStrategy.TimeSpan[i]);
                            var payment = ServiceInstanceSingleton.PaymentService.GetPaymentExchangeInfo(paymentId, _uid);
                            if (payment.State.ToStr().ToEnum<PaymentStatusEnum>().IsMiddleStatus())
                            {
                                //var result = this.CheckPaymentStatusViaUP("", operationUserId, response.QueryId, response.TxnTime, response.OrderId);

                                var result = PaymentFactory.GetInstance<UnionPayInvoker>(_uid).CheckPaymentStatusViaUP(
                                        new PaymentExchangeMessage()
                                        {
                                            PreAuthQueryID = response.QueryId,
                                            PreAuthTempOrderID = response.OrderId,
                                            PreAuthDateTime = response.TxnTime
                                        }, "");

                                //Success
                                if (result.Code == "00")
                                {
                                    //Update trasaction
                                    payment.State = successStatus.GetValue();
                                    payment.Operation = successStatus.ToStr();
                                    payment.RetryCount = i;
                                    payment.PreAuthQueryID = response.QueryId;
                                    payment.PreAuthID = response.PreAuthId;
                                    payment.PreAuthTempOrderID = response.OrderId;
                                    payment.PreAuthDateTime = response.TxnTime;
                                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(payment, _uid);

                                    //Add order if current type of opeartion is fee deduction
                                    if (!String.IsNullOrWhiteSpace(payment.Message))
                                    {
                                        UnionPayExchangeMessage exchangeDic = payment.Message.JsonDeserialize<UnionPayExchangeMessage>();
                                        //only avaliable for fee dedcution op
                                        if (String.Equals(PayOperationEnum.FeeDeduction.ToString(), exchangeDic.type))
                                        {
                                            //Add orders
                                            ServiceInstanceSingleton.DataService.AddOrderAfterPayment(new ProxyOrder()
                                            {
                                                ProxyBookingID = exchangeDic.bookingId.ToInt(),
                                                BookingUserID = exchangeDic.userId,
                                                State = CommonState.Active.GetValue(),
                                                CreatedBy = exchangeDic.adminUserId.ToGuidNull()
                                            }, _uid);
                                        }
                                    }
                                    else
                                    {
                                        LogInfor.WriteInfo(MessageCode.CVB000024.ToStr(), String.Format(MessageCode.CVB000024.GetDescription(), paymentId), _uid);
                                    }

                                    break;
                                }
                                i++;
                                //Exceed the max times of retry
                                if (i >= intervalStrategy.TimeSpan.Length)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            LogInfor.WriteError("Payment Retry", ex.ToStr(), _uid);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogInfor.WriteError("Payment Retry", ex.ToStr(), _uid);
                }

            }, TaskCreationOptions.LongRunning);
        }


        public bool IsFailed()
        {
            if (TransactionResCode != null)
            {
                return TransactionResCode.Success == 0;
            }
            return true;
        }


        public ITransactionInvoker DeductionTransaction(PaymentExchangeMessage dedcutMsg, UnionPayExchangeMessage message)
        {
            TransactionUPManager.RunPart(() =>
            {
                dedcutMsg.Message = message.ObjectToJson();
                bool addNewRelation = false;
                if (dedcutMsg.PaymentID == 0)
                {
                    dedcutMsg.CreatedOn = DateTime.Now;
                    dedcutMsg.UniqueID = Guid.NewGuid().ToStr();
                    dedcutMsg.PaymentID = ServiceInstanceSingleton.PaymentService.AddPaymentMessageExchange(dedcutMsg, _uid);
                    addNewRelation = true;
                }
                else
                {
                    var originalMsg = ServiceInstanceSingleton.PaymentService.GetPaymentExchangeInfo(dedcutMsg.PaymentID, _uid);
                    originalMsg.Operation = dedcutMsg.Operation;
                    originalMsg.State = dedcutMsg.State;
                    originalMsg.PreAuthDateTime = dedcutMsg.PreAuthDateTime;
                    originalMsg.PreAuthTempOrderID = dedcutMsg.PreAuthTempOrderID;
                    originalMsg.DeductionPrice = dedcutMsg.DeductionPrice;
                    originalMsg.ProxyBookingID = dedcutMsg.ProxyBookingID;
                    dedcutMsg = originalMsg;
                }
                //this.UpdatePaymentMessageExchange(dedcutMsg, modifyUserId);

                dedcutMsg.State = PaymentStatusEnum.Deducting.GetValue();
                dedcutMsg.Operation = PaymentStatusEnum.Deducting.ToString();
                ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(dedcutMsg, _uid);

                //>0 means the dedcution payment and the previous payment belong to same transaction  
                if (dedcutMsg.LastPaymentID > 0)
                {
                    var preTrans = ServiceInstanceSingleton.PaymentService.GetPaymentExchangeInfo(dedcutMsg.LastPaymentID, _uid);
                    preTrans.UniqueID = dedcutMsg.UniqueID;
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(preTrans, _uid);
                }

                if (addNewRelation)
                {
                    ProxyBookingPayment pys = new ProxyBookingPayment()
                    {
                        UPPaymentID = dedcutMsg.PaymentID,
                        ProxyBookingID = dedcutMsg.ProxyBookingID,
                        State = VRentDataDictionay.BookingPaymentState.Disable.GetValue(),

                    };
                    //new relationship
                    //pys.State = VRentDataDictionay.BookingPaymentState.Disable.GetValue();
                    //pys.UPPaymentID = dedcutMsg.PaymentID;
                    //Disable old one
                    ServiceInstanceSingleton.DataService.UpdateBookingPaymentState(pys, _uid);

                    pys.State = VRentDataDictionay.BookingPaymentState.Enable.GetValue();
                    //Add new old
                    ServiceInstanceSingleton.DataService.AddBookingPayment(pys, _uid);
                }
            });

            Payment = dedcutMsg;
            _exchangeMessage = message;

            return this;
        }

        public ITransactionInvoker DeductionTransaction(PaymentExchangeMessage dedcutMsg, UnionPayExchangeMessage message, int[] orderItemIds)
        {
            TransactionUPManager.RunPart(() =>
            {
                dedcutMsg.Message = message.ObjectToJson();
                bool addNewRelation = false;
                if (dedcutMsg.PaymentID == 0)
                {
                    dedcutMsg.CreatedOn = DateTime.Now;
                    dedcutMsg.UniqueID = Guid.NewGuid().ToStr();
                    dedcutMsg.PaymentID = ServiceInstanceSingleton.PaymentService.AddPaymentMessageExchange(dedcutMsg, _uid);
                    addNewRelation = true;
                }
                else
                {
                    var originalMsg = ServiceInstanceSingleton.PaymentService.GetPaymentExchangeInfo(dedcutMsg.PaymentID, _uid);
                    originalMsg.Operation = dedcutMsg.Operation;
                    originalMsg.State = dedcutMsg.State;
                    originalMsg.PreAuthDateTime = dedcutMsg.PreAuthDateTime;
                    originalMsg.PreAuthTempOrderID = dedcutMsg.PreAuthTempOrderID;
                    originalMsg.DeductionPrice = dedcutMsg.DeductionPrice;
                    originalMsg.ProxyBookingID = dedcutMsg.ProxyBookingID;
                    dedcutMsg = originalMsg;
                }
                //this.UpdatePaymentMessageExchange(dedcutMsg, modifyUserId);

                dedcutMsg.State = PaymentStatusEnum.Deducting.GetValue();
                dedcutMsg.Operation = PaymentStatusEnum.Deducting.ToString();
                ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(dedcutMsg, _uid);

                if (addNewRelation)
                {
                    //BookingIndirectFeePayment pys = new BookingIndirectFeePayment() {
                    //     BookingID = dedcutMsg.ProxyBookingID,
                    //     UPPaymentID = dedcutMsg.PaymentID,
                    //     State = VRentDataDictionay.BookingIndirectFeePaymentState.Enable.GetValue()
                    //};
                    //new relationship
                    //pys.State = VRentDataDictionay.BookingIndirectFeePaymentState.Enable.GetValue();
                    //pys.UPPaymentID = dedcutMsg.PaymentID;

                    List<BookingIndirectFeePayment> all = new List<BookingIndirectFeePayment>();
                    foreach (int id in orderItemIds)
                    {
                        all.Add(new BookingIndirectFeePayment()
                        {
                            BookingID = dedcutMsg.ProxyBookingID,
                            UPPaymentID = dedcutMsg.PaymentID,
                            OrderItemID = id,
                            CreatedBy = _uid.ToGuidNull(),
                            CreateOn = DateTime.Now,
                            State = VRentDataDictionay.BookingIndirectFeePaymentState.Disable.GetValue()
                        });
                    }
                    //Disable old ones
                    ServiceInstanceSingleton.DataService.UpdateBookingIndirectFeePayment(all);
                    all.ForEach(r =>
                    {
                        r.State = VRentDataDictionay.BookingIndirectFeePaymentState.Enable.GetValue();
                    });
                    //Add new ones
                    ServiceInstanceSingleton.DataService.AddBookingIndirectFeePayment(all);
                }
            });

            Payment = dedcutMsg;
            _exchangeMessage = message;

            return this;
        }

        public ITransactionInvoker Deduction(string bookingUserId)
        {
            //invalid price
            if (Payment.DeductionPrice.ToDouble() <= 0)
            {
                Payment.State = PaymentStatusEnum.NoFee.GetValue();
                Payment.Operation = PaymentStatusEnum.NoFee.ToString();
                ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                TransactionResCode = new ReturnResult() { Success = 0, Message = PaymentStatusEnum.NoFee.GetDescription() };
            }

            Dictionary<string, string> reserved = new Dictionary<string, string>();
            reserved.Add(UnionPayUtils.ReservedMessageKey1, Payment.PaymentID.ToString());
            reserved.Add(UnionPayUtils.ReservedMessageKey2, _uid);


            var paymentInvoker = PaymentFactory.GetInstance<UnionPayInvoker>(_uid, ServiceInstanceSingleton.PaymentService.GetCardToken(Payment.CardID, bookingUserId));
            Payment.ReservedMsg = reserved.ObjectToJson();

            var dectRet = paymentInvoker.DeductionViaUP(Payment);
            TransactionResCode = dectRet;
            //var dectRet = this.DeductionViaUP(
            //    dedcutMsg.DeductionPrice,
            //    dedcutMsg.PaymentID, cardId, bookingUserId, reserved.ObjectToJson(), modifyUserId, tempOrderId, tempOrderTime).JsonDeserialize<ReturnResult>();

            if (dectRet.Success == 0)
            {
                Payment.State = PaymentStatusEnum.DeductionFailed.GetValue();
                Payment.Operation = PaymentStatusEnum.DeductionFailed.ToString();
                ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                throw new FaultException<ReturnResult>(dectRet, dectRet.Message);
            }
            else if (dectRet.Success == 2)
            {
                if (dectRet.Code == "03")
                {
                    Payment.State = PaymentStatusEnum.DeductionRetryShortTime.GetValue();
                    Payment.Operation = PaymentStatusEnum.DeductionRetryShortTime.ToString();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
                else if (dectRet.Code == "05")
                {
                    Payment.State = PaymentStatusEnum.DeductionRetryLongTime.GetValue();
                    Payment.Operation = PaymentStatusEnum.DeductionRetryLongTime.ToString();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
            }
            else if (dectRet.Success == 1)
            {
                //persist queryId
                if (!String.IsNullOrWhiteSpace(paymentInvoker.Response.QueryId))
                {
                    Payment.PreAuthQueryID = paymentInvoker.Response.QueryId;
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
            }

            this._retryPaymentStatus(Payment.PaymentID, PaymentStatusEnum.Deducted, paymentInvoker.Response);

            return this;
        }

        public ITransactionInvoker DeductionEmail(ProxyUserSetting userInfo, PayOperationEnum opType)
        {
            if (userInfo != null)
            {
                #region Deduction Failed Email

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        //Fee deduction
                        if (opType == PayOperationEnum.FeeDeduction)
                        {

                            //Email sender
                            IFeeDeductionFailedSender sender = EmailSenderFactory.CreateFeeDeductionFailedSender();

                            sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                            {
                                ServiceInstanceSingleton.DataService.SendPaymentEmail(arg1, arg2.ToStr(), arg3);
                            };

                            sender.Send(new EmailParameterEntity()
                            {
                                LastName = userInfo.VName,
                                FirstName = userInfo.Name,
                                Price = Payment.DeductionPrice
                            }, new string[] { userInfo.Mail });


                        }
                        //Indirect fee deduction
                        else if (opType == PayOperationEnum.IndirectFeeDeduction)
                        {
                            IIndirectFeeDeductionFailedSender sender = EmailSenderFactory.CreateIndirectFeeDeductionFailedSender();
                            sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                            {
                                ServiceInstanceSingleton.DataService.SendPaymentEmail(arg1, arg2.ToStr(), arg3);
                            };
                            sender.Send(new EmailParameterEntity()
                            {
                                LastName = userInfo.VName,
                                FirstName = userInfo.Name,
                                Price = Payment.DeductionPrice
                            }, new string[] { userInfo.Mail });
                        }
                    }
                    catch (Exception ex)
                    {
                        //Email
                        LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                            String.Format("Exception:{0}", ex.ToStr()), _uid);
                    }
                }, TaskCreationOptions.PreferFairness);

                #endregion
            }

            return this;
        }


        public ITransactionInvoker CompleteTransaction(PaymentExchangeMessage finMeg, UnionPayExchangeMessage message)
        {
            TransactionUPManager.RunPart(() =>
            {
                //Get current payment message and avoid losing values
                var OriginalfinMeg = this._checkTransactionQueryID(finMeg.PaymentID);
                OriginalfinMeg.RealPreAuthPrice = finMeg.RealPreAuthPrice;
                OriginalfinMeg.ProxyBookingID = finMeg.ProxyBookingID;
                finMeg = OriginalfinMeg;

                //The max value of preauth price converting from yuan to fen
                var preauthMaxFen = UnionPayUtils.YuanToFen(finMeg.PreAuthPrice).ToDouble() * 1.15;
                var preauthMaxYuan = UnionPayUtils.FenToYuan(preauthMaxFen.ToStr()).ToDouble();

                var feeFen = UnionPayUtils.YuanToFen(finMeg.RealPreAuthPrice).ToLong();

                if (feeFen <= preauthMaxFen)
                {
                    //If preauth completion is success, send successful email
                    message.isSendEmail = true;

                    //Persist message
                    finMeg.Message = message.ObjectToJson();

                    finMeg.DeductionPrice = null;
                    //completing
                    finMeg.State = PaymentStatusEnum.PreAuthCompleting.GetValue();
                    finMeg.Operation = PaymentStatusEnum.PreAuthCompleting.ToString();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(finMeg, _uid);
                }
                else
                {
                    //If preauth completion is success, don't send successful email because deduction will do
                    message.isSendEmail = false;

                    //Persist message
                    finMeg.Message = message.ObjectToJson();
                    //the price for preauth completion
                    //orginPay.TxnAmt = (finMeg.PreAuthPrice.ToDouble() * 1.15).ToString();

                    //The additional price
                    finMeg.DeductionPrice = UnionPayUtils.FenToYuan(feeFen - preauthMaxFen);

                    //The real preauth price
                    finMeg.RealPreAuthPrice = UnionPayUtils.FenToYuan(preauthMaxFen);
                    //isNeedFeeDeduction = true;

                    //completing
                    finMeg.State = PaymentStatusEnum.PreAuthCompleting.GetValue();
                    finMeg.Operation = PaymentStatusEnum.PreAuthCompleting.ToString();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(finMeg, _uid);

                    //need deduct additional fee
                    var deductionOrderStruct = UnionPayUtils.GenerateTempOrder();
                    finMeg.PreAuthTempOrderID = deductionOrderStruct.Item1;
                    finMeg.PreAuthDateTime = deductionOrderStruct.Item2;
                    finMeg.Operation = PaymentStatusEnum.PreDeduction.ToString();
                    finMeg.State = PaymentStatusEnum.PreDeduction.GetValue();
                    //this.DeductionTransaction(finMeg, message);
                }

                Payment = finMeg;
                _exchangeMessage = message;

            });

            return this;
        }

        public ITransactionInvoker Complete()
        {
            Dictionary<string, string> reserved = new Dictionary<string, string>();
            reserved.Add(UnionPayUtils.ReservedMessageKey1, Payment.PaymentID.ToString());
            reserved.Add(UnionPayUtils.ReservedMessageKey2, _uid);


            var paymentInvoker = PaymentFactory.GetInstance<UnionPayInvoker>(_uid);
            Payment.ReservedMsg = reserved.ObjectToJson();
            var finRet = paymentInvoker.CompletePreauthorizaionViaUP(Payment);
            TransactionResCode = finRet;
            //var finRet = this.CompletePreauthorizaionViaUP(finMeg.PaymentID, orginPay.TxnAmt, reserved.ObjectToJson(), modifyUserId).JsonDeserialize<ReturnResult>();

            if (finRet.Success == 2)
            {
                if (finRet.Code == "03")
                {
                    Payment.State = PaymentStatusEnum.PreAuthCompleteRetryShortTime.GetValue();
                    Payment.Operation = PaymentStatusEnum.PreAuthCompleteRetryShortTime.ToString();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
                else if (finRet.Code == "05")
                {
                    Payment.State = PaymentStatusEnum.PreAuthCompleteRetryLongTime.GetValue();
                    Payment.Operation = PaymentStatusEnum.PreAuthCompleteRetryLongTime.ToString();
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);
                }
            }
            else if (finRet.Success == 0)
            {
                Payment.State = PaymentStatusEnum.PreAuthCompleteFailed.GetValue();
                Payment.Operation = PaymentStatusEnum.PreAuthCompleteFailed.ToString();
                ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(Payment, _uid);

                throw new FaultException<ReturnResult>(finRet, finRet.Message);
            }

            this._retryPaymentStatus(Payment.PaymentID, PaymentStatusEnum.PreAuthCompleted, paymentInvoker.Response);

            return this;
        }

        public ITransactionInvoker CompleteEmail(ProxyUserSetting userInfo)
        {
            if (userInfo != null)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var ds = new DataService();
                        //Email sender
                        IPreauthCompletionFailedSender sender = EmailSenderFactory.CreatePreauthCompletionFailedSender();
                        sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                        {
                            ds.SendPaymentEmail(arg1, arg2.ToStr(), arg3);
                        };

                        sender.Send(new EmailParameterEntity()
                        {
                            FirstName = userInfo.Name,
                            LastName = userInfo.VName,
                            Price = Payment.RealPreAuthPrice
                        }, userInfo.Mail);
                    }
                    catch (Exception ex)
                    {
                        //Email
                        LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                            String.Format("Exception:{0}", ex.ToStr()), _uid);
                    }
                }, TaskCreationOptions.PreferFairness);
            }

            return this;
        }


        public ITransactionInvoker CheckPaymentStatus(PaymentExchangeMessage checkPayment, string resCode)
        {
            var paymentInvoker = PaymentFactory.GetInstance<UnionPayInvoker>(_uid);
            ReturnResult ret = new ReturnResult();
            try
            {
                ret = paymentInvoker.CheckPaymentStatusViaUP(checkPayment, resCode);

                if (ret.Code == "00")
                {

                    //Update payment status
                    checkPayment.PreAuthQueryID = paymentInvoker.Response.QueryId;
                    checkPayment.PreAuthTempOrderID = paymentInvoker.Response.OrderId;
                    checkPayment.PreAuthDateTime = paymentInvoker.Response.TxnTime;
                    checkPayment.RetryCount = checkPayment.RetryCount + 1;
                    checkPayment.PreAuthID = paymentInvoker.Response.PreAuthId;

                    switch (checkPayment.State.ToStr().ToEnum<PaymentStatusEnum>().GetBelonging())
                    {
                        case PayOperationEnum.Preauth:
                            checkPayment.State = PaymentStatusEnum.PreAuthorized.GetValue();
                            checkPayment.Operation = PaymentStatusEnum.PreAuthorized.ToStr();
                            break;
                        case PayOperationEnum.PreauthCancellation:
                            checkPayment.State = PaymentStatusEnum.PreAuthCanceled.GetValue();
                            checkPayment.Operation = PaymentStatusEnum.PreAuthCanceled.ToStr();
                            break;
                        case PayOperationEnum.PreauthCompletion:
                            checkPayment.State = PaymentStatusEnum.PreAuthCompleted.GetValue();
                            checkPayment.Operation = PaymentStatusEnum.PreAuthCompleted.ToStr();
                            break;
                        case PayOperationEnum.FeeDeduction:
                            checkPayment.State = PaymentStatusEnum.Deducted.GetValue();
                            checkPayment.Operation = PaymentStatusEnum.Deducted.ToStr();
                            break;
                    }

                    this.Payment = checkPayment;
                    ServiceInstanceSingleton.PaymentService.UpdatePaymentMessageExchange(checkPayment,checkPayment.UserID);
                    ret.Message = checkPayment.Operation;
                }
            }
            catch (Exception ex)
            {
                LogInfor.WriteError("Payment Retry", String.Format("PaymentMessage:{0}, Error:{1}", checkPayment.ObjectToJson(), ex.ToStr()), checkPayment.UserID);
                ret.Success = 0;
            }
            finally
            {
                this.TransactionResCode = ret;
            }
            return this;
        }
    }
}