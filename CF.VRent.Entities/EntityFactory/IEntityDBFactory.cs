using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities.EntityFactory
{
    public interface IEntityDBFactory<out TEntity> where TEntity : RestfulCommonObject
    {
        TEntity CreateEntity(IDBEntityAggregationRoot root, params object[] otherValues);

        IEnumerable<TEntity> CreateEntity(IEnumerable<IDBEntityAggregationRoot> roots, params object[] otherValues);
    }
}
