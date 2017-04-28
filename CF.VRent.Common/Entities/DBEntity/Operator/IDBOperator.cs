using CF.VRent.Common.Entities.DBEntity.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Operator
{
    /// <summary>
    /// Operation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDBOperator<TEntity> : IDBGlobal
        where TEntity : IDBEntityAggregationRoot
    {
        DBEntityAggregation<TEntity, DBConditionObject> Get(IDBConditionAggregationRoot condition);

        DBEntityAggregation<TEntity, DBPagerObject> GetByPage(IDBPagerAggregationRoot pager);

        long Add(TEntity entity);

        long Update(TEntity entity);

        long Delete(TEntity entity);
    }
}
