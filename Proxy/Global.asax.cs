using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using System.Configuration;
using CF.VRent.WCFExtension;
using System.Collections.Specialized;
using System.Web.SessionState;
using CF.VRent.BLL;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Log;
using System.Timers;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.UserRole;
using CF.VRent.UserCompany;
using CF.VRent.Cache;


namespace Proxy
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();

            try
            {
                //Init cache
                var sessionId = KemasAdmin.SignOn().SessionID;
                var kemasExtApi = KemasAccessWrapper.CreateKemasExtensionAPIInstance();
                var roleManager = UserRoleContext.CreateRoleManager();
                var defaultKemasRole = roleManager.Roles[UserRoleConstants.BookingUserKey].GetDefaultKemasRole();
                kemasExtApi.GetRoleID(defaultKemasRole.Name, sessionId, CacheContext.Context.LongModel);

                var defaultEndUserCompany = UserCompanyContext.CreateCompanyManager().Companies[UserCompanyConstants.EndUserCompanyKey].GetDefaultKemasCompany();
                kemasExtApi.GetCompanyID(defaultEndUserCompany.Name, sessionId, CacheContext.Context.LongModel);

                ServiceImpInstanceFactory.CreateOptionInstance(new CF.VRent.Common.UserContracts.ProxyUserSetting()).GetAllCountries();
            }
            catch (Exception ex)
            {
                LogInfor.WriteDebug("Cache init failed", ex.ToString(), "System");
            }
            
        }

        void Application_End(object sender, EventArgs e)
        {
            CacheContext.Context.RemoveAll();
        }

        private void RegisterRoutes()
        {
            var securewebServiceHostFactory = new SecureWebServiceHostFactory();

            //for test can add this to bypass authentication and comment the authentication code in the fapiao preference code section
            //RouteTable.Routes.Add(new ServiceRoute("DataService", new WebServiceHostFactory(), typeof(DataService)));
            RouteTable.Routes.Add(new ServiceRoute("LoginService", securewebServiceHostFactory, typeof(LoginService)));

            RouteTable.Routes.Add(new ServiceRoute("PortalLoginService", securewebServiceHostFactory, typeof(PortalLoginService)));

            RouteTable.Routes.Add(new ServiceRoute("DataService", securewebServiceHostFactory, typeof(DataService)));

            RouteTable.Routes.Add(new ServiceRoute("ListenService", new WebServiceHostFactory(), typeof(ListenUnionService)));

            RouteTable.Routes.Add(new ServiceRoute("VrentAppService", securewebServiceHostFactory, typeof(VrentAppService)));
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }
        protected void Session_End(object sender, EventArgs e)
        {
        }
    }
}
