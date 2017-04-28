using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Aggregation
{
    public interface IDBConditionAggregationRoot : IDBSortAggregationRoot
    {
        Dictionary<string,string> RawWhereConditions { get; set; }
    }
}
