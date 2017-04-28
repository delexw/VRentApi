using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CF.VRent.SAPSDK.Entities
{
    internal class PostConfigurationEntity
    {
        public EntityType Type { get; set; }
        public XmlDocument Doc { get; set; }
        public string FilePath { get; set; }
        public string FileResource { get; set; }
    }
}
