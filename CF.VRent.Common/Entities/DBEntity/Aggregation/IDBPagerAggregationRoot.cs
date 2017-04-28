using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Aggregation
{
    public interface IDBPagerAggregationRoot : IDBConditionAggregationRoot
    {
        int PageNumber { get; set; }
        int ItemsPerPage { get; set; }
        long TotalPage { get; set; }
        long TotalCount { get; set; }
    }
}
