using CF.VRent.Common.Entities.UserExt;
using CF.VRent.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities.EntityFactory
{
    /// <summary>
    /// Create a entity including fields been allowed to be exposed by user role
    /// </summary>
    public class UserPermissionFactory
    {
        /// <summary>
        /// Create user entity with different role
        /// </summary>
        /// <param name="root"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual UserExtension CreateEntity(UserExtension root, UserRoleEntityCollection roles)
        {
            if (root != null)
            {
                if (roles != null)
                {
                    return root;
                }
            }

            return new UserExtension();
        }

        /// <summary>
        /// Create user entities with different role
        /// </summary>
        /// <param name="roots"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public IEnumerable<UserExtension> CreateEntity(IEnumerable<UserExtension> roots, UserRoleEntityCollection roles)
        {
            if (roots != null)
            {
                List<UserExtension> extensions = new List<UserExtension>();

                foreach (UserExtension ue in roots)
                {
                    extensions.Add(this.CreateEntity(ue, roles));
                }

                return extensions;
            }

            return null;
        }

        /// <summary>
        /// Condition permission
        /// </summary>
        /// <param name="root"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual UserExtension CreateUserConditionEntity(UserExtension root, UserRoleEntityCollection roles)
        {
            if (root != null && roles!=null)
            {
                //VM
                var perManager = new UserExtension();
                if (roles.IsVRentManagerUser())
                {
                    perManager.Mail = root.Mail;
                    perManager.Name = root.Name;
                    perManager.Status = root.Status;
                }
                if (roles.IsServiceCenterUser() || roles.IsAdministrationUser() || roles.IsOperationManagerUser())
                {
                    perManager = root;
                }

                return root;
            }

            return new UserExtension();
        }

        /// <summary>
        /// New corperator user 
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual UserExtension CreateNewCorpUserEntity(UserExtension root)
        {
            return new UserExtension() { 
                Mail = root.Mail,
                ClientID = root.ClientID,
                PNo = root.PNo,
                Department = root.Department,
                Costcenter = root.Costcenter,
                VName = root.VName,
                Name = root.Name,
                Description = root.Description,
                Status = root.Status,
                Company = root.Company,
                TypeOfJourney = root.TypeOfJourney,
                Gender = root.Gender
            };
        }
    }
}
