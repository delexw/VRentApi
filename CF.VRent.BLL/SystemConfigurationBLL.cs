using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CF.VRent.Entities;
using CF.VRent.Contract;
using CF.VRent.Entities.KEMASWSIF_CONFIGRef;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common;
using Microsoft.Practices.Unity;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Common.UserContracts;
using CF.VRent.Entities.EntityFactory;
using KU = CF.VRent.Entities.KEMASWSIF_USERRef;
using CF.VRent.UserRole;
using CF.VRent.UserStatus;

namespace CF.VRent.BLL
{
    public class SystemConfigurationBLL : AbstractBLL, ICompany, ISystemConfiguration
    {

        public SystemConfigurationBLL(ProxyUserSetting userInfo):base(userInfo)
        {
        }
        /// <summary>
        /// Get System configuration
        /// </summary>
        /// <returns></returns>
        public SystemConfig GetSystemConfiguration()
        {
            IKemasConfigsAPI config = new KemasConfigsAPI();
            var res = config.getSystemConfiguration();

            SystemConfig sysConfig = new SystemConfig()
            {
                // Problem: Is there a specification of the data range?
                // if not, do not convert , just pass through.
                // if there is one, convert is ok
                BookingAheadTime = res.BookingAheadTime,
                BookingBufferTime = res.BookingBufferTime,
                BookingMaxDuration = res.BookingMaxDuration,
                BookingMaxLeadTime = res.BookingMaxLeadTime,
                BookingMaxWaitTime = res.BookingMaxWaitTime,
                LostItemEnabled = res.LostItemEnabled,
                LostItemTime = res.LostItemTime
            };
            return sysConfig;
        }

        /// <summary>
        /// Get all companies
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserCompanyExtenstion> GetAllCompanies(UserRoleEntityCollection currentUserRole = null)
        {
            IKemasConfigsAPI api = KemasAccessWrapper.CreateKemasConfigAPIInstance();
            var userApi = KemasAccessWrapper.CreateKemasUserAPI2Instance();
            getClientsResponse clients = new getClientsResponse();
            
            IEnumerable<UserCompanyExtenstion> ret = new List<UserCompanyExtenstion>();

            if (currentUserRole.IsVRentManagerUser())
            {
                var vm = userApi.findUser2(this.UserInfo.ID, this.UserInfo.SessionID);
                if (vm.UserData != null && vm.UserData.Clients != null)
                {
                    ret = new UserCompanyFactory().CreateEntity<KU.Client>(vm.UserData.Clients);
                }
            }
            if (
                currentUserRole.IsAdministrationUser() ||
                currentUserRole.IsOperationManagerUser() ||
                currentUserRole.IsServiceCenterUser())
            {
                clients = api.getClients(this.UserInfo.ID, this.UserInfo.SessionID);
                ret = new UserCompanyFactory().CreateEntity<Client>(clients.Clients);
            }

            return ret;
        }
        /// <summary>
        /// Get all status
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserStatusEntity> GetAllUserStatus(UserRoleEntityCollection currentUserRoleKey = null)
        {
            return UserStatusContext.CreateStatusManager().Status.ToList();
        }
    }
}
