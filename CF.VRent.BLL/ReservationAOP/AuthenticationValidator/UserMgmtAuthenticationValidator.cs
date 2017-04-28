using CF.VRent.Entities;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.ReservationAOP.AuthenticationValidator
{
    public class UserMgmtAuthenticationValidator:IPortalAuthenticationValidator
    {
        public bool Validate(IMethodInvocation input, IPortalCertification Cert)
        {
            //Create & Edit
            if (input.MethodBase.Name == "CreateCorpUser" ||
                input.MethodBase.Name == "UpdateUser")
            {
                var updateUser = input.Arguments[0] as UserExtension;
                //Deactive, Apporve to booking car or reactived
                if ((updateUser.Status == "9" || updateUser.Status == "4" || updateUser.Status == "E" || updateUser.Status == "A") && input.MethodBase.Name == "UpdateUser")
                {
                    //only vrent manger can't deactive or active user
                    if (Cert.User.RoleEntities.IsVRentManagerUser() &&
                        !Cert.User.RoleEntities.IsServiceCenterUser() &&
                        !Cert.User.RoleEntities.IsAdministrationUser() &&
                        !Cert.User.RoleEntities.IsOperationManagerUser())
                    {
                        return false;
                    }
                }

                //vrent manager approve or reject
                if ((updateUser.Status == "7" || updateUser.Status == "8") && input.MethodBase.Name == "UpdateUser")
                {
                    if (Cert.User.RoleEntities.IsOperationManagerUser() &&
                        !Cert.User.RoleEntities.IsAdministrationUser() &&
                        !Cert.User.RoleEntities.IsServiceCenterUser() &&
                        !Cert.User.RoleEntities.IsAdministrationUser())
                    {
                        return false;
                    }
                }
            }
            else if (input.MethodBase.Name == "GetCompanyPendingUserList")
            {
                if (!Cert.User.RoleEntities.IsVRentManagerUser())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
