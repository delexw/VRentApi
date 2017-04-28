using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.DBEntity.Operator;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity
{
    public abstract class DBRespository<TEntity, VOperator> : IDBRespository<TEntity, VOperator>
        where TEntity : IDBEntityAggregationRoot
        where VOperator : IDBProxyOperator<TEntity>, new()
    {
        private string _connectionString;

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public DBRespository(VOperator dbOperator)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[Constants.SqlConnStrKey].ConnectionString;
            Operator = dbOperator;
            Operator.OperatorType = Operator.GetType();
        }

        public DBRespository()
            : this(new VOperator())
        { }

        public void ReSetConnectionString(string cs)
        {
            if (!String.IsNullOrWhiteSpace(cs))
            {
                _connectionString = cs;
            }
        }

        public VOperator Operator
        {
            get;
            set;
        }

        public abstract DBEntityAggregation<TEntity, DBConditionObject> Get(IDBConditionAggregationRoot condition);

        public abstract DBEntityAggregation<TEntity, DBPagerObject> GetByPage(IDBPagerAggregationRoot pager);

        public abstract long Add(TEntity entity);

        public abstract long Update(TEntity entity);

        public abstract long Delete(TEntity entity);

        public abstract void Dispose();

        /// <summary>
        /// Invoke custom
        /// </summary>
        /// <typeparam name="WReturn"></typeparam>
        /// <param name="functionName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public WReturn Invoke<WReturn>(string functionName, params object[] parameters)
        {
            var func = this.Operator.OperatorType.GetMethod(functionName);
            if (func != null)
            {
                var ret = func.Invoke(this.Operator, parameters);
                if (ret != null)
                {
                    return (WReturn)ret;
                }
            }
            else
            {
                throw new Exception("The fucntion " + functionName + " is not exited");
            }

            return default(WReturn);
        }
    }
}
