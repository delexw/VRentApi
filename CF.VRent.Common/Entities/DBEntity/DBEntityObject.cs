using CF.VRent.Common.Entities.DBEntity.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity
{
    /// <summary>
    /// Common Entity
    /// </summary>
    [Serializable]
    [DataContract]
    public class DBEntityObject : IDBEntityAggregationRoot
    {
        public virtual IEnumerable<string> GetKeyNames()
        {
            var properies = this.GetType().GetProperties();
            foreach (PropertyInfo pi in properies)
            {
                var cuAttrs = pi.GetCustomAttributes(typeof(Key), true);
                if (cuAttrs.Length > 0)
                {
                    yield return pi.Name;
                }
            }
            yield break;
        }

        public virtual void Dispose()
        {  
        }

        public Type EntityType
        {
            get;
            private set;
        }

        
        public DBEntityObject()
        {
            EntityType = this.GetType();
        }

    }


}
