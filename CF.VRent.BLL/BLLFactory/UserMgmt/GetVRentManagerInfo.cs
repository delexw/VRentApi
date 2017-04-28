using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public class GetVRentManagerInfo : IGetVRentManagerInfo
    {
        private ProxyUserSetting _sessionUser;
        public GetVRentManagerInfo(ProxyUserSetting sessionUser)
        {
            _sessionUser = sessionUser;
        }

        public UserExtension Get(string clientId)
        {
            var defaultKemasRole = UserRoleContext.CreateRoleManager().Roles[UserRoleConstants.VRentManagerKey].GetDefaultKemasRole();

            var api = KemasAccessWrapper.CreateKemasUserAPI2Instance();

            var vrentManagerRoleId = KemasAccessWrapper.CreateKemasExtensionAPIInstance().GetRoleID(defaultKemasRole.Name, _sessionUser.SessionID);

            var kemasUser = api.getUsers2(new Entities.KEMASWSIF_USERRef.getUsers2Request()
            {
                ItemsPerPage = 10,
                ItemsPerPageSpecified = true,
                Page = 0,
                PageSpecified = true,
                SessionID = _sessionUser.SessionID,
                SearchCondition = new Entities.KEMASWSIF_USERRef.getUsers2RequestSearchCondition()
                {
                    ClientID = clientId,
                    RoleID = vrentManagerRoleId
                }
            });

            if (kemasUser.Users != null)
            {
                return new UserFactory().CreateEntity(kemasUser.Users).ToList()[0];
            }
            return new UserExtension();
            
        }
    }
}
