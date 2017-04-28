using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using CF.VRent.Log;
using System.Net;
using CF.VRent.Contract;
using CF.VRent.Entities;
using System.Web;
using CF.VRent.Common;
using System.Threading;
using VWFSCN.IT.Log;
using CF.VRent.BLL;
using System.Configuration;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.DataAccessProxyWrapper;
using System.Web.SessionState;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Cache;
namespace Proxy
{
    /// <summary>
    /// Login Service  offer methods for login and 
    /// </summary>
    [ServiceContract]
    public interface ILoginService : IDisposable
    {
        #region New Login API
        [OperationContract]
        UserExtension LoginNew(UserExtension ue);

        [OperationContract]
        UserExtension RetrieveProfile();

        #endregion

        [OperationContract]
        ForgotPwdRes ForgotPassword(ForgotPasswordPara postForgotPwdPara);

        [OperationContract]
        bool Logout();

        [OperationContract]
        string Ping();

        [OperationContract]
        [WebGet(UriTemplate = "UserStatus?userId={userId}", ResponseFormat = WebMessageFormat.Json)]
        ProxyUserSetting GetUserStatus(string userId);
    }


    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode=ConcurrencyMode.Multiple)]
    public class LoginService : ILoginService
    {

        private UserBLL userBLL;
        private UserSettingBLL userSettingBLL;
        public LoginService()
        {
           
            userBLL = new UserBLL();
            userSettingBLL = new UserSettingBLL();
           
        }

        [WebInvoke(UriTemplate = "LoginNew", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public UserExtension LoginNew(UserExtension ue)
        {
            UserExtension output = null;
            UserSettingBLL usb = new UserSettingBLL();
            output = usb.Login(ue);
            return output;

        }


        [WebGet(UriTemplate = "LoginNew", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public UserExtension RetrieveProfile()
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            AppRegistrationBLL burb = new AppRegistrationBLL(setting);
            return burb.RetrieveProfile();
        }

        /// <summary>
        /// ForgetPassword
        /// </summary>
         [WebInvoke(UriTemplate = "ForgotPwd", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //[WebGet(UriTemplate = "ForgotPwd?mail={mail}&uname={uName}&lang={lang}", ResponseFormat = WebMessageFormat.Json)]
        public ForgotPwdRes ForgotPassword(ForgotPasswordPara postForgotPwdPara)
        {
            //check if it is kemas user first.
            var res = userBLL.ForgotPassword(postForgotPwdPara.Email, postForgotPwdPara.Lang);

            if (res.Result == 0 && res.success == 1)
            {
                return res;
            }
            else
            {
                var webEx = new WebFaultException<ReturnResult>(new ReturnResult()
                {
                    Code = MessageCode.CVB000001.ToString(),
                    Message = MessageCode.CVB000001.GetDescription(),
                    Type = MessageCode.CVB000001.GetMessageType(),
                    Success = res.success
                }, HttpStatusCode.BadRequest);
                throw webEx;
            }
        }

        /// <summary>
        /// Log out of the system
        /// </summary>
        /// <returns></returns>
         [WebGet(UriTemplate = "Logout", ResponseFormat = WebMessageFormat.Json)]
         public bool Logout()
         {
             //Remove cache
             var user = ServiceUtility.RetrieveUserInfoFromSession();
             CacheContext.Context.Remove(user.ID);

             HttpSessionState session =  HttpContext.Current.Session;
             if(session != null)
             {
                 session.Abandon();
             }
             return true;
         }

        /// <summary>
        /// Ping the service 
        /// </summary>
        /// <returns>Return 200 means successfully</returns>
         [WebGet(UriTemplate = "Ping", ResponseFormat = WebMessageFormat.Json)]
         public string Ping()
         {
             return "200";
         }

         public void Dispose()
         {
             
         }


         public ProxyUserSetting GetUserStatus(string userId)
         {
             ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

             if (!setting.ID.Trim().Equals(userId))
             {
                 throw new WebFaultException<ReturnResult>(new ReturnResult() {
                     Code = MessageCode.CVCE000002.ToString(),
                     Message = MessageCode.CVCE000002.GetDescription(),
                     Type = MessageCode.CVCE000002.GetMessageType()
                 },HttpStatusCode.Unauthorized);
             }
             HttpContext.Current.Session["UserSetting"] = new UserBLL().GetUserStatus(setting);
             return setting;
         }
    }
}