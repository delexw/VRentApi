using CF.VRent.Common.Entities.DBEntity.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.Common.Entities.DBEntity.Operator
{
    /// <summary>
    /// Operater for respository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDBRespositoryOperator<TEntity> : IDBOperator<TEntity>
        where TEntity : IDBEntityAggregationRoot
    {
        WReturn Invoke<WReturn>(string functionName, params object[] parameters); 
    }
}
