using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Email
{
    public abstract class EmailConstants
    {
        public static string ServiceCenterKey { get { return "SC"; } }
        public static string OperationManagerKey { get { return "SCL"; } }
        public static string AdministrationKey { get { return "ADMIN"; } }
        public static string TestUserKey { get { return "TestUser"; } }
        public static string DebitNoteInternalUserKey { get { return "DN_Internal"; } }
    }
}
