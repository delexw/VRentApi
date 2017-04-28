using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Contract;

using CF.VRent.Common;
using System.ServiceModel.Web;
using System.Net;
using System.Web;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using CF.VRent.Log;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.DataAccessProxyWrapper;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.BLL
{
    /// <summary>
    /// BLL for user setting information
    /// </summary>
   public  class UserSettingBLL:AbstractBLL
    {
        public UserSettingBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        {
        }
        public UserSettingBLL()
            : this(null)
        {
        }

       /// <summary>
       /// Login with username and password
       /// </summary>
       /// <param name="uname">user name</param>
       /// <param name="upwd">user pwd</param>
       /// <returns></returns>
       public UserExtension Login(string uname, string upwd)
       {
           UserExtension userInfo = null;

           upwd = Encrypt.GetPasswordFormat(upwd);

           KemasAuthencationAPI clientLogin = new KemasAuthencationAPI();
           var auth = clientLogin.authByLogin(uname, upwd);

           if (auth != null && Convert.ToInt32(auth.Result) == 0)
           {
               KemasUserAPI kemasUsers = new KemasUserAPI();
               findUser2Response findUser = kemasUsers.findUser2(auth.ID, auth.SessionID);

               if (findUser.UserData != null)
               {
                   userInfo = UserRegistrationConst.AssembleUserExtention(findUser.UserData, auth);

                   ProxyUserSetting setting = ServiceUtility.ConvertFromUserExtention(userInfo);
                   setting.SessionID = auth.SessionID;

                   //Proxy roles
                   List<ProxyRole> roleList = new List<ProxyRole>();
                   if (findUser.UserData.Roles != null)
                   {
                       foreach (Role item in findUser.UserData.Roles)
                       {
                           ProxyRole role = new ProxyRole() { RoleMember = item.Name };
                           roleList.Add(role);
                       }
                       setting.AllRoles = roleList.ToArray();
                   }

                   //Keep the kemas status
                   userInfo.ChangePwd = auth.ChangePwd.ToInt();
                   setting.ChangePwd = auth.ChangePwd.ToInt();

                   if (HttpContext.Current != null)
                   {

                       HttpContext.Current.Session.Add(ServiceUtility.UserSettings, setting);
                   }
               }
               else
               {
                   throw new VrentApplicationException(findUser.Error.ErrorCode, findUser.Error.ErrorMessage, ResultType.KEMAS);
               }

           }
           else
           {
               throw new WebFaultException<ReturnResult>(new ReturnResult()
               {
                   Success = 0,
                   Code = MessageCode.CVCE000006.ToString(),
                   Message = MessageCode.CVCE000006.GetDescription(),
                   Type = MessageCode.CVCE000006.GetMessageType()
               }, HttpStatusCode.InternalServerError);

           }

           userInfo.RoleEntities = null;
           return userInfo;
       }

       #region New Login API

       public static void AppendUserIdentity(WS_Auth_Response auth,UserExtension ue) 
       {
           KemasUserAPI client = new KemasUserAPI();
           Right[] rights = client.getRights(ue.ID);
           ue.Rights = rights;

           //KemasUserAPI roleClient = new KemasUserAPI();
           //getRolesResponse rolesRes = roleClient.getRoles(auth.SessionID);

           //ue.Roles = rolesRes.Roles;
       }


       public UserExtension Login(UserExtension ueLogin)
       {
           UserExtension backend = null;
           if (!string.IsNullOrEmpty(ueLogin.Mail) && !string.IsNullOrEmpty(ueLogin.Password))
           {
               backend = this.Login(ueLogin.Mail, ueLogin.Password);
           }
           else
           {
               throw new VrentApplicationException(ErrorConstants.BadLoginDataCode, string.Format(ErrorConstants.BadLoginDataMessage, ueLogin.Mail, ueLogin.Password), ResultType.VRENTFE);
           }

           backend.RoleEntities = null;
           return backend;
       }

       #endregion

       //public ProxyUserSetting ChangePwd(string uName, string uid, string name, string vName, UserNewPassword uNewPwd) 
       //{
       //    var res = Login(uName, uNewPwd.Current);

       //    //If current user password correct
       //    if (res != null && res.Result == 0)
       //    {
       //        //ProxyUser user = new ProxyUser();
       //        //user.ID = uid;
       //        //user.Password = Encrypt.GetPasswordFormat(uNewPwd.New);
       //        //user.Name = name;
       //        //user.VName = vName;
       //        //user.ChangePwd = uNewPwd.ChangePwd;
       //        //user.CurrentPassword = uNewPwd.Current;

       //        KemasUserAPI kemasUser = new KemasUserAPI();
       //        UserData ud = new UserData();
       //        ud.ID = res.ID;
       //        ud.Password = Encrypt.GetPasswordFormat(uNewPwd.New);
       //        ud.CurrentPassword = uNewPwd.Current;

       //        string resultStr = kemasUser.updateUser(res.ID, ud);

       //        int result = Int32.Parse(resultStr);

       //        if (result == 0)
       //        {
       //            ReponseRes<string> rRes = new ReponseRes<string>()
       //            {
       //                Result = 1,
       //                message = string.Empty,
       //                Data = string.Empty
       //            };

       //            var newUserSetting = Login(uName, uNewPwd.New);
       //            newUserSetting.UName = uName;
       //            HttpContext.Current.Session["UserSetting"] = newUserSetting;

       //            return newUserSetting;
       //        }
       //        else
       //        {
       //            var webEx = new WebFaultException<ReturnResult>(new ReturnResult()
       //            {
       //                Success = 0,
       //                Code = MessageCode.CVCE000005.ToString(),
       //                Message = MessageCode.CVCE000005.GetDescription(),
       //                Type = MessageCode.CVCE000005.GetMessageType()
       //            }, HttpStatusCode.BadRequest);

       //            throw webEx;
       //        }

               
       //    }
       //    // if the old username and password is not correct
       //    else
       //    {
       //        var webEx = new WebFaultException<ReturnResult>(new ReturnResult()
       //        {
       //            Success = 0,
       //            Code = MessageCode.CVCE000002.ToString(),
       //            Message = MessageCode.CVCE000002.GetDescription(),
       //            Type = MessageCode.CVCE000002.GetMessageType()
       //        }, HttpStatusCode.Unauthorized);
              
       //        throw webEx;
       //    }
       //}


    }
}
