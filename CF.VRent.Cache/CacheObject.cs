
using CF.VRent.Cache.BackupProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Cache
{
    public class CacheObject
    {
        private string _key;
        private object _cacheObj;
        /// <summary>
        /// The object that is cached
        /// </summary>
        public object Origin
        {
            get
            {
                return _cacheObj;
            }
        }
        private DateTime _cacheDate;
        private CachePolicy _policy;

        public delegate void NotifyOnRemove(string key, CacheRemovableArgs args);

        /// <summary>
        /// The event occurs when object is expried
        /// </summary>
        public event NotifyOnRemove OnRemove;

        internal CacheObject(string key, CachePolicy policy)
        {
            _key = key;
            _policy = policy;
            OnRemove += CacheObject_OnRemove;
        }

        void CacheObject_OnRemove(string key, CacheRemovableArgs args)
        {
            if (args.IsBackup && args.BackStoreProvider != null)
            {
                args.BackStoreProvider.Store(key, this);
            }
        }

        public CacheObject Init(object cacheObj)
        {
            _cacheObj = cacheObj;
            _cacheDate = DateTime.Now;
            return this;
        }

        /// <summary>
        /// Trigger remove with custom backup provider 
        /// </summary>
        /// <param name="provider"></param>
        public void Remove(IBackupProvider provider)
        {
            this.Remove(true, provider);
        }

        /// <summary>
        /// Default
        /// </summary>
        public void Remove()
        {
            this.Remove(CacheFactory.GetBackupProvider<FileBackupProvider>());
        }

        private void Remove(bool isBackup, IBackupProvider provider)
        {
            OnRemove(this._key, new CacheRemovableArgs() { IsBackup = isBackup, BackStoreProvider = provider });
        }
    }

    public class CacheRemovableArgs : EventArgs
    {
        public bool IsBackup { get; set; }
        public IBackupProvider BackStoreProvider { get; set; }
    }
}
