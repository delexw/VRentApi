
using CF.VRent.Cache;
using CF.VRent.Entities.DataAccessProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface IOptionsBLL
    {
        IEnumerable<Country> GetAllCountries(CacheModel cacheIt = null);
    }
}
