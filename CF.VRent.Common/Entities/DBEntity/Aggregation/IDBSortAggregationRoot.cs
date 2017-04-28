using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Aggregation
{
    public interface IDBSortAggregationRoot : IDBGlobal
    {
        Dictionary<string, string> RawOrderByConditions { get; set; }
    }
}
