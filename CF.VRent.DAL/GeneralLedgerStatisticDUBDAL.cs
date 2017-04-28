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
using CF.VRent.UPSDK;
using CF.VRent.Common.Entities;

namespace CF.VRent.DAL
{
    public class GeneralLedgerStatisticDUBDAL : DBProxyOperator<GeneralLedgerStatisticDUB>
    {
        public override void Dispose()
        {
            
        }

        public override DBEntityAggregation<GeneralLedgerStatisticDUB, DBConditionObject> Get(IDBConditionAggregationRoot condition)
        {
            SqlParameter[] paras = new SqlParameter[]
                {
                   new SqlParameter("@DateFrom", condition.RawWhereConditions["DateFrom"].ToDate()),                
                   new SqlParameter("@DateEnd",condition.RawWhereConditions["DateEnd"].ToDate()),
                };

            var statisticTable = SQLHelper.ExecuteDataTable(null, "Sp_GeneralLedgerStatisticForDUB_Get", CommandType.StoredProcedure, paras);

            List<GeneralLedgerStatisticDUB> statisticObj = new List<GeneralLedgerStatisticDUB>();


            foreach (DataRow row in statisticTable.Rows)
            {
                statisticObj.Add(new GeneralLedgerStatisticDUB()
                {
                    ID = row["ID"].ToInt(),
                    BookingType = row["BookingType"].ToInt(),
                    RentPaymentID = row["UPPaymentID"].ToInt(),
                    RentCreditPrice = UnionPayUtils.FenToYuan(row["RentCreditPrice"].ToStr()).ToDouble(),
                    RentDebitPrice = UnionPayUtils.FenToYuan(row["RentDebitPrice"].ToStr()).ToDouble(),
                    RentalTime = row["RentalTime"].ToDateNull(),
                    RentalPaymentStatus = row["RentalPaymentStatus"] == DBNull.Value ? PaymentStatusEnum.NULL : row["RentalPaymentStatus"].ToStr().ToEnum<PaymentStatusEnum>(),
                    FeeType = row["FeeType"].ToStr().ToEnum<VRentDataDictionay.GLItemDetailType>(),
                    UserID = row["UserID"].ToStr().ToGuid()
                });
            }

            var returnEntity = new DBEntityAggregation<GeneralLedgerStatisticDUB, DBConditionObject>();

            returnEntity.SetEntities(statisticObj);

            return returnEntity;
        }
    }
}
