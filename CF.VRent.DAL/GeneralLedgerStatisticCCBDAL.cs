using CF.VRent.Common.Entities.DBEntity;
using CF.VRent.Common.Entities.DBEntity.Aggregation;
using CF.VRent.Common.Entities.DBEntity.Operator;
using CF.VRent.DataAccessProxy.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CF.VRent.Common;
using System.Data;

namespace CF.VRent.DAL
{
    public class GeneralLedgerStatisticCCBDAL : DBProxyOperator<GeneralLedgerStatisticCCB>
    {
        public override void Dispose()
        {
            
        }

        public override DBEntityAggregation<GeneralLedgerStatisticCCB, DBConditionObject> Get(IDBConditionAggregationRoot condition)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@DateFrom", condition.RawWhereConditions["DateFrom"].ToDate()),                
                   new SqlParameter("@DateEnd",condition.RawWhereConditions["DateEnd"].ToDate()),
                };

           var statisticTable =  SQLHelper.ExecuteDataTable(null, "Sp_GeneralLedgerStatisticForCCB_Get", CommandType.StoredProcedure, paras);

           List<GeneralLedgerStatisticCCB> statisticObj = new List<GeneralLedgerStatisticCCB>();


           foreach (DataRow row in statisticTable.Rows)
           {
               statisticObj.Add(new GeneralLedgerStatisticCCB() {
                   ClientID = row["ClientID"].ToGuidNull(),
                   CCBTotalCredit = row["CCBTotalCredit"].ToDouble(),
                   CCBTotalDebit = row["CCBTotalDebit"].ToDouble(),
                   DebitNoteID = row["ID"].ToInt()
               });
           }

           var returnEntity = new DBEntityAggregation<GeneralLedgerStatisticCCB, DBConditionObject>();

           returnEntity.SetEntities(statisticObj);

           return returnEntity;
            
        }
    }
}
