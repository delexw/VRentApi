using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.KEMASWSIF_AUTHRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{

    public interface IKemasAuthencation
    {
        WS_Auth_Response authByLogin(string user, string pass);
        Error logout(string sessionID);
    }

    public class KemasAuthencationAPI : IKemasAuthencation, IDisposable
    {

        public WS_Auth_Response authByLogin(string user, string pass)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.authByLogin(user, pass),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public Error logout(string sessionID)
        {
            WSKemasPortTypeClient client = null;

            return KemasAccessWrapper.InnerTryCatchInvoker
            (
                () => client.logout(sessionID),
                client,
                MethodInfo.GetCurrentMethod().Name
            );
        }

        public void Dispose()
        {
            //Nothing
        }
    }
}