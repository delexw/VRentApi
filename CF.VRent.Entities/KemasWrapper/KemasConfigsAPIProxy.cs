using CF.VRent.Cache;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{
    public class KemasConfigsAPIProxy : IKemasConfigsAPI
    {
        public KEMASWSIF_CONFIGRef.ConfigModel getSystemConfiguration()
        {
            var kemasObj = new KemasConfigsAPI().getSystemConfiguration();

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<ConfigModel, Error>();

            validator.Validate(kemasObj);

            return kemasObj;
        }

        public KEMASWSIF_CONFIGRef.getClientsResponse getClients(string sessionID)
        {
            var kemasObj = new KemasConfigsAPI().getClients(sessionID);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<getClientsResponse, Error>();

            validator.Validate(kemasObj);

            return kemasObj;
        }

        public KEMASWSIF_CONFIGRef.updateClientResponse getUserTypeOfJourney(KEMASWSIF_CONFIGRef.updateClientRequest request)
        {
            var kemasObj = new KemasConfigsAPI().getUserTypeOfJourney(request);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<updateClientResponse, Error>();

            validator.Validate(kemasObj);

            return kemasObj;
        }

        public KEMASWSIF_CONFIGRef.getCitiesResponse getCities(string sessionID)
        {
            var kemasObj = new KemasConfigsAPI().getCities(sessionID);

            var validator = KemasAccessWrapper.CreateKemasValidatorInstance<getCitiesResponse, Error>();

            validator.Validate(kemasObj);

            return kemasObj;
        }


        public getClientsResponse getClients(string userId, string sessionID, CacheModel cacheIt = null)
        {
            if (cacheIt != null && cacheIt.Exist(userId + "_getClients"))
            {
                return cacheIt.Get<getClientsResponse>(userId + "_getClients");
            }
            else
            {
                var clients = this.getClients(sessionID);
                cacheIt.Set(userId + "_getClients", clients);
                return clients;
            }
        }
    }
}
