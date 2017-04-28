using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.DBEntity.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity
{
    public interface IDBRespository<TEntity, VOperator> : IDBRespositoryOperator<TEntity>
        where TEntity : IDBEntityAggregationRoot
        where VOperator : IDBProxyOperator<TEntity>, new()
    {
        VOperator Operator { get; set; }
    }
}
