using CF.VRent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public interface IGetVRentManagerInfo
    {
        /// <summary>
        /// Get the vrent manager info
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        UserExtension Get(string clientId);
    }
}
