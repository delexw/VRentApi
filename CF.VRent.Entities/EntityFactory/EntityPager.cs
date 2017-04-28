using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Entities.EntityFactory
{
    [Serializable]
    [DataContract]
    public class EntityPager<TEntity> where TEntity : RestfulCommonObject
    {
        [DataMember]
        public int PageNumber { get; set; }
        [DataMember]
        public int ItemsPerPage { get; set; }
        [DataMember]
        public long TotalPage { get; set; }
        [DataMember]
        public long TotalCount { get; set; }
        [DataMember]
        public Dictionary<string, string> RawOrderByConditions { get; set; }
        [DataMember]
        public Dictionary<string, string> RawWhereConditions { get; set; }
        [DataMember]
        public IEnumerable<TEntity> Enitites { get; set; }
    }

    public class Pager
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public long TotalPage { get; set; }
        public long TotalCount { get; set; }
        public Dictionary<string, string> RawOrderByConditions { get; set; }
        public Dictionary<string, string> RawWhereConditions { get; set; }
    }
}
