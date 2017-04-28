using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CF.VRent.Entities;
using System.ServiceModel.Web;
using CF.VRent.Log;
using System.Net;
using CF.VRent.BLL;
using System.Configuration;
using CF.VRent.Entities.FapiaoPreferenceProxy;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Common;
using CF.VRent.Contract;
using System.ServiceModel;
using CF.VRent.Common.UserContracts;

namespace Proxy
{

    /// <summary>
    /// Methods related to Reservation Operation Service
    /// </summary>
    public partial class DataService
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// 
        [WebGet(UriTemplate = "FaPiaoPreferences?uid={uid}&fapiaoTypeId={fapiaoTypeId}&searchName={searchName}&offset={offset}&limit={limit}", ResponseFormat = WebMessageFormat.Json)]
        public CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference[] GetAllFapiaoPreference(string uid, int fapiaoTypeId,string searchName,int offset, int limit)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession(uid);

            CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference[] res = null;

            IFapiaoPreference Ifp = new FapiaoPreferenceImpl(setting);
            res = Ifp.GetAllFapiaoPreference(uid);
            return res;
        }
        
        [WebInvoke(UriTemplate = "FaPiaoPreferences", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference CreateFapiaoPreference(CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference fp)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference pfp = null;

            IFapiaoPreference ifp = new FapiaoPreferenceImpl(setting);
            pfp = ifp.SaveFapiaoPreference(fp);
            return pfp;
        }

        [WebGet(UriTemplate = "FaPiaoPreferences/{fpid}", ResponseFormat = WebMessageFormat.Json)]
        public CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference GetFapiaoPreferenceDetail(string fpid)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference pfp = null;

            IFapiaoPreference Ifp = new FapiaoPreferenceImpl(setting);
            pfp = Ifp.GetFapiaoPreferenceDetail(fpid);
            return pfp;
        }

        [WebInvoke(UriTemplate = "FaPiaoPreferences/{fpid}", Method = "DELETE", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void DeleteFapiaoPreference(string fpid)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            IFapiaoPreference ifp = new FapiaoPreferenceImpl(setting);
            ifp.DeleteFapiaoPreference(fpid);
        }


        [WebInvoke(UriTemplate = "FaPiaoPreferences/{fpid}", Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference UpdateFapiaoPreference(string fpid, CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference fp)
        {
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();
            CF.VRent.Entities.FapiaoPreferenceProxy.ProxyFapiaoPreference pfp = null;

            IFapiaoPreference ifp = new FapiaoPreferenceImpl(setting);
            pfp = ifp.UpdateFapiaoPreference(fp);
            return pfp;
        }

    }
}