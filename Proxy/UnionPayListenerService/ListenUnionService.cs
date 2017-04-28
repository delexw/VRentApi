using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.UPSDK.Entities;
using CF.VRent.BLL;
using CF.VRent.UPSDK;
using System.Net;
using CF.VRent.Entities;
using CF.VRent.Entities.PaymentService;
using CF.VRent.Entities.KemasWrapper;
using System.Configuration;
using System.Reflection;
using CF.VRent.Entities.DataAccessProxy;
using PME = CF.VRent.Entities.PaymentService;
using CF.VRent.Entities.KEMASWSIF_RESERVATIONRef;
using System.Threading;
using CF.VRent.BLL.BLLFactory.Payment;
using CF.VRent.BLL.BLLFactory;
using System.Threading.Tasks;
using CF.VRent.Contract;
using CF.VRent.Email.EmailSender;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Email;
using CF.VRent.Email.EmailSender.Payment;

namespace Proxy
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ListenUnionService : IListenUnionService
    {
        private IPayment pb = ServiceImpInstanceFactory.CreatePaymentInstance(new CF.VRent.Common.UserContracts.ProxyUserSetting());

        /// <summary>
        /// After preauth is successful in union pay, a request from union pay will call this method in ours
        /// </summary>
        /// <param name="response"></param>
        public void ListenPreauthorization(Stream response)
        {
            //Pre authorization sccess
            PME.PaymentExchangeMessage mes = new PME.PaymentExchangeMessage();

            var resObj = _LogResponse(response);

            if (!String.IsNullOrWhiteSpace(resObj.UniqueValue) && !String.IsNullOrWhiteSpace(resObj.UserID))
            {
                mes = pb.GetPaymentExchangeInfo(resObj.UniqueValue.ToInt(), resObj.UserID);
                mes.PaymentID = resObj.UniqueValue.ToInt();
                //mes.RetryCount = 0;
                mes.State = Convert.ToInt16(PaymentStatusEnum.PreAuthorized);
                mes.PreAuthID = resObj.Message.PreAuthId.ToStr();
                mes.PreAuthQueryID = resObj.Message.QueryId.ToStr();
                mes.PreAuthDateTime = resObj.Message.TxnTime.ToStr();
                //mes.PreAuthPrice = resObj.TxnAmt.ToStr();
                mes.PreAuthTempOrderID = resObj.Message.OrderId.ToStr();
                mes.Operation = PaymentStatusEnum.PreAuthorized.ToString();

                var r = pb.UpdateExchangeMessage(mes, resObj.UserID);

                Task.Factory.StartNew(() =>
                {
                    //SendEmail
                    try
                    {
                        #region SendEmail
                        KemasUserAPI api = new KemasUserAPI();
                        var user = api.findUser(resObj.UserID);

                        //Email sender
                        IPreauthSuccessSender sender = EmailSenderFactory.CreatePreauthSuccessSender();
                        sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                        {
                            pb.SendPaymentEmail(arg1, arg2, arg3);
                        };
                        sender.Send(new EmailParameterEntity()
                        {
                            Price = mes.PreAuthPrice,
                            FirstName = user.VName,
                            LastName = user.Name
                        }, user.Mail);

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        //Email
                        LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                            String.Format("Exception:{0}", ex.ToStr()), resObj.UserID);
                    }

                }, TaskCreationOptions.PreferFairness);

            }
        }


        /// <summary>
        /// After preauth cancellation is successful in union pay, a request from union pay will call this method in VRent side
        /// </summary>
        /// <param name="response"></param>
        public void ListenCancelPreauthorization(Stream response)
        {
            _cancelPreauth(response, PaymentStatusEnum.PreAuthCanceled);
        }

        /// <summary>
        /// After preauth cancellation is successful in union pay, a request from union pay will call this method in VRent side
        /// </summary>
        /// <param name="response"></param>
        public void ListenCancelPreauthorizationForRedoPreauth(Stream response)
        {
            _cancelPreauth(response, PaymentStatusEnum.PreAuthCanceled, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="status"></param>
        /// <param name="directCancellation">True: cancellation only  False: cancellation with a new preauth</param>
        private void _cancelPreauth(Stream response, PaymentStatusEnum status, bool directCancellation = true)
        {
            var resObj = _LogResponse(response);

            if (!String.IsNullOrWhiteSpace(resObj.UniqueValue) && !String.IsNullOrWhiteSpace(resObj.UserID))
            {
                //Get exchange message
                var exchangeMessage = pb.GetPaymentExchangeInfo(resObj.UniqueValue.ToInt(), resObj.UserID);
                exchangeMessage.State = status.GetValue();
                exchangeMessage.Operation = status.ToString();

                if (directCancellation)
                {
                    pb.UpdateExchangeMessage(exchangeMessage, resObj.UserID);
                }
                else
                {
                    //Archive
                    pb.AddPaymentExchangeMessageHistory(exchangeMessage);
                }

                //Create order
                var createOrder = ServiceImpInstanceFactory.CreateCreateOrderInstance();
                createOrder.Create(exchangeMessage, resObj.UserID);

                #region Email
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        IPreauthCancelSuccessSender sender = EmailSenderFactory.CreatePreauthCancelSuccessSender();
                        sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                        {
                            pb.SendPaymentEmail(arg1, arg2, arg3);
                        };
                        sender.Send(new EmailParameterEntity()
                        {
                            Price = createOrder.UPMessage.priceTotal,
                            FirstName = createOrder.UPMessage.userName,
                            LastName = createOrder.UPMessage.userVName
                        }, createOrder.UPMessage.userMail);
                    }
                    catch (Exception ex)
                    {
                        //Email
                        LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                            String.Format("Exception:{0}", ex.ToStr()), createOrder.UPMessage.adminUserId);
                    }
                }, TaskCreationOptions.PreferFairness);

                #endregion
            }
        }

        /// <summary>
        /// After fee/indirecr fee deduction is successful in union pay, a request from union pay will call this method in ours
        /// </summary>
        /// <param name="response"></param>
        public void ListenCompleteConsuming(Stream response)
        {
            var resObj = _LogResponse(response);

            if (!String.IsNullOrWhiteSpace(resObj.UniqueValue) && !String.IsNullOrWhiteSpace(resObj.UserID))
            {
                //pb.ChangePaymentStatus(uniqueId.ToInt(), uniqueId.ToStr(), PaymentStatusEnum.Deducted, uid);
                //Get exchange message
                var exchangeMessage = pb.GetPaymentExchangeInfo(resObj.UniqueValue.ToInt(), resObj.UserID);

                exchangeMessage.PreAuthQueryID = resObj.Message.QueryId;
                exchangeMessage.State = PaymentStatusEnum.Deducted.GetValue();
                exchangeMessage.Operation = PaymentStatusEnum.Deducted.ToString();

                pb.UpdateExchangeMessage(exchangeMessage, resObj.UserID);

                //Create order
                var createOrder = ServiceImpInstanceFactory.CreateCreateOrderInstance();
                createOrder.Create(exchangeMessage, resObj.UserID);

                #region Fee deduction success Email

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        IEmailSender sender = null;
                        //Fee dedution email
                        if (createOrder.UPMessage.type == PayOperationEnum.FeeDeduction.ToString())
                        {
                            sender = EmailSenderFactory.CreateFeeDeductionSuccessSender();
                        }
                        //indirect fee dedution email
                        else if (createOrder.UPMessage.type == PayOperationEnum.IndirectFeeDeduction.ToString())
                        {
                            sender = EmailSenderFactory.CreateIndirectFeeDeductionSuccessSender();
                        }
                        if (sender != null)
                        {
                            sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                            {
                                pb.SendPaymentEmail(arg1, arg2, arg3);
                            };

                            sender.Send(new EmailParameterEntity()
                            {
                                Price = createOrder.UPMessage.priceTotal,
                                FirstName = createOrder.UPMessage.userVName,
                                LastName = createOrder.UPMessage.userName
                            }, createOrder.UPMessage.userMail);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Email
                        LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                            String.Format("Exception:{0}", ex.ToStr()), createOrder.UPMessage.adminUserId);
                    }
                }, TaskCreationOptions.PreferFairness);


                #endregion
            }
        }

        /// <summary>
        /// After preauth completion is successful in union pay, a request from union pay will call this method in ours
        /// </summary>
        /// <param name="response"></param>
        public void ListenCompletePreauthorization(Stream response)
        {
            var resObj = _LogResponse(response);

            if (!String.IsNullOrWhiteSpace(resObj.UniqueValue) && !String.IsNullOrWhiteSpace(resObj.UserID))
            {
                //Get exchange message
                var exchangeMessage = pb.GetPaymentExchangeInfo(resObj.UniqueValue.ToInt(), resObj.UserID);
                exchangeMessage.State = PaymentStatusEnum.PreAuthCompleted.GetValue();
                exchangeMessage.Operation = PaymentStatusEnum.PreAuthCompleted.ToString();

                //Create order
                var createOrder = ServiceImpInstanceFactory.CreateCreateOrderInstance();
                createOrder.Create(exchangeMessage, resObj.UserID);

                if (createOrder.UPMessage.isSendEmail)
                {
                    pb.UpdateExchangeMessage(exchangeMessage, resObj.UserID);

                    #region Send fee duction email
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            //Send email
                            IPreauthCompletionSuccessSender sender = EmailSenderFactory.CreatePreauthCompletionSuccessSender();
                            sender.onSendEvent += (EmailParameterEntity arg1, EmailType arg2, string[] arg3) =>
                            {
                                pb.SendPaymentEmail(arg1, arg2, arg3);
                            };

                            sender.Send(new EmailParameterEntity()
                            {
                                Price = createOrder.UPMessage.priceTotal,
                                FirstName = createOrder.UPMessage.userVName,
                                LastName = createOrder.UPMessage.userName
                            }, createOrder.UPMessage.userMail);
                        }
                        catch (Exception ex)
                        {
                            //Email
                            LogInfor.EmailLogWriter.WriteError(MessageCode.CVB000032.ToStr(),
                                String.Format("Exception:{0}", ex.ToStr()), createOrder.UPMessage.adminUserId);
                        }
                    }, TaskCreationOptions.PreferFairness);
                    #endregion
                }
                else
                {
                    //Archive as it is not final status
                    pb.AddPaymentExchangeMessageHistory(exchangeMessage);
                }
            }
        }

        /// <summary>
        /// Format post data
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="pb"></param>
        /// <param name="uid"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        private IPaymentMessageStreamSerializer _LogResponse(Stream resp)
        {
            var serializer = ServiceImpInstanceFactory.CreatePaymentMessageStreamSerializerInstance();
            serializer.Serialize(resp);

            //Log
            pb.LogPayment(serializer.Message, serializer.UserID, UnionPayEnum.MerInform);

            return serializer;
        }
    }
}