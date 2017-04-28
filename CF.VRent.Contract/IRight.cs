using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.Contract
{
    public interface IRight
    {
        List<ProxyRight> GetAllRights(string uid);
    }
}
