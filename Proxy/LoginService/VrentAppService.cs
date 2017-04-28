using CF.VRent.BLL;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace Proxy
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class VrentAppService : IVrentAppService
    {

        #region New Registration API In APP

        [WebInvoke(UriTemplate = "Users?tcId={tcId}&Lang={lang}", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public UserExtension Register(UserExtension feReg, string tcId, string lang)
        {
            UserExtension RegUser = null;
            var termConditionBll = new TermsConditionBLL();
            ReturnResult incrementalTerms = termConditionBll.AcceptTC(new CF.VRent.Entities.TermsConditionService.UserTermsConditionAgreement() { TCID = Convert.ToInt32( tcId), UserID = Guid.NewGuid(), CreatedBy = Guid.NewGuid() });

            if (incrementalTerms.Success == 1)
            {
                //AppRegistrationBLL burb = new AppRegistrationBLL();
                IAppRegistrationBLL burb = ServiceImpInstanceFactory.CreateAppRegistrationInstance();
                RegUser = burb.UserRegistration(feReg, lang);
            }

            return RegUser;
        }

        [WebInvoke(UriTemplate = "Users/{userID}?Lang={lang}", Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public UserExtension UpdateProfile(string userID, UserExtension user, string lang)
        {
            //get user session
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(userID);


            //AppRegistrationBLL burb = new AppRegistrationBLL(setting);
            IAppRegistrationBLL burb = ServiceImpInstanceFactory.CreateAppRegistrationInstance(setting);
            return burb.UpdateProfile(user, lang);
        }

        [WebInvoke(UriTemplate = "Users/{userID}/ChangePWD?Lang={lang}", Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public UserExtension ChangePassword(string userID, UserExtension user, string lang)
        {
            //get user session
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(userID);

            AppRegistrationBLL burb = new AppRegistrationBLL(setting);
            return burb.ChangePassword(user, lang);
        }


        #endregion

    }
}