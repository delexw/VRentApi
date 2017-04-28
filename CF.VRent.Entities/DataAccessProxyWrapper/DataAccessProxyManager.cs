using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.UserContracts;
using CF.VRent.Email.EmailSender.Entity;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.FapiaoPreferenceProxy;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.PaymentService;
using CF.VRent.Entities.TermsConditionService;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Entities.DataAccessProxyWrapper
{
    public partial class DataAccessProxyManager : IDataService, IFapiaoPreferenceService, IPaymentService, ITermsConditionService
    {
        public static string UnexpectedDAPErrorCode = "CVED10001";
        public static string UnexpectedDAPErrorMsg = "DataAccessProxy throws an unexpected exception.Message:{0},StackTrace:{1}";
        public static string DAPTimeoutErrorCode = "CVED10002";
        public static string DAPTimeoutErrorMsg = "DataAccessProxy Timeout Exception..Message:{0},StackTrace:{1}";
        public static string DAPCommunicationErrorCode = "CVED10003";
        public static string DAPCommunicationErrorMsg = "DataAccessProxy Communication Exception..Message:{0},StackTrace:{1}";

        #region Constructor
        private string _userID = null;
        public DataAccessProxyManager() 
        {

        }

        public DataAccessProxyManager(string userID)
        {
            _userID = userID;
        }

        #endregion

        #region helper Method
        public static T InnerTryCatchInvoker<T>(Func<T> function, object client, string methodName)
        {
            try
            {
                LogInfor.WriteInfo("DataAccess Server is initialized", "InvokeMethod:" + methodName, "DataAccessProxyManager");
                T r = function();
                return r;
            }
            catch (FaultException<ReturnResult> DataAccessFE)
            {
                LogInfor.WriteError(DataAccessFE.Code.ToStr(), DataAccessFE.Message, "DataAccessProxyManager");
                throw DataAccessFE;
            }
            catch (TimeoutException te)
            {
                ((ICommunicationObject)client).Abort();
                VrentApplicationException vae = new VrentApplicationException(
                    DataAccessProxyManager.DAPTimeoutErrorCode,
                    string.Format(DataAccessProxyManager.DAPTimeoutErrorMsg, te.Message, te.StackTrace),
                    ResultType.DATAACCESSProxy, te);
                LogInfor.WriteError(vae.ErrorCode.ToStr(), vae.ErrorMessage, "DataAccessProxyManager");
                throw vae;
            }
            catch (CommunicationException ce)
            {
                ((ICommunicationObject)client).Abort();
                VrentApplicationException vae = new VrentApplicationException(
                    DataAccessProxyManager.DAPCommunicationErrorCode,
                    string.Format(DataAccessProxyManager.DAPCommunicationErrorMsg, ce.Message, ce.StackTrace),
                    ResultType.DATAACCESSProxy, ce);
                LogInfor.WriteError(vae.ErrorCode.ToStr(), vae.ErrorMessage, "DataAccessProxyManager");
                throw vae;
            }
            catch (VrentApplicationException vae)
            {
                ((ICommunicationObject)client).Abort();
                LogInfor.WriteError(vae.ErrorCode.ToStr(), vae.ErrorMessage, "DataAccessProxyManager");
                throw new VrentApplicationException(vae.ErrorCode, vae.ErrorMessage, vae.Category, vae);
            }
            catch (Exception ex)
            {
                VrentApplicationException vae = new VrentApplicationException(
                    DataAccessProxyManager.UnexpectedDAPErrorCode,
                    string.Format( DataAccessProxyManager.UnexpectedDAPErrorMsg,ex.Message,ex.StackTrace), 
                    ResultType.DATAACCESSProxy, ex);
                LogInfor.WriteError(vae.ErrorCode.ToStr(), vae.ErrorMessage, "DataAccessProxyManager");
                throw vae;
            }
            finally
            {
                if (client != null)
                {
                    if (((ICommunicationObject)client).State != CommunicationState.Faulted)
                    {
                        ((ICommunicationObject)client).Close();
                    }
                    ((IDisposable)client).Dispose();
                    client = null;
                }
                LogInfor.WriteInfo("DataAccess Server is disposed", "InvokeMethod:" + methodName, "DataAccessProxyManager");
            }
        }
        public static void InnerTryCatchInvoker(Action action, object client, string methodName)
        {

            DataAccessProxyManager.InnerTryCatchInvoker<Object>(() => { action(); return null; }, client, methodName);
            //try
            //{
            //    action();
            //}
            //catch (FaultException<ReturnResult> DataAccessFE)
            //{
            //    throw DataAccessFE;
            //}
            //catch (TimeoutException te)
            //{
            //    ((ICommunicationObject)client).Abort();
            //    VrentApplicationException vae = new VrentApplicationException(
            //        DataAccessProxyManager.DAPTimeoutErrorCode,
            //        string.Format(DataAccessProxyManager.DAPTimeoutErrorMsg, te.Message, te.StackTrace),
            //        ResultType.DATAACCESSProxy, te);
            //    throw vae;
            //}
            //catch (CommunicationException ce)
            //{
            //    ((ICommunicationObject)client).Abort();
            //    VrentApplicationException vae = new VrentApplicationException(
            //        DataAccessProxyManager.DAPCommunicationErrorCode,
            //        string.Format(DataAccessProxyManager.DAPCommunicationErrorMsg, ce.Message, ce.StackTrace),
            //        ResultType.DATAACCESSProxy, ce);
            //    throw vae;
            //}
            //catch (VrentApplicationException vae)
            //{
            //    ((ICommunicationObject)client).Abort();
            //    throw new VrentApplicationException(vae.ErrorCode, vae.ErrorMessage, vae.Category, vae);
            //}
            //catch (Exception ex)
            //{
            //    VrentApplicationException vae = new VrentApplicationException(
            //        DataAccessProxyManager.UnexpectedDAPErrorCode,
            //        string.Format(DataAccessProxyManager.UnexpectedDAPErrorMsg, ex.Message, ex.StackTrace),
            //        ResultType.DATAACCESSProxy, ex);
            //    throw vae;
            //}
            //finally
            //{
            //    if (client != null)
            //    {
            //        if (((ICommunicationObject)client).State != CommunicationState.Faulted)
            //        {
            //            ((ICommunicationObject)client).Close();
            //        }
            //        ((IDisposable)client).Dispose();
            //        client = null;
            //    }
            //}
        }       

        #endregion

        #region IDataService interface wrapper
        //public int Register(ProxyUser proxyuser)
        //{
        //    DataServiceClient client = new DataServiceClient();
        //    return InnerTryCatchInvoker
        //        (
        //            () => client.Register(proxyuser),
        //            client,
        //            MethodInfo.GetCurrentMethod().Name
        //        );
        //}

        //public int LoginWeb(string uname, string upwd)
        //{
        //    DataServiceClient client = new DataServiceClient();
        //    return InnerTryCatchInvoker
        //        (
        //            () => client.LoginWeb(uname,upwd),
        //            client,
        //            MethodInfo.GetCurrentMethod().Name
        //        );
        //}

        //public ProxyUser GetWebUserInfo(string email)
        //{
        //    DataServiceClient client = new DataServiceClient();
        //    return InnerTryCatchInvoker
        //        (
        //            () => client.GetWebUserInfo(email),
        //            client,
        //            MethodInfo.GetCurrentMethod().Name
        //        );
        //}

        //public bool IsWebUserEmail(string email)
        //{
        //    DataServiceClient client = new DataServiceClient();
        //    return InnerTryCatchInvoker
        //        (
        //            () => client.IsWebUserEmail(email),
        //            client,
        //            MethodInfo.GetCurrentMethod().Name
        //        );            
        //}

        //public bool ActiveUser(string email)
        //{
        //    DataServiceClient client = new DataServiceClient();
        //    return InnerTryCatchInvoker
        //        (
        //            () => client.ActiveUser(email),
        //            client,
        //            MethodInfo.GetCurrentMethod().Name
        //        );            
            
        //}

        public ProxyReservation[] RetrieveReservations(Guid userID, string[] states)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrieveReservations(userID,states),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyReservation RetrieveReservationByBookingID(int proxyBookingID)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrieveReservationByBookingID(proxyBookingID),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyReservation CreateProxyReservation(ProxyReservation reservation, ProxyBookingPayment upPayment, ProxyBookingPrice pbp)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.CreateProxyReservation(reservation, upPayment, pbp),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyReservation CancelProxyReservation(ProxyReservation reservation, ProxyBookingPrice pbp)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.CancelProxyReservation(reservation, pbp),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyReservation UpdateProxyReservation(ProxyReservation reservation, ProxyBookingPrice pbp)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.UpdateProxyReservation(reservation, pbp),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyBookingPrice LoadPrincingItems(int proxyBookingID)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.LoadPrincingItems(proxyBookingID),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyReservationPayment[] GetWaitingPayDUBBookings()
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetWaitingPayDUBBookings(),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int UpdateBookingPaymentState(ProxyBookingPayment bookp, string uid)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.UpdateBookingPaymentState(bookp,uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int AddBookingPayment(ProxyBookingPayment bookp, string uid)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddBookingPayment(bookp, uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int AddOrderAfterPayment(ProxyOrder order, string userId)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddOrderAfterPayment(order, userId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public void SendPaymentEmail(EmailParameterEntity paras, string emailType, string[] emailAddress)
        {
            DataServiceClient client = new DataServiceClient();
            InnerTryCatchInvoker
                (
                    () => client.SendPaymentEmail(paras,emailType,emailAddress),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyBookingPayment[] GetBookingPaymentByPaymentId(int paymentId)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetBookingPaymentByPaymentId(paymentId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyBookingPayment[] GetBookingPaymentByBookingId(int bookingId)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetBookingPaymentByBookingId(bookingId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyFapiao[] RetrieveMyFapiaoData(Guid userID)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrieveMyFapiaoData(userID),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyFapiao RetrieveFapiaoDataDetail(int FapiaoDataID)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrieveFapiaoDataDetail(FapiaoDataID),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public bool AddBookingIndirectFeePayment(BookingIndirectFeePayment[] records)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker<bool>
                (
                    () => client.AddBookingIndirectFeePayment(records),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int UpdateBookingIndirectFeePayment(BookingIndirectFeePayment[] records)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker<int>
                (
                    () => client.UpdateBookingIndirectFeePayment(records),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public BookingIndirectFeePayment[] GetBookingIndirectFeePaymentByBookingID(int bookingId)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker<BookingIndirectFeePayment[]>
                (
                    () => client.GetBookingIndirectFeePaymentByBookingID(bookingId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }


        public BookingIndirectFee[] GetTotalIndirectFeeForAll()
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker<BookingIndirectFee[]>
                (
                    () => client.GetTotalIndirectFeeForAll(),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        #endregion

        #region Indirect Fee
        public IndirectFeeType[] RetrieveIndirectFeeTypes()
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrieveIndirectFeeTypes(),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int AddIndirectFeeTypes(IndirectFeeType[] newTypes)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddIndirectFeeTypes(newTypes),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ReturnResultRetrieveOrderItems FindBookingOrders(int bookingId, string[] groups,ProxyUserSetting operatorInfo)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.FindBookingOrders(bookingId, groups, operatorInfo),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ReturnResultAddIndirectFeeItems AddOrderItemsByProxyBookingID(int proxyBookingID, ProxyOrderItem[] orderItems, ProxyUserSetting operatorInfo)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddOrderItemsByProxyBookingID(proxyBookingID, orderItems,operatorInfo),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }
        #endregion
        #region IFapiaoPreferenceService FP Warapper
        public CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference[] GetAllFapiaoPreference(string uid)
        {
            FapiaoPreferenceServiceClient client = new FapiaoPreferenceServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetAllFapiaoPreference(uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference GetFapiaoPreferenceDetail(string fpid)
        {
            FapiaoPreferenceServiceClient client = new FapiaoPreferenceServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetFapiaoPreferenceDetail(fpid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference CreateFapiaoPreference(CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference fp)
        {
            FapiaoPreferenceServiceClient client = new FapiaoPreferenceServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.CreateFapiaoPreference(fp),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public void DeleteFapiaoPreference(string fpid)
        {
            FapiaoPreferenceServiceClient client = new FapiaoPreferenceServiceClient();
            InnerTryCatchInvoker
                (
                    () => client.DeleteFapiaoPreference(fpid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference UpdateFapiaoPreference(CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference oldfp, CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference newFP)
        {
            FapiaoPreferenceServiceClient client = new FapiaoPreferenceServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.UpdateFapiaoPreference(oldfp, newFP),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }
        #endregion

        #region IPaymentService Wrapper

        public int LogPayment(string upWrapperBase64, string logType, string uid)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.LogPayment(upWrapperBase64, logType,uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int AddUPBindingCard(string cusWrapperBase64, string tokenBase64, string uid)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddUPBindingCard(cusWrapperBase64,  tokenBase64, uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public PaymentCard[] GetUserCreditCard(string uid)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetUserCreditCard(uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int DeleteUPBindingCard(string id, string uid)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.DeleteUPBindingCard(id,uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public string GetCardToken(string id, string uid)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetCardToken(id, uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int AddPaymentMessageExchange(PaymentExchangeMessage message, string userId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddPaymentMessageExchange(message,userId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int GetPaymentMessageState(string Id, string userId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetPaymentMessageState(Id, userId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int UpdatePaymentMessageExchange(PaymentExchangeMessage message, string userId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.UpdatePaymentMessageExchange(message, userId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public PaymentExchangeMessage GetPaymentExchangeInfo(int Id, string userId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetPaymentExchangeInfo(Id, userId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int UpdateCreditCardState(string cardId, int state, string userId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.UpdateCreditCardState(cardId, state, userId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int UpdateBookingStatusAfterPayment(string kmId, string state, string userId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.UpdateBookingStatusAfterPayment(kmId, state, userId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int GetPaymentStatusByBookingId(int bookingId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetPaymentStatusByBookingId(bookingId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        //public string SendUnionPayRequest(Dictionary<string, string> param, string url)
        //{
        //    PaymentServiceClient client = new PaymentServiceClient();
        //    return InnerTryCatchInvoker
        //        (
        //            () => client.SendUnionPayRequest(param, url),
        //            client,
        //            MethodInfo.GetCurrentMethod().Name
        //        );
        //}

        public string AddUPBindingCardViaUP(string customInfoJson, string reservedMsg, string uid)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddUPBindingCardViaUP(customInfoJson, reservedMsg, uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ReturnResult CheckPaymentStatusViaUP(string resCode,int paymentId, string uid)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.CheckPaymentStatusViaUP(resCode,paymentId, uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public string CancelCardBindingViaUP(string token, string uid)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.CancelCardBindingViaUP(token, uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public string SendBindingSMSCodeViaUP(string cardObjectJson, string uid)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.SendBindingSMSCodeViaUP(cardObjectJson, uid),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public string SendPreauthorizationSMSCodeViaUP(string cardObjectJson, string price, string uid, string tempOrderId, string tempOrderTime)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.SendPreauthorizationSMSCodeViaUP(cardObjectJson, price, uid, tempOrderId, tempOrderTime),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        //public string CompletePreauthorizaionViaUP(int paymentId, string price, string reservedMessage, string uid)
        //{
        //    PaymentServiceClient client = new PaymentServiceClient();
        //    return InnerTryCatchInvoker
        //        (
        //            () => client.CompletePreauthorizaionViaUP(paymentId, price, reservedMessage, uid),
        //            client,
        //            MethodInfo.GetCurrentMethod().Name
        //        );
        //}

        //public string DeductionViaUP(string price, int paymentId, string cardId, string cardUserId, string reservedMsg, string uid,
        //    string tempOrderId = null,
        //    string tempOrderTime = null)
        //{
        //    PaymentServiceClient client = new PaymentServiceClient();
        //    return InnerTryCatchInvoker
        //        (
        //            () => client.DeductionViaUP(price, paymentId, cardId, cardUserId, reservedMsg, uid, tempOrderId, tempOrderTime),
        //            client,
        //            MethodInfo.GetCurrentMethod().Name
        //        );
        //}

        public int CancelAndPreauthOnce(string price, string cardId, string smsCode, int paymentId, int bookingId, string bookingUserSetting, string tempOrderId, string tempOrderTime)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.CancelAndPreauthOnce(price, cardId, smsCode, paymentId, bookingId, bookingUserSetting, tempOrderId, tempOrderTime),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int PreauthOnce(string price, string cardId, string smsCode, string bookingUserSetting, int proxyBookingId, string tempOrderId, string tempOrderTime)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.PreauthOnce(price, cardId, smsCode, bookingUserSetting, proxyBookingId, tempOrderId, tempOrderTime),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public void CancelPreauthOnce(int paymentId, string price, string bookingUserSetting, int bookingId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            InnerTryCatchInvoker
                (
                    () => client.CancelPreauthOnce(paymentId, price, bookingUserSetting, bookingId),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }
        public void DeductionOnce(string type, string price, string cardId, int bookingId, int[] orderItemIds, string bookingUserSetting, string userSetting, string userPwd, string tempOrderId = null,
            string tempOrderTime = null,bool retry = false)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            InnerTryCatchInvoker
                (
                    () => client.DeductionOnce(type, price, cardId, bookingId, orderItemIds, bookingUserSetting, userSetting, userPwd, tempOrderId, tempOrderTime, retry),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }
        public void FinishAndDeduction(string type,
            double price,
            string kemasBookingId,
            int proxyBookingId,
            string bookingUserId,
            int paymentId,
            string cardId,
            string bookingUserSetting,
            string userSetting,
            string priceStructure,
            string userPwd,
            string tempOrderId = null,
            string tempOrderTime = null, string kemasState = null)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            InnerTryCatchInvoker
                (
                    () => client.FinishAndDeduction(type,
                        price,
                        kemasBookingId,
                        proxyBookingId,
                        bookingUserId,
                        paymentId, cardId, bookingUserSetting, userSetting, priceStructure, userPwd, tempOrderId, tempOrderTime, kemasState),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public int UpdatePaymentMessageExchangeRetry(int paymentId, int retryFlag, string userId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (() => client.UpdatePaymentMessageExchangeRetry(paymentId,retryFlag,userId), client, MethodInfo.GetCurrentMethod().Name);
        }

        public int UpdatePaymentMessageExchangeStatus(int paymentId, UPSDK.PaymentStatusEnum paymentStatus, string userId)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            return InnerTryCatchInvoker
                (() => client.UpdatePaymentMessageExchangeStatus(paymentId,paymentStatus,userId), client, MethodInfo.GetCurrentMethod().Name);
        }

        public RetryBooking[] GetAllRetryBookings()
        {
            PaymentServiceClient client = new PaymentServiceClient();

            return InnerTryCatchInvoker(() => client.GetAllRetryBookings().ToArray(), client, MethodInfo.GetCurrentMethod().Name);
        }

        public PaymentExchangeMessage[] GetFailedTransactionByBooking(int bookingId)
        {
            PaymentServiceClient client = new PaymentServiceClient();

            return InnerTryCatchInvoker(() => client.GetFailedTransactionByBooking(bookingId).ToArray(), client, MethodInfo.GetCurrentMethod().Name);
        }

        #endregion   
    
        public ReturnResultBulkSink BulkSyncProxyReservations(ProxyReservation[] reservations)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.BulkSyncProxyReservations(reservations),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }
        
        public ProxyReservationsWithPaging RetrieveReservationsWithPaging(ProxyReservationsWithPaging pagedBookings)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrieveReservationsWithPaging(pagedBookings),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ProxyReservationsWithPaging RetrieveReservationsWithPagingByRole(ProxyReservationsWithPaging pagedBookings,ProxyUserSetting userInfo)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrieveReservationsWithPagingByRole(pagedBookings,userInfo),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public void AddPaymentExchangeMessageHistory(PaymentExchangeMessage message)
        {
            PaymentServiceClient client = new PaymentServiceClient();
            InnerTryCatchInvoker
                (
                    () => client.AddPaymentExchangeMessageHistory(message),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }
        
        #region Request Fapiao APIs

        public ReturnResultExt UpdateFapiaoRequest(ProxyFapiaoRequest request, UserProfile up)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.UpdateFapiaoRequest(request,up),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public ReturnResultExtRetrieve RetrieveFapiaoRequestDetailByFapiaoSource(int proxyBookingID, int[] fapiaoSource,UserProfile up)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrieveFapiaoRequestDetailByFapiaoSource(proxyBookingID,fapiaoSource,up),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        #endregion

        #region TermsCondition

        public DBEntityAggregation<TermsCondition, DBConditionObject> GetLatestTC(DBConditionObject condition)
        {
            TermsConditionServiceClient client = new TermsConditionServiceClient();

            return InnerTryCatchInvoker
                (
                    () => client.GetLatestTC(condition),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public long AddOrUpdateTC(TermsCondition entity)
        {
            TermsConditionServiceClient client = new TermsConditionServiceClient();

            return InnerTryCatchInvoker
               (
                   () => client.AddOrUpdateTC(entity),
                   client,
                   MethodInfo.GetCurrentMethod().Name
               );
        }

        public long AcceptTC(UserTermsConditionAgreement entity)
        {
            TermsConditionServiceClient client = new TermsConditionServiceClient();

            return InnerTryCatchInvoker
               (
                   () => client.AcceptTC(entity),
                   client,
                   MethodInfo.GetCurrentMethod().Name
               );
        } 
        #endregion


        public ProxyBookingPrice SavingPrincingItems(ProxyBookingPrice pbp)
        {
            DataServiceClient client = new DataServiceClient();

            return InnerTryCatchInvoker
               (
                   () => client.SavingPrincingItems(pbp),
                   client,
                   MethodInfo.GetCurrentMethod().Name
               );
        }


        public UserTransferCUDResult AddTransferRequest(UserTransferRequest transferRequest, ProxyUserSetting operatorInfo)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddTransferRequest(transferRequest, operatorInfo),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public UserTransferCUDResult UpdateTransferRequest(UserTransferRequest transferRequest, ProxyUserSetting operatorInfo)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.UpdateTransferRequest(transferRequest, operatorInfo),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public UserTransferRResult RetrieveTransferRequests(ProxyUserSetting operatorInfo)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrieveTransferRequests(operatorInfo),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );

        }


        public UserTransferRResult RetrievePendingTransferRequests(Guid userID)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.RetrievePendingTransferRequests(userID),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }


        #region Common Data Dictionay
        public DBEntityAggregation<Country, DBConditionObject> GetAllCountries(DBConditionObject condition)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetAllCountries(condition),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        } 
        #endregion


        #region Statistic
        public DBEntityAggregation<GeneralLedgerStatisticCCB, DBConditionObject> GetCCBStatistic(DBConditionObject condition)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetCCBStatistic(condition),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public DBEntityAggregation<GeneralLedgerStatisticDUB, DBConditionObject> GetDUBStatistic(DBConditionObject condition)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.GetDUBStatistic(condition),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public long AddGeneralLedgerHeader(GeneralLedgerHeader entity)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddGeneralLedgerHeader(entity),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public long AddGeneralLedgerItem(GeneralLedgerItem entity)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddGeneralLedgerItem(entity),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public long AddGeneralLedgerItemDetails(GeneralLedgerItemDetail entity)
        {
            DataServiceClient client = new DataServiceClient();
            return InnerTryCatchInvoker
                (
                    () => client.AddGeneralLedgerItemDetails(entity),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        } 
        #endregion


        
    }
}
