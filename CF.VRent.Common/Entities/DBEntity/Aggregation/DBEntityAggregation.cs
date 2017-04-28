using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Aggregation
{
    /// <summary>
    /// Entity Common Aggregation Object include Entity Collection and Pager/Condition Object
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="VCondition"></typeparam>
    [Serializable]
    [DataContract]
    public class DBEntityAggregation<TEntity, VCondition> : IAggregation<TEntity, VCondition>
        where TEntity : IDBEntityAggregationRoot
        where VCondition : IDBConditionAggregationRoot
    {
        [DataMember]
        public IEnumerable<TEntity> Entities
        {
            get;
            private set;
        }

        [DataMember]
        public VCondition Condition
        {
            get;
            private set;
        }


        public void SetCondition(VCondition condition)
        {
            Condition = condition;
        }

        public void SetEntities(IEnumerable<TEntity> entitis)
        {
            Entities = entitis;
        }
    }
}
