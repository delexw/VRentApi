using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface IIndirectFeeOperation:IBLL
    {
        int AddOrderItems(int proxyBookingID, ProxyOrderItem[] orderItems);
    }
}
