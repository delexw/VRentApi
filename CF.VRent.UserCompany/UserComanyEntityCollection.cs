using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UserCompany
{
    /// <summary>
    /// User Company Collection
    /// </summary>
    public class UserComanyEntityCollection : IEnumerable<UserCompanyEntity>,ICollection<UserCompanyEntity>
    {
        private UserCompanyEntity[] _companies;

        public UserComanyEntityCollection()
            : this(new List<UserCompanyEntity>())
        {
        }

        public UserComanyEntityCollection(IEnumerable<UserCompanyEntity> companies)
        {
            _companies = companies.ToArray();
        }

        /// <summary>
        /// Current user is whether end user or not
        /// </summary>
        /// <returns></returns>
        public bool IsEndUser()
        {
            if (_companies == null)
            {
                return false;
            }
            return _companies.FirstOrDefault(r => r.Key.Trim() == UserCompanyConstants.EndUserCompanyKey) != null;
        }

        /// <summary>
        /// Get company by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UserCompanyEntity this[string key]
        {
            get
            {
                return _companies.FirstOrDefault(r => r.Key.Trim() == key.Trim());
            }
        }

        public UserCompanyEntity this[int index]
        {
            get
            {
                return _companies[index];
            }
        }

        /// <summary>
        /// Get end user company entity by kemas company id
        /// </summary>
        /// <param name="kemasId"></param>
        /// <returns></returns>
        public UserCompanyEntity GetEndUserCompany(Guid kemasId)
        {
            var uce = _companies.FirstOrDefault(r => r.Key.Trim() == UserCompanyConstants.EndUserCompanyKey && r.KemasCompany.Where(s => s.ID.Trim() == kemasId.ToString()).Count() > 0);
            return uce;
        }
        /// <summary>
        /// Get end user company entity by kemas company name
        /// </summary>
        /// <param name="kemasCompanyName"></param>
        /// <returns></returns>
        public UserCompanyEntity GetEndUserCompany(string kemasCompanyName)
        {
            var uce = _companies.FirstOrDefault(r => r.Key.Trim() == UserCompanyConstants.EndUserCompanyKey && r.KemasCompany.Where(s => s.Name.Trim() == kemasCompanyName.Trim()).Count() > 0);
            return uce;
        }

        public IEnumerator<UserCompanyEntity> GetEnumerator()
        {
            return _companies.ToList().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _companies.GetEnumerator();
        }

        /// <summary>
        /// Get all kemas companies from user company group
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KemasCompanyEntity> GetAllKemasCompanyEntities()
        {
            List<KemasCompanyEntity> cs = new List<KemasCompanyEntity>();

            foreach (UserCompanyEntity uce in _companies)
            {
                foreach (KemasCompanyEntity kce in uce.KemasCompany)
                {
                    if (!cs.Contains(kce))
                    {
                        cs.Add(kce);
                    }
                }
            }

            return cs;
        }

        public void Add(UserCompanyEntity item)
        {
            throw new Exception("Collection is readonly");
        }

        public void Clear()
        {
            throw new Exception("Collection is readonly");
        }

        public bool Contains(UserCompanyEntity item)
        {
            return _companies.Contains(item);
        }

        public void CopyTo(UserCompanyEntity[] array, int arrayIndex)
        {
            _companies.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _companies.Length; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(UserCompanyEntity item)
        {
            throw new Exception("Collection is readonly");
        }
    }
}
