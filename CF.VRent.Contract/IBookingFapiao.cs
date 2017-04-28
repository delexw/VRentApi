using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface IBookingFapiao
    {
        ProxyFapiao[] RetrieveFapiaos(Guid uid);

        ProxyFapiao RetrieveFapiaoDetail(int faPiaoID);
    }
}
