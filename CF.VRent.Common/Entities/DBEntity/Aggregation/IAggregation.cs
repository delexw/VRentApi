using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Aggregation
{
    /// <summary>
    /// Aggreation Object Interface
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="VCondition"></typeparam>
    public interface IAggregation<in TEntity, in VCondition>
        where TEntity : IDBEntityAggregationRoot
        where VCondition : IDBConditionAggregationRoot
    {
        void SetEntities(IEnumerable<TEntity> entitis);
        void SetCondition(VCondition condition);
    }
}
