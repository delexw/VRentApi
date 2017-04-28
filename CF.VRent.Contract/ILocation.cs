using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Entities.FapiaoPreferenceProxy;

namespace CF.VRent.Contract
{
    public interface ILocation
    {
        List<ProxyLocation> GetALLLocation(string uid);
    }
}
