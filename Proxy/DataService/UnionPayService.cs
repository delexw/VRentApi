using CF.VRent.Common.Entities;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using CF.VRent.BLL;
using CF.VRent.UPSDK;
using CF.VRent.UPSDK.Entities;
using CF.VRent.Entities.PaymentService;
using System.Net;
using CF.VRent.Log;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KemasWrapper;
using System.Collections.Specialized;
using System.Configuration;
using CF.VRent.Common.UserContracts;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.UserStatus;

namespace Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DataService
    {
        /// <summary>
        /// Add new binding bank card
        /// </summary>
        [WebInvoke(UriTemplate = "BankCards?userId={userId}", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public UnionPayCustomInfo AddBindingBankCard(UnionPayCustomInfo cusObj, string userId)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(userId);

            IPayment pBLL = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            if (!setting.ID.Trim().Equals(userId))
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVCE000002.ToString(),
                    Message = MessageCode.CVCE000002.GetDescription(),
                    Type = MessageCode.CVCE000002.GetMessageType()
                }, HttpStatusCode.Unauthorized);
            }

            UserBLL userBLL = new UserBLL(setting);

            if (!userBLL.CheckUserStatus(setting))
            {
                throw new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000003.ToString(),
                    Message = MessageCode.CVB000003.GetDescription(),
                    Type = MessageCode.CVB000003.GetMessageType()
                }, HttpStatusCode.BadRequest);
            }

            var result = new UnionPayCustomInfo() { CardId = pBLL.AddUPBindingCard(cusObj, userId) };
            return result;
        }

        /// <summary>
        /// Get all binding band cards
        /// </summary>
        [WebGet(UriTemplate = "BankCards?userId={uid}", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<PaymentCard> GetBindingBankCard(string uid)
        {

            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);

            IPayment pBLL = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            var res = pBLL.GetUserCreditCard(uid);

            res = res.Where(r => pBLL.CheckCreditCardTokenAvailable(r.CardId, uid)).ToList();

            return res;
        }

        /// <summary>
        /// Get sms verfication code from UP
        /// </summary>
        [WebInvoke(UriTemplate = "RegisterBankCards?userId={uid}&type={type}&price={price}", Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public Payment GetBindingSMSCode(UnionPayCustomInfo cardObject, string uid, string type, string price)
        {

            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);

            IPayment pBLL = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            Payment ret = new Payment();

            if (type == "1")
            {
                ret = pBLL.SendBindingSMSCode(cardObject, uid);
            }
            else
            {
                ret = pBLL.SendPreauthorizationSMSCode(cardObject, price, uid);
            }

            return ret;
        }

        /// <summary>
        /// Preauth
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="price"></param>
        /// <param name="cardId"></param>
        /// <param name="smsCode"></param>
        /// <param name="tempOrderId"></param>
        /// <param name="tempOrderTime"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "BankCards/{cardId}?userId={uid}&price={price}&smsCode={smsCode}&tempOrderId={tempOrderId}&tempOrderTime={tempOrderTime}&bookingId={bookingId}", Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public Payment PreAuthorize(string uid, string price, string cardId, string smsCode, string tempOrderId, string tempOrderTime, string bookingId)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);

            IPayment pBLL = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            var api = KemasAccessWrapper.CreateKemasUserAPI2Instance();

            var user = api.findUser2(setting.ID, setting.SessionID);

            var statusManager = UserStatusContext.CreateStatusManager(user.UserData.Status);

            //Only for avalible user, otherwise return null
            var unavilableStatus = new string[] { "1", "2", "3", "5", "9", "B", "C", "D", "F", "A" };
            foreach (string s in unavilableStatus)
            {
                if (statusManager.Status[s].Value == 1)
                {
                    throw new VrentApplicationException(ErrorConstants.NoPrivilegeCode, string.Format(ErrorConstants.NoPrivilegeMessage, uid), ResultType.VRENTFE);
                }
            }

            ProxyUserSetting temp = new ProxyUserSetting()
            {
                ID = setting.ID,
                Name = setting.Name,
                VName = setting.VName,
                Mail = setting.Mail
            };

            //If the tempOrderId is null and the tempOrderTime is null as well
            //It means the user didn't get the preauth sms code
            //So if the smsCode param was not null, set it null
            if (String.IsNullOrWhiteSpace(tempOrderId) && String.IsNullOrWhiteSpace(tempOrderTime))
            {
                smsCode = null;
            }

            var orderStruct = UnionPayUtils.GenerateTempOrder();

            if (String.IsNullOrWhiteSpace(tempOrderId))
            {
                tempOrderId = orderStruct.Item1;
            }
            if (String.IsNullOrWhiteSpace(tempOrderTime))
            {
                tempOrderTime = orderStruct.Item2;
            }

            return pBLL.PreAuthorize(price, cardId, smsCode, temp, bookingId.ToInt(), tempOrderId, tempOrderTime);
        }

        [WebGet(UriTemplate = "Payments/{paymentId}?userId={userId}&bookingId={bookingId}", ResponseFormat = WebMessageFormat.Json)]
        public ReturnResult CheckPaymentStatus(string userId, string paymentId, string bookingId)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(userId);

            IPayment pBLL = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            ReturnResult ret = new ReturnResult(false) { Success = 0 };

            var state = pBLL.GetPaymentStatus(paymentId.ToInt(), userId);
            //ret.Code = state.ToString();
            //ret.Message = ((PaymentStatusEnum)Enum.Parse(typeof(PaymentStatusEnum), state.ToString())).GetDescription();
            state.Code = state.Success.ToStr();
            return state;
        }


        [WebInvoke(UriTemplate = "BankCards/{cardId}?userId={userId}", Method = "DELETE", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public ReturnResult DeleteBindingBankCard(string cardId, string userId)
        {

            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(userId);

            IPayment pBLL = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            ReturnResult ret = new ReturnResult(false) { Success = 0 };

            //Modified 
            //Change the binding credit card state to 1
            //var r = pBLL.DeleteUPBindingCard(cardId, userId);
            var r = pBLL.CancelCreditCard(cardId, userId);

            if (r)
            {
                ret.Success = 1;
            }

            return ret;
        }

        /// <summary>
        /// Cancel and redo preauth
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="userId"></param>
        /// <param name="price"></param>
        /// <param name="cardId"></param>
        /// <param name="smsCode"></param>
        /// <param name="tempOrderId"></param>
        /// <param name="tempOrderTime"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "Payments?userId={userId}&bookingId={bookingId}&cardId={cardId}&price={price}&smsCode={smsCode}&tempOrderId={tempOrderId}&tempOrderTime={tempOrderTime}", 
            Method = "PUT", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public Payment CancelPreauthorization(string bookingId, 
            string userId, string price, 
            string cardId, string smsCode, 
            string tempOrderId, string tempOrderTime)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(userId);

            IPayment pBLL = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            ProxyUserSetting tempPS = new ProxyUserSetting()
            {
                ID = setting.ID,
                Name = setting.Name,
                VName = setting.VName,
                Mail = setting.Mail
            };

            var orderStruct = UnionPayUtils.GenerateTempOrder();

            if (String.IsNullOrWhiteSpace(tempOrderId))
            {
                tempOrderId = orderStruct.Item1;
            }
            if (String.IsNullOrWhiteSpace(tempOrderTime))
            {
                tempOrderTime = orderStruct.Item2;
            }

            Payment retPay = new Payment() {
                PaymentID = pBLL.RedoPreauth(bookingId.ToInt(), price, cardId, smsCode, tempPS, tempOrderId, tempOrderTime)
            };

            return retPay;
        }

        /// <summary>
        /// dedcut cancel fee
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "Payments?userId={userId}&bookingId={bookingId}", Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public ReturnResult FeeDeduction(string bookingId, string userId)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(userId);

            IPayment pBLL = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            ReturnResult ret = new ReturnResult(false) { Success = 0 };

            ProxyUserSetting tempPS = new ProxyUserSetting() {
                ID = setting.ID,
                Name = setting.Name,
                UName = setting.UName,
                VName = setting.VName,
                Mail = setting.Mail,
                SessionID = setting.SessionID
            };

            if (pBLL.FeeDeduction(bookingId.ToInt(), tempPS))
            {
                ret.Success = 1;
            }

            return ret;
        }

        /// <summary>
        /// Cancel preauth
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "Payments?userId={userId}&bookingId={bookingId}",
            Method = "DELETE", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        public ReturnResult CancelPreauth(string bookingId, string userId)
        {

            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(userId);

            IPayment pBLL = ServiceImpInstanceFactory.CreatePaymentInstance(setting);

            ReturnResult ret = new ReturnResult(false) { Success = 0 };
            ProxyUserSetting tempPS = new ProxyUserSetting()
            {
                ID = setting.ID,
                Name = setting.Name,
                VName = setting.VName,
                Mail = setting.Mail
            };
            if (pBLL.CancelPreauth(bookingId.ToInt(), tempPS))
            {
                ret.Success = 1;
            }
            return ret;
        }
    }
}