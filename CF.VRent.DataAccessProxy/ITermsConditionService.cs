using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.DBEntity.Operator;
using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITermsConditionService" in both code and config file together.
    [ServiceContract]
    public interface ITermsConditionService
    {

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        DBEntityAggregation<TermsCondition, DBConditionObject> GetLatestTC(DBConditionObject condition);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        long AddOrUpdateTC(TermsCondition entity);

        [OperationContract]
        [FaultContract(typeof(ReturnResult))]
        long AcceptTC(UserTermsConditionAgreement entity);
    }
}
