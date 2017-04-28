using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using CF.VRent.Entities;

namespace CF.VRent.Contract
{
    public interface ICategory
    {
        List<ProxyCategory> GetAllCategories(Lang lang);
    }
}
