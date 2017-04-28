using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using CF.VRent.Log;

namespace CF.VRent.Cache.ConcreteModel
{
    public class RuntimeCache : CacheModel
    {
        private MemoryCache _cache;

        public RuntimeCache(CachePolicy policy)
            : base(policy)
        {
            if (_cache == null)
            {
                _cache = new MemoryCache(policy.CacheName);
            }
        }

        /// <summary>
        /// Set obj to memory
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cacheObj"></param>
        public override void Set(string key, object cacheObj)
        {
            //MemoryCache cache = MemoryCache.Default;

            var cacheWrapper = Attributes.NewCache(key).Init(cacheObj);

            var itemPolicy = new CacheItemPolicy()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(this.Attributes.Time),
                        UpdateCallback = new CacheEntryUpdateCallback((CacheEntryUpdateArguments arg) =>
                        {
                            if (this.Attributes.Log)
                            {
                                cacheWrapper.Remove();
                                CacheContext.LogWhenUpdate(arg);
                            }
                        })
                    };

            if (cacheObj != null)
            {
                if (!_cache.Contains(key))
                {
                    if (this.Attributes.Log)
                    {
                        CacheContext.LogWhenNew(key, cacheObj);
                    }
                    _cache.Set(key, cacheWrapper, itemPolicy);
                }
                else
                {
                    _cache.Set(key, cacheWrapper, itemPolicy);
                }
            }
        }

        /// <summary>
        /// Get value from cache. if empty, return null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public override T Get<T>(string key)
        {
            if (this.Attributes.Log)
            {
                CacheContext.LogWheHit(key);
            }
            var obj = _cache.Get(key);
            if (obj != null)
            {
                var co = obj as CacheObject;
                return co.Origin as T;
            }
            return null;
        }

        public override bool Exist(string key)
        {
            return _cache.Contains(key);
        }

        /// <summary>
        /// Get the keys with prefix
        /// </summary>
        /// <param name="pre"></param>
        /// <returns></returns>
        public override IEnumerable<string> GetKeyStartWith(string pre)
        {
            foreach (var pair in _cache)
            {
                if (pair.Key.StartsWith(pre))
                {
                    yield return pair.Key;
                }
            }

            yield break;
        }

        /// <summary>
        /// Get cached obj with prefix of key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pre"></param>
        /// <returns></returns>
        public override IEnumerable<T> GetObjectKeyStartWith<T>(string pre)
        {
            foreach (var pair in _cache)
            {
                if (pair.Key.StartsWith(pre))
                {
                    yield return (T)((CacheObject)pair.Value).Origin;
                }
            }

            yield break;
        }


        public override void Remove(string key)
        {
            var entry = _cache.Remove(key);
            if (Attributes.Log)
            {
                CacheContext.LogRemoveOne(key, entry);
            }
        }

        public override void RemoveAll()
        {
            var count = _cache.Trim(100);
            if (Attributes.Log)
            {
                CacheContext.LogRemoveAll(Attributes.CacheName, count);
            }
        }
    }
}
