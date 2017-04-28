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
    public class CompletedBookingDAL
    {
        #region
        public static void GenerateJobSchedule()
        {
            int affectedCnt = -1;

            List<SqlParameter> parameters = new List<SqlParameter>();

            //affectedCnt = DataAccessProxyConstantRepo.ExecuteSPNonQuery(GenerateDebitNotesByMonth, parameters.ToArray());
            parameters.Clear();

        }
        private const string CompletedBookingsTable = "CompletedBookings";
        private const string SelectCmd = "Select * from CompletedBookings";
        public static void SaveStagedBookings(StagedBookings bookings, ProxyUserSetting userInfo) 
        {
            DataSet ds = DataAccessProxyConstantRepo.RetrieveTableSchema(SelectCmd, CompletedBookingsTable);
            DataTable dt = ds.Tables[CompletedBookingsTable];

            if(bookings.Items != null && bookings.Items.Length > 0)
            {
                //int index = 0;
                foreach (var completeBooking in bookings.Items)
                {
                    DataRow booking = dt.NewRow();
                      //[ID]
                    //booking["ID"] = index++;
                      //,[KemasBookingID]
                    booking["KemasBookingID"] = completeBooking.KemasBookingID;
                      //,[BookingID]
                    if (completeBooking.BookingID == null)
                    {
                        booking["BookingID"] = DBNull.Value;
                    }
                    else
                    {
                        booking["BookingID"] = completeBooking.BookingID; 
                    }
                      //,[DateBegin]
                    booking["DateBegin"] = completeBooking.DateBegin;
                      //,[DateEnd]
                    booking["DateEnd"] = completeBooking.DateEnd;
                      //,[StartLocationID]
                    booking["StartLocationID"] = completeBooking.StartLocationID;
                      //,[StartLocationName]
                    booking["StartLocationName"] = completeBooking.StartLocationName;
                      //,[BillingOption]
                    booking["BillingOption"] = completeBooking.BillingOption;
                      //,[Category]
                    booking["Category"] = completeBooking.Category;
                      //,[CreatorID]
                    booking["CreatorID"] = completeBooking.CreatorID;
                      //,[UserID]
                    booking["UserID"] = completeBooking.UserID;
                      //,[CorporateID]
                    booking["CorporateID"] = completeBooking.CorporateID;
                      //,[CorporateName]
                    booking["CorporateName"] = completeBooking.CorporateName;
                      //,[CarID]
                    if (completeBooking.CarID == null)
                    {
                        booking["CarID"] = DBNull.Value;
                    }
                    else
                    {
                        booking["CarID"] = completeBooking.CarID; 
                    }
                      //,[CarName]
                    booking["CarName"] = completeBooking.CarName;
                      //,[KemasBookingNumber]
                    booking["KemasBookingNumber"] = completeBooking.KemasBookingNumber;
                      //,[PickupBegin]
                    booking["PickupBegin"] = completeBooking.PickupBegin;
                      //,[PickupEnd]
                    booking["PickupEnd"] = completeBooking.PickupEnd;
                      //,[KeyOut]
                    if (completeBooking.KeyOut == null)
                    {
                        booking["KeyOut"] = DBNull.Value;
                    }
                    else
                    {
                        booking["KeyOut"] = completeBooking.KeyOut; 
                    }

                      //,[KeyIn]
                    if (completeBooking.KeyIn == null)
                    {
                        booking["KeyIn"] = DBNull.Value;
                    }
                    else
                    {
                        booking["KeyIn"] = completeBooking.KeyIn;
                    }
                      //,[State]
                    booking["State"] = completeBooking.State;
                      //,[Price]
                    booking["Price"] = completeBooking.Price;
                      //,[PricingDetail]
                    booking["PricingDetail"] = completeBooking.PricingDetail;
                      //,[PaymentStatus]
                    booking["PaymentStatus"] = completeBooking.PaymentStatus;
                      //,[SyncState]
                    booking["SyncState"] = completeBooking.SyncState;
                      //,[MatchState]
                    booking["MatchState"] = completeBooking.CompareResult;
                      //,[CreatedOn]
                    booking["CreatedOn"] = completeBooking.CreatedOn;
                      //,[CreatedBy]
                    booking["CreatedBy"] = completeBooking.CreatedBy;
                      //,[ModifiedOn]
                    if (completeBooking.ModifiedOn == null)
                    {
                        booking["ModifiedOn"] = DBNull.Value;
                    }
                    else
                    {
                        booking["ModifiedOn"] = completeBooking.ModifiedOn; 
                    }
                      //,[ModifiedBy]
                    if (completeBooking.ModifiedOn == null)
                    {
                        booking["ModifiedBy"] = DBNull.Value;
                    }
                    else
                    {
                        booking["ModifiedBy"] = completeBooking.ModifiedBy;
                    }


                    dt.Rows.Add(booking);
                }
            }
            DataAccessProxyConstantRepo.SaveCompletedBookings(dt, CompletedBookingsTable);
        }
        #endregion

        #region Helper Methods

        private static CompletedBooking ReadSingleBookingFromDataReader(SqlDataReader sqlReader)
        {
            CompletedBooking completed = new CompletedBooking();

            while (sqlReader.Read())
            {
                //[ID]
                //,[KemasBookingID]
                //,[BookingID]
                //,[DateBegin]
                //,[DateEnd]

                completed.ID = Convert.ToInt32(sqlReader["ID"].ToString());
                completed.KemasBookingID = Guid.Parse(sqlReader["KemasBookingID"].ToString());

                if(!sqlReader["BookingID"].Equals(DBNull.Value))
                {
                    completed.BookingID = Convert.ToInt32(sqlReader["BookingID"].ToString());
                }
                completed.DateBegin =  DateTime.Parse( sqlReader["DateBegin"].ToString());
                completed.DateEnd =  DateTime.Parse( sqlReader["DateEnd"].ToString());

                //,[StartLocationID]
                //,[StartLocationName]
                //,[BillingOption]
                //,[Category]
                //,[CreatorID]                
                completed.StartLocationID = Guid.Parse( sqlReader["StartLocationID"].ToString());
                completed.StartLocationName =  sqlReader["StartLocationName"].ToString();
                completed.BillingOption = Convert.ToInt32 (sqlReader["BillingOption"].ToString());
                completed.Category =  sqlReader["Category"].ToString();
                completed.CreatorID = Guid.Parse( sqlReader["CreatorID"].ToString());

                //,[UserID]
                //,[CorporateID]
                //,[CorporateName]
                //,[CarID]
                //,[CarName]
                completed.UserID = Guid.Parse( sqlReader["UserID"].ToString());
                completed.CorporateID =  sqlReader["CorporateID"].ToString();
                completed.CorporateName =  sqlReader["CorporateName"].ToString();

                if (!sqlReader["CarID"].Equals(DBNull.Value))
                {
                    completed.CarID = Guid.Parse(sqlReader["CarID"].ToString()); 
                }
                completed.CarName = sqlReader["CarName"].ToString();

                //,[KemasBookingNumber]
                //,[PickupBegin]
                //,[PickupEnd]
                //,[KeyOut]
                //,[KeyIn]
                completed.KemasBookingNumber = sqlReader["KemasBookingNumber"].ToString();
                completed.PickupBegin =  DateTime.Parse( sqlReader["PickupBegin"].ToString());
                completed.PickupEnd =  DateTime.Parse( sqlReader["PickupEnd"].ToString());
                completed.KeyOut = DateTime.Parse( sqlReader["KeyOut"].ToString());
                completed.KeyIn = DateTime.Parse( sqlReader["KeyIn"].ToString());


                //,[State]
                //,[Price]
                //,[PricingDetail]
                //,[PaymentStatus]
                //,[SyncState]
                completed.State = sqlReader["State"].ToString();
                completed.Price = decimal.Parse(sqlReader["Price"].ToString());
                completed.PricingDetail = sqlReader["PriceDetail"].ToString();
                completed.PaymentStatus = sqlReader["PaymentStatus"].ToString();
                completed.SyncState = (StagingState)Enum.Parse(typeof(StagingState), sqlReader["SyncState"].ToString());

                //,[MatchState]
                //,[CreatedOn]
                //,[CreatedBy]
                //,[ModifiedOn]
                //,[ModifiedBy]
                completed.CompareResult = (MatchState)Enum.Parse(typeof(StagingState), sqlReader["MatchState"].ToString());
                completed.CreatedOn = Convert.ToDateTime(sqlReader["CreatedOn"].ToString());
                completed.CreatedBy = Guid.Parse(sqlReader["CreatedBy"].ToString());

                completed.ModifiedOn = sqlReader["ModifiedOn"].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader["ModifiedOn"].ToString());
                completed.ModifiedBy = sqlReader["ModifiedBy"].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader["ModifiedBy"].ToString());
            }

            return completed;
        }

        private static ProxyReservation[] ReadMultipleBookingFromDataReader(SqlDataReader sqlReader)
        {
            List<ProxyReservation> bookings = new List<ProxyReservation>();

            while (sqlReader.Read())
            {
                ProxyReservation booking = new ProxyReservation();

                booking.ProxyBookingID = Convert.ToInt32(sqlReader[0].ToString());

                booking.BillingOption = Convert.ToInt32(sqlReader[1].ToString());
                booking.KemasBookingID = Guid.Parse(sqlReader[2].ToString());
                booking.KemasBookingNumber = sqlReader[3].ToString();

                //,[DateBegin]
                //,[DateEnd]
                //,[TotalAmount]
                booking.DateBegin = Convert.ToDateTime(sqlReader[4].ToString());
                booking.DateEnd = Convert.ToDateTime(sqlReader[5].ToString());
                booking.TotalAmount = Convert.ToDecimal(sqlReader[6].ToString());

                //,[UserID]
                //,[UserFirstName]
                //,[UserLastName]
                booking.UserID = Guid.Parse(sqlReader[7].ToString());
                booking.UserFirstName = sqlReader[8].ToString();
                booking.UserLastName = sqlReader[9].ToString();

                //,[CorporateID]
                //,[CorporateName]
                booking.CorporateID = sqlReader[10].ToString();
                booking.CorporateName = sqlReader[11].ToString();

                //,[CreatorID]
                //,[CreatorFirstName]
                //,[CreatorLastName]
                booking.CreatorID = sqlReader[12].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[12].ToString());
                booking.CreatorFirstName = sqlReader[13].ToString();
                booking.CreatorLastName = sqlReader[14].ToString();

                //,[StartLocationID]
                //,[StartLocationName]
                booking.StartLocationID = sqlReader[15].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[15].ToString());
                booking.StartLocationName = sqlReader[16].ToString();

                //,[State]
                //,[CreatedOn]
                //,[CreatedBy]
                //,[ModifiedOn]
                //,[ModifiedBy]
                booking.State = sqlReader[17].ToString();
                booking.CreatedOn = Convert.ToDateTime(sqlReader[18].ToString());
                booking.CreatedBy = Guid.Parse(sqlReader[19].ToString());

                booking.ModifiedOn = sqlReader[20].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[20].ToString());
                booking.ModifiedBy = sqlReader[21].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[21].ToString());

                bookings.Add(booking);
            }

            return bookings.ToArray();
        }

        private static void ProceesingProxyPricing(ProxyReservation booking, ProxyBookingPrice pbp, ref List<SqlDataRecord> pricing, ref List<SqlDataRecord> pricingItems)
        {
            List<ProxyBookingPrice> prices = new List<ProxyBookingPrice>();
            prices.Add(pbp);
            pricing.AddRange(PricingDAL.CreatePrincingRecord(prices));
            pricingItems.AddRange(PricingDAL.CreatePrincingDetailRecords(pbp));

            if (pricing.Count == 0)
            {
                pricing = null;
            }

            if (pricingItems.Count == 0)
            {
                pricingItems = null;
            }
        }

        //private static void ProceesingProxyPricing(ProxyReservation booking, string priceDetail, ref List<SqlDataRecord> pricing, ref List<SqlDataRecord> pricingItems)
        //{
        //    ProxyBookingPrice pbp = null;
        //    List<ProxyBookingPrice> prices = new List<ProxyBookingPrice>();

        //    if (!string.IsNullOrEmpty(priceDetail))
        //    {
        //        PrincingInfoFactory factory = new PrincingInfoFactory(priceDetail);
        //        factory.Process();
        //        pbp = PrincingHelper.ConvertFromFEPriceInfo(factory.Price);
        //    }
        //    else
        //    {
        //        pbp = new ProxyBookingPrice();
        //        pbp.Total = booking.TotalAmount.Value;
        //        pbp.PrincingItems = new List<ProxyPrincingItem>().ToArray();
        //    }

        //    //set system columns to proper values
        //    pbp.CreatedOn = booking.CreatedOn;
        //    pbp.CreatedBy = booking.CreatedBy;

        //    if (booking.ProxyBookingID > 0)
        //    {
        //        // update booking
        //        pbp.ProxyBookingID = booking.ProxyBookingID;
        //        pbp.ModifiedOn = booking.ModifiedOn;
        //        pbp.ModifiedBy = booking.ModifiedBy;
        //    }
        //    else
        //    {
        //        //create booking
        //        pbp.ProxyBookingID = -1;
        //    }


        //    foreach (ProxyPrincingItem item in pbp.PrincingItems)
        //    {
        //        item.CreatedOn = booking.CreatedOn;
        //        item.CreatedBy = booking.CreatedBy;
        //    } 

        //    prices.Add(pbp);
        //    pricing.AddRange(PricingDAL.CreatePrincingRecord(prices));
        //    pricingItems.AddRange(PricingDAL.CreatePrincingDetailRecords(pbp));



        //}

        #endregion
    }
}
