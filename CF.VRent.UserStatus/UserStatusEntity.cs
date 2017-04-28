
using CF.VRent.UserStatus.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.UserStatus
{
    /// <summary>
    /// User Status Entity
    /// </summary>
    [Serializable]
    [DataContract]
    public class UserStatusEntity : INotifyPropertyChanged
    {
        private string _name;
        /// <summary>
        /// Get name
        /// </summary>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (PropertyBeforeChange != null)
                {
                    this.PropertyBeforeChange(this, new PropertyChangedExtEventArgs("Name", value, _value));
                }
                var old = _value;
                _name = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedExtEventArgs("Name", value, old));
                }
            }
        }

        private int _value;
        /// <summary>
        /// Get or set value
        /// </summary>
        [DataMember]
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (PropertyBeforeChange != null)
                {
                    this.PropertyBeforeChange(this, new PropertyChangedExtEventArgs("Value",value,_value));
                }
                var old = _value;
                _value = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedExtEventArgs("Value", value, old));
                }
            }
        }

        private string _flag;
        /// <summary>
        /// Get or set flag will resort the flag position of one status in binary string
        /// </summary>
        [DataMember]
        public string Flag
        {
            get
            {
                return _flag;
            }
            set
            {
                if (PropertyBeforeChange != null)
                {
                    this.PropertyBeforeChange(this, new PropertyChangedExtEventArgs("Flag", value, _flag));
                }
                var old = _flag;
                _flag = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedExtEventArgs("Flag", value, old));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedEventHandler PropertyBeforeChange;

        public UserStatusEntity(string name)
        {
            _name = name;
        }

        public UserStatusEntity():this(null)
        { }
    }
}
