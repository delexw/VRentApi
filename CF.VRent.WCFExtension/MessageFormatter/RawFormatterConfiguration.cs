using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace CF.VRent.WCFExtension.MessageFormatter
{
    public class RawFormatterConfiguration : ConfigurationElement, IRawFormaterConfiguration
    {
        [ConfigurationProperty("enabled")]
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

        [ConfigurationProperty("jsonFormatter")]
        public string JsonFormatter
        {
            get
            {
                return base["jsonFormatter"].ToStr().Trim();
            }
            set
            {
                base["jsonFormatter"] = value;
            }
        }

        [ConfigurationProperty("responseFormatter", IsRequired = false)]
        public string OutGoingFormatter
        {
            get
            {
                return base["responseFormatter"].ToStr().Trim();
            }
            set
            {
                base["responseFormatter"] = value;
            }
        }
    }
}
