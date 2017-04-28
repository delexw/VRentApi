using CF.VRent.BLL;
using CF.VRent.Common.Entities;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using CF.VRent.Entities.EntityFactory;
using Microsoft.Practices.Unity;
using CF.VRent.Common.UserContracts;
using CF.VRent.BLL.BLLFactory;

namespace Proxy
{
    /// <summary>
    /// Login portal
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PortalLoginService : IPortalLogin
    {
        [WebInvoke(Method = "POST", UriTemplate = "Login",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public UserExtension Login(UserExtension postUserSetting)
        {
            IPortalLoginBLL bll = ServiceImpInstanceFactory.CreatePortalLoginInstance();
            var user = bll.Login(postUserSetting);
            HttpContext.Current.Session["UserSetting"] = bll.UserInfo;
            return user;
        }

        [WebGet(UriTemplate = "Login", ResponseFormat = WebMessageFormat.Json)]
        public UserExtension GetUser()
        {
            //get user session
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            IPortalLoginBLL bll = ServiceImpInstanceFactory.CreatePortalLoginInstance();
            bll.UserInfo = setting;
            return bll.GetUser();
        }
    }
}