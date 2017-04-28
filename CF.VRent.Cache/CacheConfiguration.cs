using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace CF.VRent.Cache
{
    public class CacheConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public CachePolicyCollection Policy
        {
            get
            {
                return (CachePolicyCollection)base[""];
            }
        }
    }

    public class CachePolicy : ConfigurationElement
    {
        [ConfigurationProperty("type",IsRequired = true,IsKey = true)]
        public CachePolicyType Type
        {
            get
            {
                return base["type"].ToStr().ToEnum<CachePolicyType>();
            }
        }

        /// <summary>
        /// The period time of content in cache. the unit is min
        /// </summary>
        [ConfigurationProperty("time")]
        public double Time
        {
            get
            {
                return base["time"].ToDouble();
            }
        }

        [ConfigurationProperty("log", DefaultValue = true)]
        public bool Log
        {
            get
            {
                return base["log"].ToBool();
            }
        }

        [ConfigurationProperty("cacheName",IsRequired = true)]
        public string CacheName
        {
            get
            {
                return base["cacheName"].ToStr();
            }
        }

        public CacheObject NewCache(string key)
        {
            return new CacheObject(key,this);
        }
    }

    [ConfigurationCollection(typeof(CachePolicy), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap,
        RemoveItemName = "remove",
           AddItemName = "policy")]
    public class CachePolicyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CachePolicy();
        }

        public CachePolicy this[CachePolicyType type]
        {
            get
            {
                return base.BaseGet(type) as CachePolicy;
            }
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CachePolicy)element).Type;
        }
    }

    public enum CachePolicyType
    {
        Long = 1,
        Short = 2,
        Middle = 3,
        Session = 4
    }
}
