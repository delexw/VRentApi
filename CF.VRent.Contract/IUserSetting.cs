using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using System.ServiceModel;
using System.ServiceModel.Web;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.Contract
{
    /// <summary>
    /// Interface for user setting
    /// </summary>
    public interface IUserSetting
    {
        /// <summary>
        /// Get user setting information
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>User setting object</returns>
        ProxyUserSetting GetUserSetting(string id);

        /// <summary>
        /// Login for user
        /// </summary>
        /// <param name="uname">user name</param>
        /// <param name="upwd">user password</param>
        /// <returns></returns>
        ProxyUserSetting Login(string uname, string upwd);

        /// <summary>
        /// Login web user
        /// </summary>
        /// <param name="uname"></param>
        /// <param name="upwd"></param>
        /// <returns></returns>
        int LoginWeb(string uname, string upwd);
   
    }
}
