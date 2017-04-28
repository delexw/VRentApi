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
    public class GeneralLedgerItemDAL : DBProxyOperator<GeneralLedgerItem>
    {
        public override void Dispose()
        {
            
        }

        public override long Add(GeneralLedgerItem entity)
        {
            var outPut = new SqlParameter("@NewID", SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output };
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@HeaderID", entity.HeaderID),                
                   new SqlParameter("@ItemType",entity.ItemType.GetValue()),
                   new SqlParameter("@PostingBody",entity.PostingBody),
                   new SqlParameter("@CreatedBy",entity.CreatedBy),
                   new SqlParameter("@ClientID",entity.ClientID),
                   new SqlParameter("@CompanyCode",entity.CompanyCode),
                   new SqlParameter("@BusinessArea",entity.BusinessArea),
                   outPut
                };

            SQLHelper.ExecuteNonQuery(null, "Sp_GeneralLedgerItem_Add", CommandType.StoredProcedure, paras);

            if (outPut.Value != null)
            {
                return outPut.Value.ToLong();
            }

            return 0;
        }
    }
}
