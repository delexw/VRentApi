using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public interface IUserMgmtCacheChannel
    {
        UserExtension GetFromCache(int index);
        void SetToCache(UserExtension user,int index);
    }
}
