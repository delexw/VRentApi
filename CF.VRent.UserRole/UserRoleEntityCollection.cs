using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UserRole
{
    /// <summary>
    /// User Role Entity
    /// </summary>
    public class UserRoleEntityCollection:IEnumerable<UserRoleEntity>,ICollection<UserRoleEntity>
    {
        private UserRoleEntity[] _roles;

        public UserRoleEntityCollection():this(new List<UserRoleEntity>())
        { }

        public UserRoleEntityCollection(IEnumerable<UserRoleEntity> roles)
        {
            _roles = roles.ToArray();
        }

        /// <summary>
        /// Current user roles whether include "SC" or not 
        /// </summary>
        /// <returns></returns>
        public bool IsServiceCenterUser()
        {
            if (_roles == null)
            {
                return false;
            }
            return _roles.FirstOrDefault(r => r.Key.Trim() == UserRoleConstants.ServiceCenterKey) != null;
        }

        /// <summary>
        /// Current user roles whether include "VM" or not 
        /// </summary>
        /// <returns></returns>
        public bool IsVRentManagerUser()
        {
            if (_roles == null)
            {
                return false;
            }
            return _roles.FirstOrDefault(r => r.Key.Trim() == UserRoleConstants.VRentManagerKey) != null;
        }

        /// <summary>
        /// Current user roles whether include "SCL" or not 
        /// </summary>
        /// <returns></returns>
        public bool IsOperationManagerUser()
        {
            if (_roles == null)
            {
                return false;
            }
            return _roles.FirstOrDefault(r => r.Key.Trim() == UserRoleConstants.OperationManagerKey) != null;
        }

        /// <summary>
        ///  Current user roles whether include "Admin" or not 
        /// </summary>
        /// <returns></returns>
        public bool IsAdministrationUser()
        {
            if (_roles == null)
            {
                return false;
            }
            return _roles.FirstOrDefault(r => r.Key.Trim() == UserRoleConstants.AdministratorKey) != null;
        }

        /// <summary>
        /// Current user roles whether include "BU" or not 
        /// </summary>
        /// <returns></returns>
        public bool IsBookingUser()
        {
            if (_roles == null)
            {
                return false;
            }
            return _roles.FirstOrDefault(r => r.Key.Trim() == "BU") != null;
        }

        /// <summary>
        /// Get role by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UserRoleEntity this[string key]
        {
            get
            {
                return _roles.FirstOrDefault(r => r.Key.Trim() == key.Trim());
            }
        }

        public UserRoleEntity this[int index]
        {
            get
            {
                return _roles[index];
            }
        }

        /// <summary>
        /// Get role by kemasRole name array
        /// </summary>
        /// <param name="kemasRoleName"></param>
        /// <returns></returns>
        public UserRoleEntity[] this[string[] kemasRoleName]
        {
            get
            {
                var roles = _roles.Where((UserRoleEntity entity) => {

                    int count = 0;
                    foreach (string rn in kemasRoleName)
                    {
                        if (entity.KemasRole.FirstOrDefault(r => r.Name.Trim() == rn.Trim())!=null)
                        {
                            count++;
                        }
                    }

                    //kemasRoleName array contains user role
                    if (count >= entity.KemasRole.Length)
                    {
                        return true;
                    }
                    return false;
                });
                return roles.ToArray();
            }
        }

        /// <summary>
        /// Get current avaliable role string
        /// </summary>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public string GetAvailableRoleKey(string seperator = ",")
        {
            var aroles = _roles.Where(r => r.Enable);
            StringBuilder sb = new StringBuilder();

            foreach (UserRoleEntity us in aroles)
            {
                sb.Append(us.Key);
                if (aroles.ToList().IndexOf(us) < aroles.Count() - 1)
                {
                    sb.Append(seperator);
                }
            }
            return sb.ToString();
        }

        public IEnumerator<UserRoleEntity> GetEnumerator()
        {
            return _roles.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _roles.GetEnumerator();
        }

        public void Add(UserRoleEntity item)
        {
            throw new Exception("Collection is readonly");
        }

        public void Clear()
        {
            throw new Exception("Collection is readonly");
        }

        public bool Contains(UserRoleEntity item)
        {
            return _roles.Contains(item);
        }

        public void CopyTo(UserRoleEntity[] array, int arrayIndex)
        {
           _roles.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _roles.Length; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(UserRoleEntity item)
        {
            throw new Exception("Collection is readonly");
        }
    }
}
