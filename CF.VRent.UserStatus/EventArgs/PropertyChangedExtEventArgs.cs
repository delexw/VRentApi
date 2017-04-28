using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CF.VRent.UserStatus.EventArgs
{
    /// <summary>
    /// Extension from PropertyChangedEventArgs
    /// </summary>
    public class PropertyChangedExtEventArgs : PropertyChangedEventArgs
    {
        public object NewValue { get; private set; }
        public object OldValue { get; private set; }

        public PropertyChangedExtEventArgs(string propertyName)
            : this(propertyName, null, null)
        { }

        public PropertyChangedExtEventArgs(string propertyName, object newVal)
            : this(propertyName, newVal, null)
        {

        }

        public PropertyChangedExtEventArgs(string propertyName, object newVal, object oldVal)
            : base(propertyName)
        {
            this.NewValue = newVal;
            this.OldValue = oldVal;
        }
    }
}
