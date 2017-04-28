using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CF.VRent.DAL
{
    public class DebitNoteExcelDAL
    {
        public const string DebitNotePricingCatalog = "Sp_DebitNoteExcel_PricingCatalog";


        public static PricingItemMonthlysummary[] RetrievePricingCatalog(DebitNote[] notes, ProxyUserSetting userInfo) 
        {
            PricingItemMonthlysummary[] output = null;
            if (notes != null && notes.Length > 0)
            {
                IEnumerable<SqlDataRecord> monthlySummary = CreateMonthlySummaryRecord(notes);
                List<SqlParameter> parameters = new List<SqlParameter>();
                SqlParameter idParam = new SqlParameter("@debitNoteID", monthlySummary);
                idParam.SqlDbType = SqlDbType.Structured;
                idParam.TypeName = "dbo.DebitNoteID";
                parameters.Add(idParam);

                output = DataAccessProxyConstantRepo.ExecuteSPReturnReader<PricingItemMonthlysummary[]>(DebitNotePricingCatalog, parameters.ToArray(), (sqldatareader) => ReadDebitNoteMonthlysummaryFromDataReader(sqldatareader));
                parameters.Clear();
            }
            return output;
        }

        private static PricingItemMonthlysummary[] ReadDebitNoteMonthlysummaryFromDataReader(SqlDataReader sqlReader)
        {
            List<PricingItemMonthlysummary> summary = new List<PricingItemMonthlysummary>();

            while (sqlReader.Read())
            {
                PricingItemMonthlysummary pims = new PricingItemMonthlysummary();
                //,vb.UserID			 
                //dnh.ID as PeriodID,
                //targetDebitNote.clientID,
                //vb.ID as BookingID,
                //vo.ID as OrderID,
                //OrderMonth,
                //'INDIRECTFEE' as [Group],
                //Category,
                //voi.Type,
                //TotalAmount

                pims.DebitNoteID = sqlReader["DebitNoteID"].ToString();
                pims.PeriodID = sqlReader["PeriodID"].ToString();
                pims.ClientID = sqlReader["clientID"].ToString();

                pims.kemasBookingID = sqlReader["kemasBookingID"].ToString();
                pims.kemasBookingNumber = sqlReader["kemasBookingNumber"].ToString();
                pims.UserID = sqlReader["UserID"].ToString();

                if (!sqlReader["BookingID"].Equals(DBNull.Value))
                {
                    pims.BookingID = sqlReader["BookingID"].ToString(); 
                }

                if (!sqlReader["OrderID"].Equals(DBNull.Value))
                {
                    pims.OrderID = sqlReader["OrderID"].ToString();
                }

                #region Redundant columns
                // ,cb.StartLocationID as StationID
                //,cb.StartLocationName as StationName
                //,cb.DateBegin
                //,cb.DateEnd
                //,cb.KeyOut
                //,cb.KeyIn
                //,cb.CarID
                //,cb.CarCategory
                //,cb.CarModel
                if (!sqlReader["StationID"].Equals(DBNull.Value))
                {
                    pims.StationID = sqlReader["StationID"].ToString();
                }

                if (!sqlReader["StationName"].Equals(DBNull.Value))
                {
                    pims.StationName = sqlReader["StationName"].ToString();
                }

                if (!sqlReader["DateBegin"].Equals(DBNull.Value))
                {
                    //apply 24-hour system, do not apply 12-hour system, otherwise it loses "PM" or "AM"
                    //Modified by Adam
                    pims.StartTime = DateTime.Parse(sqlReader["DateBegin"].ToString()).ToString("yyyyMMdd HH:mm:ss");
                }

                if (!sqlReader["DateEnd"].Equals(DBNull.Value))
                {
                    //apply 24-hour system, do not apply 12-hour system, otherwise it loses "PM" or "AM"
                    //Modified by Adam
                    pims.EndTime = DateTime.Parse(sqlReader["DateEnd"].ToString()).ToString("yyyyMMdd HH:mm:ss");
                }

                if (!sqlReader["KeyIn"].Equals(DBNull.Value))
                {
                    //apply 24-hour system, do not apply 12-hour system, otherwise it loses "PM" or "AM"
                    //Modified by Adam
                    pims.KeyIn = DateTime.Parse(sqlReader["KeyIn"].ToString()).ToString("yyyyMMdd HH:mm:ss");
                }

                if (!sqlReader["KeyOut"].Equals(DBNull.Value))
                {
                    //apply 24-hour system, do not apply 12-hour system, otherwise it loses "PM" or "AM"
                    //Modified by Adam
                    pims.KeyOut = DateTime.Parse(sqlReader["KeyOut"].ToString()).ToString("yyyyMMdd HH:mm:ss");
                }

                if (!sqlReader["CarID"].Equals(DBNull.Value))
                {
                    pims.CarID = sqlReader["CarID"].ToString();
                }

                if (!sqlReader["CarCategory"].Equals(DBNull.Value))
                {
                    pims.CarCategory = sqlReader["CarCategory"].ToString();
                }

                if (!sqlReader["CarModel"].Equals(DBNull.Value))
                {
                    pims.CarModel = sqlReader["CarModel"].ToString();
                }

                if (!sqlReader["BookingState"].Equals(DBNull.Value))
                {
                    pims.BookingState = sqlReader["BookingState"].ToString();
                }
                #endregion

                pims.OrderMonth = sqlReader["OrderMonth"].ToString();
                pims.Group = sqlReader["Group"].ToString();
                pims.Category = sqlReader["Category"].ToString();
                pims.Type = sqlReader["Type"].ToString();

                if (!sqlReader["Description"].Equals(DBNull.Value))
                {
                    pims.Description = sqlReader["Description"].ToString(); 
                }

                pims.Total = decimal.Parse(sqlReader["TotalAmount"].ToString());
                summary.Add(pims);
            }

            return summary.ToArray();
        }

        //parent records
        private static IEnumerable<SqlDataRecord> CreateMonthlySummaryRecord(DebitNote[] dn)
        {
            SqlMetaData[] metaData = new SqlMetaData[3]
            {
                //[ID] [int] NULL,
                //[PeriodID] [int] NULL,
                //[clientID] [uniqueidentifier] NULL

                new SqlMetaData("ID",SqlDbType.Int),
                new SqlMetaData("PeriodID",SqlDbType.Int),
                new SqlMetaData("clientID",SqlDbType.UniqueIdentifier)
            };
            return dn.Select(m => SetMonthlySummaryValues(metaData, m));
        }
        private static SqlDataRecord SetMonthlySummaryValues(SqlMetaData[] metaData, DebitNote note)
        {
            SqlDataRecord bc = new SqlDataRecord(metaData);
            bc.SetInt32(0, note.ID);
            bc.SetInt32(1,note.PeriodID);
            bc.SetGuid(2,note.ClientID);
            return bc;
        }
    }
}
