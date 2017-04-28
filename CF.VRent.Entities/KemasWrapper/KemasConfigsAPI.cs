using CF.VRent.Cache;
using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{
    public interface IKemasConfigsAPI
    {
        ConfigModel getSystemConfiguration();
        getClientsResponse getClients(string sessionID);
        getClientsResponse getClients(string userId, string sessionID, CacheModel cacheIt = null);
        updateClientResponse getUserTypeOfJourney(updateClientRequest request);
        getCitiesResponse getCities(string sessionID);
    }

    public class KemasConfigsAPI : IKemasConfigsAPI, IDisposable
    {
        public ConfigModel getSystemConfiguration()
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getSystemConfiguration(),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public getClientsResponse getClients(string sessionID)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getClients(sessionID),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public getCitiesResponse getCities(string sessionID)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getCities(sessionID),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public updateClientResponse getUserTypeOfJourney(updateClientRequest request)
        {
            WSKemasPortType2Client client = new WSKemasPortType2Client();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.updateClient(request),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public void Dispose()
        {
            //Nothing
        }


        public getClientsResponse getClients(string userId, string sessionID, CacheModel cacheIt = null)
        {
            throw new NotImplementedException();
        }
    }
}