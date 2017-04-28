
using CF.VRent.UserStatus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.UserStatus
{
    /// <summary>
    /// A Collection for UserStatusEntity
    /// </summary>
    public class UserStatusEntityCollection : IEnumerable<UserStatusEntity>, ICollection<UserStatusEntity>
    {
        private UserStatusEntity[] _status;
        private string _statusBinaryPattern;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        public UserStatusEntityCollection(IEnumerable<UserStatusEntity> status)
            : this(status, null)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="extension"></param>
        public UserStatusEntityCollection(IEnumerable<UserStatusEntity> status, IEnumerable<UserStatusExtensionEntity> extension)
        {
            if (status != null)
            {
                _status = status.ToArray();
                foreach (UserStatusEntity us in _status)
                {
                    us.PropertyChanged += us_PropertyChanged;
                    us.PropertyBeforeChange += us_PropertyBeforeChange;
                }
            }
            if (extension != null)
            {
                Extensions = new UserStatusExtensionEntityCollection(extension);
            }

            _statusBinaryPattern = _getBinaryPattern();
        }

        /// <summary>
        /// Before the property changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void us_PropertyBeforeChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Flag")
            {
                var ext = e as PropertyChangedExtEventArgs;

                //if newValue == oldValue, ignore it
                if (ext.NewValue.ToString() != ext.OldValue.ToString())
                {
                    if (_status.FirstOrDefault(r => r.Flag.Trim() == ext.NewValue.ToString().Trim()) != null)
                    {
                        throw new Exception("Exist multiple flags value");
                    }
                }
            }
        }

        /// <summary>
        /// After change the property value, reset the binary pattern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void us_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Name")
            {
                var ext = e as PropertyChangedExtEventArgs;

                if (ext.NewValue.ToString() != ext.OldValue.ToString())
                {
                    _statusBinaryPattern = _getBinaryPattern();
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public UserStatusEntityCollection():this(null)
        {
        }

        private string _binaryPattern;
        public string BinaryPattern
        {
            get
            {
                StringBuilder builder = new StringBuilder(_statusBinaryPattern);
                if (Extensions != null && Extensions.Count > 0)
                {
                    builder.Append(".");
                    builder.Append(Extensions.BinaryPattern);
                }
                _binaryPattern = builder.ToString();
                return _binaryPattern;
            }
        }

        /// <summary>
        /// Extensions
        /// </summary>
        public UserStatusExtensionEntityCollection Extensions { get; private set; }

        /// <summary>
        /// Get UserStatusEntity by flag
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public UserStatusEntity this[string flag]
        {
            get
            {
                _validate();
                var entity = _status.ToList().Find(r => r.Flag.Trim() == flag.Trim());
                return entity;
            }
        }

        public UserStatusEntity this[int index]
        {
            get
            {
                _validate();
                return _status[index];
            }
        }

        /// <summary>
        /// Get binary pattern i.e "00000000"
        /// </summary>
        /// <returns></returns>
        private string _getBinaryPattern()
        {
            StringBuilder pattern = new StringBuilder();

            if (_status != null)
            {
                _status = _status.OrderBy(r => r.Flag).ToArray();

                foreach (UserStatusEntity us in _status)
                {
                    //pattern.Append(us.Value);
                    if (us.Value == 0)
                    {
                        pattern.Append(us.Value);
                    }
                    else
                    {
                        pattern.Append(us.Flag);
                    }
                }
            }

            return pattern.ToString();
        }

        public IEnumerator<UserStatusEntity> GetEnumerator()
        {
            _validate();
            return _status.ToList().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            _validate();
            return _status.GetEnumerator();
        }


        public void Add(UserStatusEntity item)
        {
            throw new Exception("Collection is readonly");
        }

        public void Clear()
        {
            throw new Exception("Collection is readonly");
        }

        public bool Contains(UserStatusEntity item)
        {
            _validate();
            return _status.ToList().Contains(item);
        }

        public void CopyTo(UserStatusEntity[] array, int arrayIndex)
        {
            _validate();
            _status.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                _validate();
                return _status.Length;
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(UserStatusEntity item)
        {
            throw new Exception("Collection is readonly");
        }

        private void _validate()
        {
            if (_status == null)
            {
                throw new NullReferenceException("Collection is null");
            }
        }

        /// <summary>
        /// Get current avaliable status string
        /// </summary>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public string GetAvailableStatus(string seperator = ",")
        {
            var astatus = _status.Where(r => r.Value > 0);
            StringBuilder sb = new StringBuilder();

            foreach (UserStatusEntity us in astatus)
            {
                sb.Append(us.Name);
                if (astatus.ToList().IndexOf(us) < astatus.Count() - 1)
                {
                    sb.Append(seperator);
                }
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// compare two status entities
    /// </summary>
    public class UserStatusComparer : IEqualityComparer<UserStatusEntity>
    {
        public bool Equals(UserStatusEntity x, UserStatusEntity y)
        {
            return x.Flag.Trim() == y.Flag.Trim();
        }

        public int GetHashCode(UserStatusEntity obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
