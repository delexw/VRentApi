using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities;
using CF.VRent.Entities.EntityFactory;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Contract
{
    public interface IUserMgmt:IBLL
    {
        /// <summary>
        /// Get users by page
        /// </summary>
        /// <param name="itemsPerPage"></param>
        /// <param name="pageNumber"></param>
        /// <param name="status">-1 = all</param>
        /// <param name="companyID">Client ID</param>
        /// <param name="userName">User Name</param>
        /// <param name="name">User Full Name</param>
        /// <param name="phone">User Phone Name</param>
        /// <returns></returns>
        EntityPager<UserExtensionHeader> GetUserList(
            int itemsPerPage,
            int pageNumber,
            UserExtension where, UserRoleEntityCollection currentUserRoleKey = null);

        /// <summary>
        /// Get users with company pending status
        /// </summary>
        /// <param name="where"></param>
        /// <param name="currentUserRoleKey"></param>
        /// <returns></returns>
        IEnumerable<UserExtensionHeader> GetCompanyUserList(UserExtension where, UserRoleEntityCollection currentUserRoleKey = null);

        /// <summary>
        /// Get user details
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserExtension GetUserDetail(string userId, UserRoleEntityCollection currentUserRoleKey = null);

        /// <summary>
        /// Create a corperate user in Portal
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        UserExtension CreateCorpUser(UserExtension user, string roleKey = null ,UserRoleEntityCollection currentUserRoleKey = null);

        /// <summary>
        /// Modifiy user info
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        UserExtension UpdateUser(UserExtension user, UserRoleEntityCollection currentUserRoleKey = null);
    }
}
