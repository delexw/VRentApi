using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.BLL.BLLFactory.UserMgmt
{
    public interface IDisableCCBAccount
    {
        /// <summary>
        /// disable ccb permission
        /// Typeofjounery = private
        /// </summary>
        /// <param name="inputUser"></param>
        /// <param name="currentUserRole"></param>
        /// <returns></returns>
        bool DisableCCBPermission(UserExtension inputUser, UserRoleEntityCollection currentUserRole);
        /// <summary>
        /// disable current user's ccb booking
        /// </summary>
        /// <param name="inputUser"></param>
        /// <param name="currentUserRole"></param>
        /// <returns></returns>
        bool DisableCCBBooking(UserExtension inputUser, UserRoleEntityCollection currentUserRole);

        /// <summary>
        /// disable current user's dcb booking
        /// </summary>
        /// <param name="inputUser"></param>
        /// <param name="currentUserRole"></param>
        /// <returns></returns>
        bool DisableDUBBooking(UserExtension inputUser, UserRoleEntityCollection currentUserRole);
    }
}
