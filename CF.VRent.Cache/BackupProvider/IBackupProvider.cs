using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Cache.BackupProvider
{
    public interface IBackupProvider
    {
        /// <summary>
        /// Back up
        /// </summary>
        /// <param name="obj"></param>
        void Store(string key,CacheObject obj);
    }
}
