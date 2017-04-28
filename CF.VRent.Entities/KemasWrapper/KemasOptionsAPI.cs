using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.Entities.KEMASWSIF_CATALOGRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{
    public interface IKemasOptionsAPI
    {
        Elements getCategories();
        Elements getTypeOfJourney(string lang);
        Elements getUserTypeOfJourney(string userID, string lang);
    }

    public class KemasOptionsAPI : IKemasOptionsAPI, IDisposable
    {
        public Elements getCategories()
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getCategory(),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public Elements getTypeOfJourney(string lang)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getTypeOfJourney(lang),
                    client,
                    MethodInfo.GetCurrentMethod().Name
                );
        }

        public Elements getUserTypeOfJourney(string userID, string lang)
        {
            WSKemasPortTypeClient client = new WSKemasPortTypeClient();
            return KemasAccessWrapper.InnerTryCatchInvoker
                (
                    () => client.getUserTypeOfJourney(userID, lang),
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