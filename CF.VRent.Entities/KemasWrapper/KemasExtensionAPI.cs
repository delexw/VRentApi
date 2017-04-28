
using CF.VRent.Cache;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{
    public interface IKemasExtensionAPI
    {
        string GetRoleID(string roleName, string sessionId, CacheModel cacheIt = null);
        string GetCompanyID(string companyName, string sessionId, CacheModel cacheIt = null);
        string GetCompanyName(string clientId, string sessionId);
    }

    public class KemasExtensionAPI : IKemasExtensionAPI
    {
        /// <summary>
        /// Get role id by rolename
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public string GetRoleID(string roleName, string sessionId, CacheModel cacheIt = null)
        {
            getRolesResponse kemasObj = null;

            if (cacheIt != null && cacheIt.Exist("GetRoleID_" + roleName))
            {
                kemasObj = cacheIt.Get<getRolesResponse>("GetRoleID_" + roleName);
            }
            else
            {
                var api = new KemasUserAPIProxy();
                kemasObj = api.getRoles(sessionId);
                var validator = KemasAccessWrapper.CreateKemasValidatorInstance<getRolesResponse, CF.VRent.Entities.KEMASWSIF_USERRef.Error>();
                validator.Validate(kemasObj);
                cacheIt.Set("GetRoleID_" + roleName, kemasObj);
            }

            if (kemasObj.Roles != null)
            {
                var role = kemasObj.Roles.FirstOrDefault(r => r.Name.Trim() == roleName.Trim());
                if (role != null)
                {
                    return role.ID;
                }
            }
            return null;
        }

        /// <summary>
        /// Get company id by companyname
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public string GetCompanyID(string companyName, string sessionId, CacheModel cacheIt = null)
        {

            getClientsResponse kemasObj = null;

            if (cacheIt != null && cacheIt.Exist("GetCompanyID_" + companyName)) 
            {
                kemasObj = cacheIt.Get<getClientsResponse>("GetCompanyID_" + companyName);
            }
            else
            {
                var api = new KemasConfigsAPIProxy();

                kemasObj = api.getClients(sessionId);

                var validator = KemasAccessWrapper.CreateKemasValidatorInstance<getClientsResponse, CF.VRent.Entities.KEMASWSIF_CONFIGRef.Error>();

                validator.Validate(kemasObj);

                cacheIt.Set("GetCompanyID_" + companyName, kemasObj);
            }

            if (kemasObj.Clients != null)
            {
                var client = kemasObj.Clients.FirstOrDefault(r => r.Name.Trim() == companyName.Trim());
                if (client != null)
                {
                    return client.ID;
                }
            }

            return null;
        }

        /// <summary>
        /// Get company name by company id
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public string GetCompanyName(string clientId, string sessionId)
        {
            var api = new KemasConfigsAPIProxy();

            var kemasObj = api.getClients(sessionId);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<getClientsResponse, CF.VRent.Entities.KEMASWSIF_CONFIGRef.Error>();

            validator.Validate(kemasObj);

            if (kemasObj.Clients != null)
            {
                var client = kemasObj.Clients.FirstOrDefault(r => r.ID == clientId);
                if (client != null)
                {
                    return client.Name;
                }
            }

            return null;
        }
    }
}
