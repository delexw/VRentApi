
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UserRole
{
    public abstract class UserRoleConstants
    {
        public static string ServiceCenterKey { get { return "SC"; } }
        public static string VRentManagerKey { get { return "VM"; } }
        public static string OperationManagerKey { get { return "SCL"; } }
        public static string AdministratorKey { get { return "ADMIN"; } }
        public static string BookingUserKey { get { return "BU"; } }
    }
}
