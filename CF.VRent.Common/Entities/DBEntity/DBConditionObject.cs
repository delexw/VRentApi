using CF.VRent.Common.Entities.DBEntity.Aggregation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity
{
    [Serializable]
    [DataContract]
    public class DBConditionObject : IDBConditionAggregationRoot
    {
        [DataMember]
        public Dictionary<string, string> RawWhereConditions
        {
            get;
            set;
        }

        public virtual void Dispose()
        {

        }

        [DataMember]
        public Dictionary<string, string> RawOrderByConditions
        {
            get;
            set;
        }

        public DBConditionObject()
        {
            RawWhereConditions = new Dictionary<string,string>();
            RawOrderByConditions = new Dictionary<string, string>();
        }
    }
}
