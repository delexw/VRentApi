using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public interface IGetPriceDetails
    {
        string PriceDetails { get; }
        string PriceTotal { get; }
        ProxyBookingPrice Get(string kemasBookingId, int proxyBookingId);
    }
}
