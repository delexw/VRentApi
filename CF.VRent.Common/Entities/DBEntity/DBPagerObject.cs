using CF.VRent.Common.Entities.DBEntity.Aggregation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity
{
    /// <summary>
    /// Pager Object for searching objects by page
    /// </summary>
    [Serializable]
    [DataContract]
    public class DBPagerObject : IDBPagerAggregationRoot
    {
        [DataMember]
        public int PageNumber
        {
            get;
            set;
        }

        [DataMember]
        public int ItemsPerPage
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<string, string> RawOrderByConditions
        {
            get;
            set;
        }

        [DataMember]
        public long TotalPage
        {
            get;
            set;
        }

        [DataMember]
        public long TotalCount
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<string, string> RawWhereConditions
        {
            get;
            set;
        }

        public DBPagerObject()
        {
            ItemsPerPage = 10;
            PageNumber = 1;
            RawWhereConditions = new Dictionary<string, string>();
            RawOrderByConditions = new Dictionary<string, string>();
        }

        public virtual void Dispose()
        { }
    }
}
