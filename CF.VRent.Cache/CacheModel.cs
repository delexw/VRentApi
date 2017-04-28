using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Cache
{
    public abstract class CacheModel : ICacheModel
    {
        public abstract void Set(string key, object cacheObj);

        public abstract T Get<T>(string key) where T : class,new();

        public abstract bool Exist(string key);

        public abstract IEnumerable<string> GetKeyStartWith(string pre);

        public abstract IEnumerable<T> GetObjectKeyStartWith<T>(string pre) where T : class,new();

        public CachePolicy Attributes { get;  private set;}

        public CacheModel(CachePolicy policy)
        {
            Attributes = policy;
        }

        public abstract void Remove(string key);

        public abstract void RemoveAll();
    }
}
