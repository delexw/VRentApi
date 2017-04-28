using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace CF.VRent.Email
{
    /// <summary>
    /// Email configuration section
    /// </summary>
    public class EmailConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("templates", IsRequired = true, IsDefaultCollection = true)]
        public EmailTempleateConfigurationCollection Templates
        {
            get
            {
                return (EmailTempleateConfigurationCollection)base["templates"];
            }
            set
            {
                base["templates"] = value;
            }
        }

        [ConfigurationProperty("templateRootPath", IsRequired = true)]
        public string TemplateRootPath
        {
            get
            {
                return base["templateRootPath"].ToString().Trim();
            }
            set
            {
                base["templateRootPath"] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return base["name"].ToString().Trim();
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("addressGroups")]
        public EmailAddressGroupConfigurationCollection AddressGroups
        {
            get
            {
                return base["addressGroups"] as EmailAddressGroupConfigurationCollection;
            }
            set
            {
                base["addressGroups"] = value;
            }
        }

        [ConfigurationProperty("host")]
        public EmailHostConfiguration Host
        {
            get
            {
                return base["host"] as EmailHostConfiguration;
            }
            set
            {
                base["host"] = value;
            }
        }
    }

    /// <summary>
    /// Address group
    /// </summary>
    public class EmailAddressGroupConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public EmailAddressConfigurationCollection Addresses
        {
            get
            {
                return base[""] as EmailAddressConfigurationCollection;
            }
            set
            {
                base[""] = value;
            }
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return base["name"].ToString().Trim();
            }
            set
            {
                base["name"] = value;
            }
        }
    }

    /// <summary>
    /// Address configuration
    /// </summary>
    public class EmailAddressConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = true, IsKey = true)]
        public string Address
        {
            get
            {
                return base["value"].ToString().Trim();
            }
            set
            {
                base["value"] = value;
            }
        }
    }

    /// <summary>
    /// Addresses
    /// </summary>
    [ConfigurationCollection(typeof(EmailAddressConfiguration),
           CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap,
           RemoveItemName = "remove",
           AddItemName = "address")]
    public class EmailAddressConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new EmailAddressConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EmailAddressConfiguration)element).Address;
        }
    }

    /// <summary>
    /// Address groups
    /// </summary>
    [ConfigurationCollection(typeof(EmailAddressGroupConfiguration),
            CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap,
            RemoveItemName = "remove",
            AddItemName = "addresses")]
    public class EmailAddressGroupConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new EmailAddressGroupConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EmailAddressGroupConfiguration)element).Name;
        }
    }

    /// <summary>
    /// Host
    /// </summary>
    public class EmailHostConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("address", IsKey = true, IsRequired = true)]
        public string Address
        {
            get
            {
                return base["address"].ToString().Trim();
            }
            set
            {
                base["address"] = value;
            }
        }

        [ConfigurationProperty("from", IsRequired = true)]
        public string From
        {
            get
            {
                return base["from"].ToString().Trim();
            }
            set
            {
                base["from"] = value;
            }
        }

        [ConfigurationProperty("pwd", IsRequired = false)]
        public string Password
        {
            get
            {
                return base["pwd"].ToString().Trim();
            }
            set
            {
                base["pwd"] = value;
            }
        }

        [ConfigurationProperty("async", IsRequired = false)]
        public bool Async
        {
            get
            {
                return base["async"].ToBool();
            }
            set
            {
                base["async"] = value;
            }
        }
    }

    /// <summary>
    /// Email templates
    /// </summary>
    public class EmailTempleateConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("filePath",IsRequired = true)]
        public string FilePath {
            get
            {
                return base["filePath"].ToStr().Trim();
            }
            set
            {
                base["filePath"] = value;
            }
        }

        [ConfigurationProperty("typeName", IsRequired = true, IsKey = true)]
        public string TypeName
        {
            get
            {
                return base["typeName"].ToStr().Trim();
            }
            set
            {
                base["typeName"] = value;
            }
        }

        [ConfigurationProperty("subject")]
        public string MailTitle
        {
            get
            {
                return base["subject"].ToStr().Trim();
            }
            set
            {
                base["subject"] = value;
            }
        }
    }

    /// <summary>
    /// Email templates collection
    /// </summary>
    [ConfigurationCollection(typeof(EmailTempleateConfiguration),
            CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap,
            RemoveItemName = "remove",
            AddItemName = "template")]
    public class EmailTempleateConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return (ConfigurationElement)new EmailTempleateConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EmailTempleateConfiguration)element).TypeName;
        }
    }
}
