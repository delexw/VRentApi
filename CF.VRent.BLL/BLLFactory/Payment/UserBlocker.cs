using CF.VRent.Common;
using CF.VRent.Entities;
using CF.VRent.Entities.KemasWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common.Entities;
using CF.VRent.UserStatus.Interfaces;

namespace CF.VRent.BLL.BLLFactory.Payment
{
    public  class UserBlocker:IUserBlocker
    {
        public UserBlocker(UserExtension admin, UserExtension currentUser)
        {
            if (admin == null)
            {
                throw new ArgumentNullException("admin", "admin is null");
            }
            if (currentUser == null)
            {
                throw new ArgumentNullException("currentUser", "currentUser is null");
            }
            _admin = admin;
            _current = currentUser;
        }

        /// <summary>
        /// Deactive DUB
        /// </summary>
        /// <returns></returns>
        public virtual bool DeactiveDUB()
        {
            IKemasUserAPI userApi = UnityHelper.GetUnityContainer("KemasApiWapperContainer").Resolve<IKemasUserAPI>();

            var kemasUser = userApi.findUser2(_current.ID, admin.SessionID);

            if (kemasUser.UserData != null)
            {
                var userStatusMg = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserStatusManager>(new ParameterOverride("binaryPattern", kemasUser.UserData.Status));

                var statusFlow =  ServiceImpInstanceFactory.CreateUserTransferFlowStrategyInstance(null);

                //Deactive-DUB
                statusFlow.ExternalDeactiveDUB(_current, ref userStatusMg);
                
                var deactiveUser = userApi.updateUser2(new Entities.KEMASWSIF_USERRef.updateUser2Request()
                {
                    SessionID = admin.SessionID,
                    Language = "english",
                    UserData = new Entities.KEMASWSIF_USERRef.updateUserData()
                    {
                        ID = _current.ID,
                        Mail = kemasUser.UserData.Mail,
                        Status = _current.Status
                    }
                });

                userStatusMg = UnityHelper.GetUnityContainer("UserMgmtContainer").Resolve<IUserStatusManager>(new ParameterOverride("binaryPattern", deactiveUser.UserData.Status));

                if (userStatusMg.Status["A"].Value == 1)
                {
                    return true;
                }
                else
                {
                    throw new VrentApplicationException(new Common.Entities.ReturnResult()
                    {
                        Code = MessageCode.CVB000020.ToString(),
                        Message = String.Format(MessageCode.CVB000020.GetDescription(), deactiveUser.UserData.Mail, deactiveUser.UserData.Status),
                        Type = Common.Entities.ResultType.VRENT
                    });
                }
            }
            else
            {
                return false;
            }
        }

        private UserExtension _admin;
        public Entities.UserExtension admin
        {
            get { return _admin; }
        }

        private UserExtension _current;
        public Entities.UserExtension currentUser
        {
            get { return _current; }
        }
    }
}
