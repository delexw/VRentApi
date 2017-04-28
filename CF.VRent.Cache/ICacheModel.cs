using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Cache
{
   public  interface ICacheModel
    {
       void Set(string key, object cacheObj);
       T Get<T>(string key) where T : class,new();
       bool Exist(string key);
       void Remove(string key);
       void RemoveAll();
    }
}
