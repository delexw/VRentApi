using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common.Entities.UserExt;
using CF.VRent.Entities.KemasWrapper;
using CF.VRent.Entities.KEMASWSIF_USERRef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Unity;
using CF.VRent.Common;
using CF.VRent.Entities.DataAccessProxy;
using CF.VRent.Common.UserContracts;
using CF.VRent.UserRole;
using CF.VRent.UserRole.Interfaces;
using CF.VRent.UserCompany;
using CF.VRent.UserStatus;
using CF.VRent.UserStatus.Interfaces;
using CF.VRent.UserCompany.Interfaces;
using System.Web;

namespace CF.VRent.Entities.EntityFactory
{
    public class UserFactory
    {
        /// <summary>
        /// Unbox UserExtension to UserExtensionHeader
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual UserExtensionHeader CreateHeaderEntity(UserExtension root)
        {
            var extType = typeof(UserExtensionHeader);

            var newExt = new UserExtensionHeader();
            if (root != null)
            {
                var udType = root.GetType();
                //Set property value
                foreach (PropertyInfo pi in extType.GetProperties())
                {
                    var rootProperty = udType.GetProperty(pi.Name);
                    if (rootProperty != null)
                    {
                        pi.SetValue(newExt, rootProperty.GetValue(root, null), null);
                    }
                }

                newExt.LoginName = root.Mail;
            }

            return newExt;
        }
        /// <summary>
        /// From user transfer to user extension header
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual UserExtensionHeader CreateHeaderEntity(UserTransferRequest root)
        {
            return new UserExtensionHeader() { 
                Name = root.FirstName,
                VName = root.LastName,
                LoginName = root.Mail,
                ID = root.UserID.ToString()
            };
        }

        /// <summary>
        /// From user transfer list to user extension header list
        /// </summary>
        /// <param name="roots"></param>
        /// <returns></returns>
        public virtual IEnumerable<UserExtensionHeader> CreateHeaderEntity(IEnumerable<UserTransferRequest> roots)
        {
            if (roots != null)
            {
                List<UserExtensionHeader> extensions = new List<UserExtensionHeader>();

                foreach (UserTransferRequest ud in roots)
                {
                    extensions.Add(this.CreateHeaderEntity(ud));
                }

                return extensions;
            }

            return null;
        }

        /// <summary>
        /// From userData2 to UserExtensionHeader
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public virtual UserExtensionHeader CreateHeaderEntity(UserData2 root)
        {
            var extType = typeof(UserExtensionHeader);

            var newExt = new UserExtensionHeader();
            if (root != null)
            {
                var udType = root.GetType();
                //Set property value
                foreach (PropertyInfo pi in extType.GetProperties())
                {
                    var rootProperty = udType.GetProperty(pi.Name);
                    if (rootProperty != null)
                    {
                        var pV = rootProperty.GetValue(root, null);
                        if (rootProperty.PropertyType == typeof(string))
                        {
                            pV = HttpUtility.HtmlDecode(pV.ToString());
                        }
                        pi.SetValue(newExt, pV, null);
                    }
                }

                //Username
                newExt.LoginName = root.Mail;

                newExt = _setUserStatus(root.Status, newExt, root);
                newExt.RoleEntities = _convertToUserRole(root.Roles);
                newExt.ComanyEntites = _convertToUserCompany(root.Clients);
            }

            return newExt;
        }

        /// <summary>
        /// From List userdata2 to List UserExtensionHeader
        /// </summary>
        /// <param name="roots"></param>
        /// <returns></returns>
        public IEnumerable<UserExtensionHeader> CreateHeaderEntity(IEnumerable<UserData2> roots)
        {
            if (roots != null)
            {
                List<UserExtensionHeader> extensions = new List<UserExtensionHeader>();

                foreach (UserData2 ud in roots)
                {
                    extensions.Add(this.CreateHeaderEntity(ud));
                }

                return extensions;
            }

            return null;
        }

