using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.DBEntity.Respository;
using CF.VRent.DAL;
using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.DataAccessProxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TermsConditionService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TermsConditionService.svc or TermsConditionService.svc.cs at the Solution Explorer and start debugging.
    
    public class TermsConditionService : ITermsConditionService
    {
        private SQLServerRepository<TermsCondition,TermsConditionDAL> resp;
        private SQLServerRepository<UserTermsConditionAgreement, UserTermsConditionDAL> respUser;

        public TermsConditionService()
        {
            //a instance associate entity with DAL object
            resp = new SQLServerRepository<TermsCondition, TermsConditionDAL>();
            respUser = new SQLServerRepository<UserTermsConditionAgreement, UserTermsConditionDAL>();
        }

        public DBEntityAggregation<TermsCondition, DBConditionObject> GetLatestTC(DBConditionObject condition)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<DBEntityAggregation<TermsCondition, DBConditionObject>>(
                () =>
                {
                    return resp.Get(condition);
                });
        }

        public long AddOrUpdateTC(TermsCondition entity)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<long>(
                () =>
                {
                    return resp.Add(entity);
                });
        }

        public long AcceptTC(UserTermsConditionAgreement entity)
        {
            return DataAccessProxyConstantRepo.DataAccessExceptionGuard<long>(
                () =>
                {
                    return respUser.Add(entity);
                });
        }
    }
}
