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
    public class GeneralLedgerItemDetailDAL : DBProxyOperator<GeneralLedgerItemDetail>
    {
        public override void Dispose()
        {
            
        }

        public override long Add(GeneralLedgerItemDetail entity)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@HeaderID", entity.HeaderID),                
                   new SqlParameter("@ItemID",entity.ItemID),
                   new SqlParameter("@PaymentID",entity.PaymentID),
                   new SqlParameter("@DebitNoteID",entity.DebitNoteID),
                   new SqlParameter("@DetailType",entity.DetailType.GetValue()),
                   new SqlParameter("@CreatedBy",entity.CreatedBy)
                };

            SQLHelper.ExecuteNonQuery(null, "Sp_GeneralLedgerItemDetail_Add", CommandType.StoredProcedure, paras);

            return 0;
        }
    }
}
