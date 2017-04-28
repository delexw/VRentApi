using CF.VRent.Common.Entities.DBEntity.Aggregation;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Operator
{
    /// <summary>
    /// Operation for proxy object
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDBProxyOperator<TEntity> : IDBOperator<TEntity>
        where TEntity : IDBEntityAggregationRoot
    {
        Type OperatorType { get; set; }
        string Name { get; set; }
    }
}
