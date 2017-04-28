using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Contract;
using CF.VRent.Entities;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.EntityFactory;
using System.ServiceModel.Web;
using CF.VRent.Common.Entities;
using CF.VRent.Common;
using System.Net;
using CF.VRent.Common.UserContracts;

namespace CF.VRent.BLL
{
    public class PortalUserLoginBLL : AbstractBLL, IPortalLoginBLL
    {

        public PortalUserLoginBLL(ProxyUserSetting userInfo)
            : base(userInfo)
        {
        }
        public PortalUserLoginBLL()
            : this(null)
        {
        }
        /// <summary>
        /// Login in portal
        /// </summary>
        /// <param name="postUserSetting"></param>
        /// <returns></returns>
        public UserExtension Login(Entities.UserExtension postUserSetting)
        {
            postUserSetting.Mail = postUserSetting.Name;

            UserExtension userInfo = new UserSettingBLL().Login(postUserSetting);
            ProxyUserSetting setting = ServiceUtility.RetrieveUserInfoFromSession();

            this.UserInfo = setting;

            return new UserFactory().CreateEntity(setting);
        }

        /// <summary>
        /// Get user info in portal
        /// </summary>
        /// <returns></returns>
        public UserExtension GetUser()
        {
            //get user session
            return new UserFactory().CreateEntity(UserInfo);
        }
    }
}
