using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;

namespace CF.VRent.Contract
{
    public interface IDriver
    {
        List<ProxyDriver> FindAllDrivers(string uid);
    }
}