        /// <summary>
        /// From userData2 to UserExtension
        /// </summary>
        /// <param name="root"></param>
        /// <param name="otherValues"></param>
        /// <returns></returns>
        public virtual UserExtension CreateEntity(UserData2 root)
        {
            var extType = typeof(UserExtension);

            var newExt = new UserExtension();
            if (root != null)
            {
                var udType = root.GetType();
                //Set property value
                foreach (PropertyInfo pi in extType.GetProperties())
                {
                    var rootProperty = udType.GetProperty(pi.Name);
                    if (rootProperty != null)
                    {
                        var pV = rootProperty.GetValue(root, null);
                        if (rootProperty.PropertyType == typeof(string))
                        {
                            pV = HttpUtility.HtmlDecode(pV.ToString());
                    }
                        pi.SetValue(newExt, pV, null);
                }
                }

                if (root.Clients != null && root.Clients.Length > 0)
                {
                    //Set ClientID
                    newExt.ClientID = root.Clients[0].ID;
                }

                newExt.License = root.License;
                newExt.ProxyLicense = new UserLicenseFactory().CreateEntity(root.License);
                newExt.Clients = root.Clients;

                newExt = _setUserStatus(root.Status, newExt, root);
                newExt.RoleEntities = _convertToUserRole(root.Roles);
                newExt.ComanyEntites = _convertToUserCompany(root.Clients);
            }

            return newExt;
        }

        /// <summary>
        /// From List userdata2 to List userextension
        /// </summary>
        /// <param name="roots"></param>
        /// <param name="otherValues"></param>
        /// <returns></returns>
        public IEnumerable<UserExtension> CreateEntity(IEnumerable<UserData2> roots)
        {
            if (roots != null)
            {
                List<UserExtension> extensions = new List<UserExtension>();

                foreach (UserData2 ud in roots)
                {
                    extensions.Add(this.CreateEntity(ud));
                }


                return extensions;
            }

            return null;
        }

        /// <summary>
        /// From userextension to updateUserData
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public virtual updateUserData CreateEntity(UserExtension ext)
        {
            var extType = typeof(updateUserData);

            var newUpdateUserData = new updateUserData();

            if (ext != null)
            {
                var udType = ext.GetType();
                //Set property value
                foreach (PropertyInfo pi in extType.GetProperties())
                {
                    var rootProperty = udType.GetProperty(pi.Name);
                    if (rootProperty != null)
                    {
                        pi.SetValue(newUpdateUserData, rootProperty.GetValue(ext, null), null);
                    }
                }
            }

            //Must set these values to allow to save fields
            newUpdateUserData.TypeOfJourneySpecified = true;
            newUpdateUserData.GenderSpecified = true;
            newUpdateUserData.AllowChangePwdSpecified = true;
            newUpdateUserData.EnabledSpecified = true;
            newUpdateUserData.BlockedSpecified = true;

            return newUpdateUserData;
        }



        /// <summary>
        /// Convert from ProxyUserSetting to UserExtension
        /// </summary>
        /// <param name="orginalUser"></param>
        /// <returns></returns>
        public virtual UserExtension CreateEntity(ProxyUserSetting orginalUser)
        {
            if (orginalUser != null)
            {
                var newUser = new UserExtension() { 
                    SessionID = orginalUser.SessionID,
                    ID = orginalUser.ID,
                    AllowChangePwd = orginalUser.AllowChangePwd,
                    Blocked = orginalUser.Blocked,
                    Enabled = orginalUser.Enabled,
                    Name = orginalUser.Name,
                    VName = orginalUser.VName,
                    Status = orginalUser.Status,
                    Mail = orginalUser.Mail,
                    Password = orginalUser.Pwd,
                    ClientID = orginalUser.ClientID,//added by daniel
                    Company = orginalUser.Company

                };

                newUser = _setUserStatus(newUser.Status, newUser, orginalUser);




                if (orginalUser.AllRoles != null)
                {
                    newUser.RoleEntities = _convertToUserRole(orginalUser.AllRoles.ToArray());
                }
                else
                {
                    newUser.RoleEntities = new UserRoleEntityCollection();
                }

                return newUser;
            }
            return new UserExtension();
        }

        #region Private
        private UserRoleEntityCollection _convertToUserRole(ProxyRole[] kemasRoles)
        {
            //Convert kemasRole to UserRoleCollection
            IUserRoleManager roles = UserRoleContext.CreateRoleManager();
            if (kemasRoles != null)
            {
                List<UserRoleEntity> u = new List<UserRoleEntity>();
                List<string> kemasRoleNames = new List<string>();
                foreach (ProxyRole role in kemasRoles)
                {
                    kemasRoleNames.Add(HttpUtility.HtmlDecode(role.RoleMember));
                }

                //Get kemasRoles by UserRole name
                var groupRoles = roles.Roles[kemasRoleNames.ToArray()];
                //Convert kemasRoles to UserRoleEntityCollection
                foreach (UserRoleEntity e in groupRoles)
                {
                    if (u.Where(r => r.Key.Trim() == e.Key).Count() == 0 && e.Enable)
                    {
                        u.Add(e);
                    }
                }

                return new UserRoleEntityCollection(u);
            }
            else
            {
                return new UserRoleEntityCollection();
            }
        }

