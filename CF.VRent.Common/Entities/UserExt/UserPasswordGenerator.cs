using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace CF.VRent.Common.Entities.UserExt
{
    public class UserPasswordGenerator : ConfigurationSection
    {
        [ConfigurationProperty("enabled", DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return base["enabled"].ToBool();
            }
            set
            {
                base["enabled"] = value;
            }
        }


        [ConfigurationProperty("length", DefaultValue = 6)]
        public int Length
        {
            get
            {
                return base["length"].ToInt();
            }
            set
            {
                base["length"] = value;
            }
        }
    }
}
