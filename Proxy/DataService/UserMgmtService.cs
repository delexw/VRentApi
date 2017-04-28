using CF.VRent.BLL;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using CF.VRent.Common;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using CF.VRent.Contract;
using Microsoft.Practices.Unity;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Common.UserContracts;
using CF.VRent.BLL.BLLFactory;
using CF.VRent.UserStatus;

namespace Proxy
{
    /// <summary>
    /// User management api
    /// </summary>
    public partial class DataService
    {
        #region
        [WebInvoke(UriTemplate = "Portal/Clients", Method="POST", ResponseFormat = WebMessageFormat.Json)]
        public UserCompanyExtenstion CreateClient(CompanyProfileRequest companyProfile)
        {
            ProxyUserSetting userinfo = ServiceUtility.RetrieveUserInfoFromSession();
            ICompanyBLL cb = ServiceImpInstanceFactory.CreateClientMgmtInstance(userinfo);
            return cb.CreateCompany(companyProfile);
        }

        [WebInvoke(UriTemplate = "Portal/Clients/{ClientID}", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public UserCompanyExtenstion UpdateClient(string ClientID, UserCompanyExtenstion companyProfile)
        {
            ProxyUserSetting userinfo = ServiceUtility.RetrieveUserInfoFromSession();
            ICompanyBLL cb = ServiceImpInstanceFactory.CreateClientMgmtInstance(userinfo);
            return cb.UpdateCompany(companyProfile);
        }

        [WebGet(UriTemplate = "Portal/Clients", ResponseFormat = WebMessageFormat.Json)]
        public UserCompanyExtenstion[] RetrieveClients()
        {
            ProxyUserSetting userinfo = ServiceUtility.RetrieveUserInfoFromSession();
            CompanyBLL cb = new CompanyBLL(userinfo);
            return cb.RetrieveCompanys();
        }

        [WebGet(UriTemplate = "Portal/Clients/{ClientID}", ResponseFormat = WebMessageFormat.Json)]
        public UserCompanyExtenstion RetrieveClientByID(string ClientID)
        {
            ProxyUserSetting userinfo = ServiceUtility.RetrieveUserInfoFromSession();
            CompanyBLL cb = new CompanyBLL(userinfo);
            return cb.RetrieveCompanyByID(ClientID);
        }

        [WebInvoke(UriTemplate = "Portal/Clients/{ClientID}",Method="POST", ResponseFormat = WebMessageFormat.Json)]
        public UserCompanyExtenstion EnableDisableClient(string ClientID,Status status)
        {
            ProxyUserSetting userinfo = ServiceUtility.RetrieveUserInfoFromSession();
            //CompanyBLL cb = new CompanyBLL(userinfo);
            ICompanyBLL cb = ServiceImpInstanceFactory.CreateClientMgmtInstance(userinfo);
            return cb.EnableDisableCompany(ClientID, status.status);
        }

        #endregion

        [WebGet(UriTemplate = "Portal/Users/{userId}",ResponseFormat= WebMessageFormat.Json)]
        public UserExtension GetUserDetial(string userId)
        {
            var bll = ServiceImpInstanceFactory.CreateUserMgmtInstance();

            return bll.GetUserDetail(userId);
        }

        [WebGet(UriTemplate = "Portal/Users?itemsPerPage={itemsPerPage}&pageNumber={pageNumber}&status={status}&companyName={companyName}&userName={userName}&name={name}&phone={phone}", ResponseFormat = WebMessageFormat.Json)]
        public EntityPager<UserExtensionHeader> GetUserList(int itemsPerPage,
            int pageNumber,
            string status, string companyName, string userName, string name, string phone)
        {
            //Seperate status
            var where = new UserExtension() { Status = status, Company = companyName, Mail = userName, Name = name, Phone = phone };

            var bll = ServiceImpInstanceFactory.CreateUserMgmtInstance();

            return bll.GetUserList(itemsPerPage.ToInt(), pageNumber.ToInt(), where);
        }

        [WebGet(UriTemplate = "Portal/Users/CompanyPendUsers", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<UserExtensionHeader> GetCompanyPendingUserList()
        {
            //Company pending
            var where = new UserExtension() { Status = "6" };

            var bll = ServiceImpInstanceFactory.CreateUserMgmtInstance();

            return bll.GetCompanyUserList(where);
        }

        [WebGet(UriTemplate = "Companies", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<UserCompanyExtenstion> GetCompanies()
        {
            var bll = ServiceImpInstanceFactory.CreateCompanyInstance();
            return bll.GetAllCompanies();
        }

        [WebGet(UriTemplate = "Portal/UserStatus", ResponseFormat = WebMessageFormat.Json)]
        public IEnumerable<UserStatusEntity> GetStatus()
        {
            var bll = ServiceImpInstanceFactory.CreateSystemConfigurationInstance();

            return bll.GetAllUserStatus();
        }

        [WebInvoke(Method = "POST", UriTemplate = "Portal/User", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public UserExtension CreateCorpUser(UserExtension user)
        {
            var bll = ServiceImpInstanceFactory.CreateUserMgmtInstance();
            return bll.CreateCorpUser(user);
        }

        [WebInvoke(Method = "PUT", UriTemplate = "Portal/Users/{userId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public UserExtension UpdateUser(UserExtension user, string userId)
        {
            var bll = ServiceImpInstanceFactory.CreateUserMgmtInstance();
            user.ID = userId;
            return bll.UpdateUser(user);
        }
    }
}