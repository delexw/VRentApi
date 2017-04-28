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
    public class GeneralLedgerHeaderDAL : DBProxyOperator<GeneralLedgerHeader>
    {
        public override void Dispose()
        {
            
        }

        public override long Add(GeneralLedgerHeader entity)
        {
            var outPut = new SqlParameter("@NewID", SqlDbType.BigInt) { Direction = System.Data.ParameterDirection.Output };
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@PostingFrom", entity.PostingFrom),                
                   new SqlParameter("@PostingEnd",entity.PostingEnd),
                   new SqlParameter("@CreatedBy",entity.CreatedBy),
                   new SqlParameter("@HeaderType",entity.HeaderType.GetValue()),
                   outPut
                };

            SQLHelper.ExecuteNonQuery(null, "Sp_GeneralLedgerHeader_Add", CommandType.StoredProcedure, paras);

            if (outPut.Value != null)
            {
                return outPut.Value.ToLong();
            }

            return 0;
        }
    }
}
