using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Aggregation
{
    public interface IDBEntityAggregationRoot : IDBGlobal
    {
        IEnumerable<string> GetKeyNames();
    }
}
