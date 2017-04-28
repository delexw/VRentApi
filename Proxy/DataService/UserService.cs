using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using CF.VRent.Log;
using System.Net;
//using CF.VRent.Contract;
using CF.VRent.Entities;
using System.Web;
using CF.VRent.Common;
using CF.VRent.BLL;
using System.Diagnostics;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.WCFExtension;
using CF.VRent.Common.UserContracts;


namespace Proxy
{

    /// <summary>
    /// Methods related to User Operation
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public partial class DataService
    {
        ///// <summary>
        ///// ForgetPassword
        ///// </summary>
        ///// <param name="uid">uid</param>
        ///// <param name="uNewPwd">password</param>
        ///// <returns></returns>
        //[WebInvoke(UriTemplate = "User/{uid}/Password", Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //public ProxyUserSetting ChangePassword(string uid,UserNewPassword uNewPwd)
        //{
        //    ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);

        //    UserSettingBLL uSettingBLL = new UserSettingBLL(setting);
        //    var res = uSettingBLL.ChangePwd(setting.Mail, setting.ID, setting.Name, setting.VName, uNewPwd);

        //    return res;
        //}


        ///// <summary>
        ///// Ping the service
        ///// </summary>
        ///// <returns>200 means successfully</returns>
        [WebGet(UriTemplate = "Ping", ResponseFormat = WebMessageFormat.Json)]
        public string Ping()
        {
            return "200";
        }
    }
}
