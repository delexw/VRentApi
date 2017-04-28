using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CF.VRent.Entities.KemasWrapper
{
    public class KemasCompanyAPI: IDisposable
    {
        public updateClientResponse UpdateCompany(updateClientRequest request)
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
        }
    }
}
