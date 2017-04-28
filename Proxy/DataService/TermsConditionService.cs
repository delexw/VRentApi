using CF.VRent.Common.Entities;
using CF.VRent.Entities.TermsConditionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using CF.VRent.Common;
using CF.VRent.BLL;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.BLL.BLLFactory;

namespace Proxy
{
    public partial class DataService
    {
       
        [WebInvoke(UriTemplate = "Portal/TermsCondition", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public ReturnResult AddOrUpgradeTC(TermsCondition tc)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            tc.CreatedBy = setting.ID.ToGuidNull();
            tc.Content = HttpUtility.HtmlEncode(tc.Content);
            return ServiceImpInstanceFactory.CreateTermsConditionInstance().AddOrUpgradeTC(tc);
        }

        [WebGet(UriTemplate = "TermsCondition?tcType={type}&isIncludeContent={isIncludeContent}", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<TermsConditionExtension> GetLastestTC(string type,string isIncludeContent)
        {
            return ServiceImpInstanceFactory.CreateTermsConditionInstance().GetLastestTC(type, isIncludeContent.ToInt());
        }

        [WebInvoke(UriTemplate = "User/TermsCondition?tcType={type}", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public ReturnResult AcceptTC(string type)
        {
            ReturnResult ret = new ReturnResult();

            var bll = ServiceImpInstanceFactory.CreateTermsConditionInstance();
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            var tcList = bll.GetLastestTC(type, 0);
            foreach (TermsConditionExtension tc in tcList)
            {
                var utc = new UserTermsConditionAgreement();
                utc.TCID = tc.TC.ID;
                utc.CreatedBy = setting.ID.ToGuidNull();
                utc.UserID = setting.ID.ToGuid();
                ret = bll.AcceptTC(utc);
            }
            return ret;
        }
    }
}