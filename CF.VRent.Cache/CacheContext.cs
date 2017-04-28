using CF.VRent.Common;
using CF.VRent.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CF.VRent.Cache
{
    public sealed class CacheContext
    {
        public static readonly CacheContext Context;

        public CacheModel LongModel { get; private set; }

        public CacheModel ShortModel { get; private set; }

        public CacheModel MidModel { get; private set; }

        public CacheModel SessionModel { get; private set; }

        public CachePolicyCollection Policy { get; private set; }

        static CacheContext()
        {
            if (Context == null)
            {
                Context = new CacheContext();
            }
        }

        private CacheContext()
        {
            var config = ConfigurationManager
                .GetSection("VRentCache") as CacheConfiguration;

            Policy = config.Policy;

            LongModel = CacheFactory.CreateModel(Policy[CachePolicyType.Long]);

            ShortModel = CacheFactory.CreateModel(Policy[CachePolicyType.Short]);

            MidModel = CacheFactory.CreateModel(Policy[CachePolicyType.Middle]);

            SessionModel = CacheFactory.CreateModel(Policy[CachePolicyType.Session]);
        }

        /// <summary>
        /// Get model accroding to policy type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CacheModel Model(CachePolicyType type)
        {
            return CacheFactory.CreateModel(Policy[type]);
        }

        internal static void LogWhenUpdate(CacheEntryUpdateArguments arg)
        {
            LogInfor.WriteDebug("Cache obj is updated", 
                String.Format("CacheKey:{0}, CacheContentSource:{1}, Reason:{2}", arg.Key,
                              arg.Source.GetCacheItem(arg.Key).Value.ObjectToJson(), arg.RemovedReason.ToString()), "System");
        }

        internal static void LogWhenNew(string key, object newObj)
        {
            LogInfor.WriteDebug("Cache obj is added",
                String.Format("CacheKey:{0}, CacheContent:{1}", key, newObj.ObjectToJson()), "System");
        }

        internal static void LogWheHit(string key)
        {
            LogInfor.WriteDebug("Cache obj is hit",
                String.Format("CacheKey:{0}", key), "System");
        }

        internal static void LogRemoveAll(string cacheName, long count)
        {
            LogInfor.WriteDebug("Cache objs are removed",
                           String.Format("Model:{0},Count:{1}", cacheName, count), "System");
        }

        internal static void LogRemoveOne(string key,object entry)
        {
            LogInfor.WriteDebug("Cache obj is removed",
                                      String.Format("CacheKey:{0}, CacheContent:{1}", key, entry.ObjectToJson()), "System");
        }


        /// <summary>
        /// Remove cached objs with uniqueId
        /// </summary>
        /// <param name="uniqueId"></param>
        public void Remove(string uniqueId)
        {
            foreach (string key in this.ShortModel.GetKeyStartWith(uniqueId))
            {
                this.ShortModel.Remove(key);
            }

            foreach (string key in this.LongModel.GetKeyStartWith(uniqueId))
            {
                this.LongModel.Remove(key);
            }

            foreach (string key in this.MidModel.GetKeyStartWith(uniqueId))
            {
                this.MidModel.Remove(key);
            }

            foreach (string key in this.SessionModel.GetKeyStartWith(uniqueId))
            {
                this.SessionModel.Remove(key);
            }
        }

        public void RemoveAll()
        {
            this.ShortModel.RemoveAll();
            this.LongModel.RemoveAll();
            this.MidModel.RemoveAll();
            this.SessionModel.RemoveAll();
        }
    }
}
