using CF.VRent.Common.Entities.DBEntity.Operator;
using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CF.VRent.Common;

namespace CF.VRent.DAL
{
    public class UserTermsConditionDAL : DBProxyOperator<UserTermsConditionAgreement>
    {
        public override void Dispose()
        {
            
        }

        public override long Add(UserTermsConditionAgreement entity)
        {
            var output = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@UserID", entity.UserID),                
                   new SqlParameter("@TCID",entity.TCID),
                   new SqlParameter("@CreatedBy", entity.CreatedBy),
                   output
                };

            //TODO:no sp name
            int res = SQLHelper.ExecuteNonQuery(null, "Sp_UserTermsConditionAgreement_Create", CommandType.StoredProcedure, paras);

            if (output.Value != null)
            {
                return output.Value.ToInt();
            }

            return base.Add(entity);
        }
    }
}
