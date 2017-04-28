using CF.VRent.Cache.BackupProvider;
using CF.VRent.Cache.ConcreteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Cache
{
    public class CacheFactory
    {
        public static CacheModel CreateModel(CachePolicy policy)
        {
            return new RuntimeCache(policy);
        }

        public static IBackupProvider GetBackupProvider<T>() where T: IBackupProvider
        {
            return Activator.CreateInstance<T>();
        }
    }
}