        private UserRoleEntityCollection _convertToUserRole(Role[] kemasRoles)
        {
            //Convert kemasRole to UserRoleCollection
            IUserRoleManager roles = UserRoleContext.CreateRoleManager();
            if (kemasRoles != null)
            {
                List<UserRoleEntity> u = new List<UserRoleEntity>();
                List<string> kemasRoleNames = new List<string>();
                foreach (Role role in kemasRoles)
                {
                    kemasRoleNames.Add(HttpUtility.HtmlDecode(role.Name));
                }

                //Get kemasRoles by UserRole name
                var groupRoles = roles.Roles[kemasRoleNames.ToArray()];
                //Convert kemasRoles to UserRoleEntityCollection
                foreach (UserRoleEntity e in groupRoles)
                {
                    if (u.Where(r => r.Key.Trim() == e.Key).Count() == 0 && e.Enable)
                    {
                        u.Add(e);
                    }
                }

                return new UserRoleEntityCollection(u);
            }
            else
            {
                return new UserRoleEntityCollection();
            }
        }

        private UserComanyEntityCollection _convertToUserCompany(Client[] kemasClients)
        {
            //End user or corperate user
            IUserCompanyManager company = UserCompanyContext.CreateCompanyManager();
            if (kemasClients != null)
            {
                List<UserCompanyEntity> c = new List<UserCompanyEntity>();
                foreach (Client client in kemasClients)
                {
                    var ce = company.Companies.GetEndUserCompany(HttpUtility.HtmlDecode(client.Name));
                    if (ce != null)
                    {
                        c.Add(ce);
                    }
                }

                return new UserComanyEntityCollection(c);
            }
            else
            {
                return new UserComanyEntityCollection();
            }
        }

        /// <summary>
        /// Reset status by UserData2
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="userStatus"></param>
        /// <param name="newUser"></param>
        /// <param name="kemasUser"></param>
        /// <returns></returns>
        private TUser _setUserStatus<TUser>(string userStatus, TUser newUser, UserData2 kemasUser) where TUser : UserExtensionHeader
        {
            //Format status
            //Get the non-zero status name
            if (!String.IsNullOrWhiteSpace(userStatus))
            {
                IUserStatusManager status = UserStatusContext.CreateStatusManager(userStatus);

                var statusSets = new UserStatusFactory(kemasUser).CreateEntity(status.Status);

                _setUserStatus<TUser>(statusSets, newUser);
            }

            return newUser;
        }

        /// <summary>
        /// Reset status by ProxyUserSetting
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="userStatus"></param>
        /// <param name="newUser"></param>
        /// <param name="kemasUser"></param>
        /// <returns></returns>
        private TUser _setUserStatus<TUser>(string userStatus, TUser newUser, ProxyUserSetting kemasUser) where TUser : UserExtensionHeader
        {
            //Format status
            //Get the non-zero status name
            if (!String.IsNullOrWhiteSpace(userStatus))
            {
                IUserStatusManager status = UserStatusContext.CreateStatusManager(userStatus);

                var statusSets = new UserStatusFactory(kemasUser).CreateEntity(status.Status);

                _setUserStatus<TUser>(statusSets, newUser);
            }

            return newUser;
        }

        /// <summary>
        /// Reset status fields
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="statusCollection"></param>
        /// <param name="newUser"></param>
        private void _setUserStatus<TUser>(UserStatusEntityCollection statusCollection, TUser newUser) where TUser : UserExtensionHeader
        {
            newUser.StatusName = statusCollection.GetAvailableStatus().Split(',');
            newUser.StatusEntities = statusCollection;
            newUser.StatusExtensionEntities = statusCollection.Extensions;
            newUser.Status = statusCollection.BinaryPattern;
        }

        /// <summary>
        /// public method for user status fields setting
        /// </summary>
        /// <param name="newUser"></param>
        /// <param name="status"></param>
        public void SetUserStatus(UserExtension newUser,UserStatusEntityCollection status)
        {
            //Reflect the status collection according to kemas value
            var statusSets = new UserStatusFactory(newUser).CreateEntity(status);
            _setUserStatus<UserExtension>(statusSets, newUser);
        }
        #endregion
    }
}
