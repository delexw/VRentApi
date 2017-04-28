using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface IBLL
    {
        ProxyUserSetting UserInfo { get; set; }
    }
}
