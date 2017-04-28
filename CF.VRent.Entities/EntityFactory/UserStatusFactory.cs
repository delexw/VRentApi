using CF.VRent.Common.Entities;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.UserStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities.EntityFactory
{
    /// <summary>
    /// Convert status value from kemas properties to user status flag
    /// </summary>
    public class UserStatusFactory
    {
        private enum ConstructorType
        {
            KemasUserData,
            ProxyUserData,
            UserExtensionData
        }

        private UserData2 _kemasUser;
        private ProxyUserSetting _proxyUser;
        private UserExtension _userExt;

        private ConstructorType _flag;

        public UserStatusFactory(UserData2 kemasUser)
        {
            if (kemasUser == null)
            {
                throw new ArgumentNullException("kemasUser", "Not null");
            }
            _kemasUser = kemasUser;
            _flag = ConstructorType.KemasUserData;
        }

        public UserStatusFactory(ProxyUserSetting kemasUser)
        {
            if (kemasUser == null)
            {
                throw new ArgumentNullException("kemasUser", "Not null");
            }
            _proxyUser = kemasUser;
            _flag = ConstructorType.ProxyUserData;
        }

        public UserStatusFactory(UserExtension userExt)
        {
            if (userExt == null)
            {
                throw new ArgumentNullException("userExt", "Not null");
            }
            _userExt = userExt;
            _flag = ConstructorType.UserExtensionData;
        }

        /// <summary>
        /// Convert 4 status from kemas
        /// License Approve or reject, KEMAS Block, KEMAS deactive
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public virtual UserStatusEntityCollection CreateEntity(UserStatusEntityCollection collection)
        {
            switch (_flag)
            {
                case ConstructorType.KemasUserData:
                    //License Approve or reject
                    //Disabled the logic as kemas doesn't support pending status
                    #region Disabled segment
                    //if (_kemasUser.License != null)
                    //{
                    //    collection["4"].Value = _kemasUser.License.State == 0 ? 1 : 0;
                    //    collection["5"].Value = _kemasUser.License.State == 2 ? 1 : 0;
                    //} 
                    #endregion

                    //KEMAS Block
                    //collection["B"].Value = _kemasUser.Blocked;

                    //KEMAS deactive
                    //collection["C"].Value = _kemasUser.Enabled ^ 1;
                    break;
                case ConstructorType.ProxyUserData:
                    //KEMAS Block
                    //collection["B"].Value = _proxyUser.Blocked;

                    //KEMAS deactive
                    //collection["C"].Value = _proxyUser.Enabled ^ 1;
                    break;
                case ConstructorType.UserExtensionData:
                    //License Approve or reject
                    //Disabled the logic as kemas doesn't support pending status
                    #region Disabled segment
                    //if (_userExt.License != null)
                    //{
                    //    collection["4"].Value = _userExt.License.State == 0 ? 1 : 0;
                    //    collection["5"].Value = _userExt.License.State == 2 ? 1 : 0;
                    //} 
                    #endregion

                    //KEMAS Block
                    //collection["B"].Value = _userExt.Blocked;

                    //KEMAS deactive
                    //collection["C"].Value = _userExt.Enabled ^ 1;
                    break;
            }
            

            return collection;
        }
    }
}
