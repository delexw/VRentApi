using CF.VRent.Common.UserContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UserRole
{
    public class RoleUtility
    {
        private static Dictionary<string, string> _vrentRoles = null;
        static RoleUtility()
        {
            _vrentRoles = new Dictionary<string, string>();
            _vrentRoles.Add(UserRoleConstants.BookingUserKey, UserRoleConstants.BookingUserKey);
            _vrentRoles.Add(UserRoleConstants.AdministratorKey, UserRoleConstants.AdministratorKey);
            _vrentRoles.Add(UserRoleConstants.OperationManagerKey, UserRoleConstants.OperationManagerKey);
            _vrentRoles.Add(UserRoleConstants.ServiceCenterKey, UserRoleConstants.ServiceCenterKey);
            _vrentRoles.Add(UserRoleConstants.VRentManagerKey, UserRoleConstants.VRentManagerKey);
        }

        public static bool IsVRentManager(ProxyUserSetting userRole) 
        {
            ProxyRole role = userRole.VrentRoles.FirstOrDefault(m => m.RoleMember.Equals( UserRoleConstants.VRentManagerKey));
            return role != null ? true : false;
        }

        public static bool IsBookingUser(ProxyUserSetting userRole)
        {
            ProxyRole role = userRole.VrentRoles.FirstOrDefault(m => m.RoleMember.Equals(UserRoleConstants.BookingUserKey));
            return role != null ? true : false;
        }

        public static bool IsAdministrator(ProxyUserSetting userRole)
        {
            ProxyRole role = userRole.VrentRoles.FirstOrDefault(m => m.RoleMember.Equals(UserRoleConstants.AdministratorKey));
            return role != null ? true : false;
        }

        public static bool IsOperationManager(ProxyUserSetting userRole)
        {
            ProxyRole role = userRole.VrentRoles.FirstOrDefault(m => m.RoleMember.Equals(UserRoleConstants.OperationManagerKey));
            return role != null ? true : false;
        }

        public static bool IsServiceCenter(ProxyUserSetting userRole)
        {
            ProxyRole role = userRole.VrentRoles.FirstOrDefault(m => m.RoleMember.Equals(UserRoleConstants.ServiceCenterKey));
            return role != null ? true : false;
        }
    }
}
