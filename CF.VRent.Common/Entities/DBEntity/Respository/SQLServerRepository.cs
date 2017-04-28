using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.DBEntity.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Respository
{
    /// <summary>
    /// A respository for sql server
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="VOperator"></typeparam>
    public class SQLServerRepository<TEntity, VOperator> : DBRespository<TEntity, VOperator>
        where TEntity : IDBEntityAggregationRoot
        where VOperator : IDBProxyOperator<TEntity>, new()
    {
        public SQLServerRepository(string connectionString)
            : base()
        {
            this.ReSetConnectionString(connectionString);
        }

        public SQLServerRepository(VOperator dbOperator)
            : base(dbOperator)
        { }

        public SQLServerRepository()
            : this("")
        {
        }

        public override void Dispose()
        {
            if (Operator != null)
            {
                Operator.Dispose();
                Operator = default(VOperator);
            }
        }

        public override DBEntityAggregation<TEntity, DBConditionObject> Get(IDBConditionAggregationRoot condition)
        {
            //call operator function
            return Operator.Get(condition);
        }

        public override long Add(TEntity entity)
        {
            //call operator function
            return Operator.Add(entity);
        }

        public override long Update(TEntity entity)
        {
            //call operator function
            return Operator.Update(entity);
        }

        public override long Delete(TEntity entity)
        {
            //call operator function
            return Operator.Delete(entity);
        }

        public override DBEntityAggregation<TEntity, DBPagerObject> GetByPage(IDBPagerAggregationRoot pager)
        {
            //call operator function
            return Operator.GetByPage(pager);
        }
    }
}
