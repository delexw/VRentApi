using CF.VRent.Common.Entities.DBEntity.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Operator
{
    public abstract class DBProxyOperator<TEntity> : IDBProxyOperator<TEntity> where TEntity : IDBEntityAggregationRoot
    {
        public Type OperatorType
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public virtual DBEntityAggregation<TEntity, DBConditionObject> Get(IDBConditionAggregationRoot condition)
        {
            return null;
        }

        public virtual DBEntityAggregation<TEntity, DBPagerObject> GetByPage(IDBPagerAggregationRoot pager)
        {
            return null;
        }

        public virtual long Add(TEntity entity)
        {
            return 0;
        }

        public virtual long Update(TEntity entity)
        {
            return 0;
        }

        public virtual long Delete(TEntity entity)
        {
            return 0;
        }

        public abstract void Dispose();
    }
}
