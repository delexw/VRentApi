using CF.VRent.Common;
using CF.VRent.Common.Entities;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.Log;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CF.VRent.DAL
{
    public class PrincingHelper
    {

        #region Convert a FE Princing Info to a DB princing info
        public static ProxyBookingPrice ConvertFromFEPriceInfo(BookingPriceInfo priceInfo)
        {
            ProxyBookingPrice price = null;

            try
            {
                price = new ProxyBookingPrice();


                price.Timestamp = priceInfo.TimeStamp;
                price.Total = priceInfo.Total;
                price.TagID = priceInfo.ID;

                List<ProxyPrincingItem> items = new List<ProxyPrincingItem>();

                //old version
                //items.Add(ConvertFromRental(priceInfo.Rental));
                //new version
                items.AddRange(ConvertFromRentalItems(priceInfo.Rental));

                items.Add(ConvertFromInsuranceFee(priceInfo.InsuranceFee));
                items.Add(ConvertFromFuel(priceInfo.Fuel));
                items.AddRange(ConvertFromFineItems(priceInfo.Fine.Items));

                price.PrincingItems = items.ToArray();
            }
            catch
            {
                price = null;
            }


            return price;
        }

        //private static ProxyPrincingItem ConvertFromRental(RentalFee rental)
        //{
        //    return new ProxyPrincingItem()
        //    {
        //        Category = PrincingInfoFactory.RentalNode,
        //        Group = PrincingInfoFactory.RentalFeeCategory,
        //        Total = rental.Total,
        //    };
        //}

        private static ProxyPrincingItem[] ConvertFromRentalItems(RentalFee rental)
        {
            List<ProxyPrincingItem> rentalItems = new List<ProxyPrincingItem>();
            if (rental.Items != null && rental.Items.Length > 0)
            {
                foreach (var item in rental.Items)
                {
                    ProxyPrincingItem ppi = new ProxyPrincingItem()
                    {
                        Category = item.Description,
                        Group = PrincingInfoFactory.RentalFeeCategory,
                        Description = item.RawDescription,
                        Total = item.Total
                    };
                }
            }
            return rentalItems.ToArray();
        }

        private static ProxyPrincingItem ConvertFromInsuranceFee(Insurance insurance)
        {
            return new ProxyPrincingItem()
            {
                Category = PrincingInfoFactory.InsuranceFeeNode,
                Group = PrincingInfoFactory.RentalFeeCategory,
                Total = insurance.Total,
            };
        }

        private static ProxyPrincingItem ConvertFromFuel(FuelFee fuel)
        {
            return new ProxyPrincingItem()
            {
                Category = PrincingInfoFactory.FuelNode,
                Group = PrincingInfoFactory.RentalFeeCategory,
                Quantity = fuel.Kilometer,
                Total = fuel.Total,
            };
        }

        private const string FineCategor = "Fine";

        private static ProxyPrincingItem[] ConvertFromFineItems(Item[] fineItems)
        {
            List<ProxyPrincingItem> fines = new List<ProxyPrincingItem>();

            foreach (var item in fineItems)
            {
                ProxyPrincingItem ppi = new ProxyPrincingItem()
                {
                    Category = FineCategor,
                    Type = item.Type,
                    Group = PrincingInfoFactory.RentalFeeCategory,
                    Description = item.Description,
                    Total = item.Total,
                };

                fines.Add(ppi);
            }

            return fines.ToArray();
        }

        #endregion


    }

    public class PricingDAL
    {
        public const string CreateProxyPrincingSP = "Sp_VrentPrincing_Create";
        public const string ReadProxyPrincingSP = "Sp_VrentPrincing_Read";

        #region Read a price detail by proxybookingID
        public static ProxyBookingPrice LoadPricingItems(int proxyBookingID)
        {
            ProxyBookingPrice pbp = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter userPara = new SqlParameter("@BookingID", proxyBookingID);
            parameters.Add(userPara);

            pbp = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyBookingPrice>(ReadProxyPrincingSP, parameters.ToArray(), (sqldatareader) => ReadPricingInfoFromReaderByProxyBookingID(sqldatareader));
            parameters.Clear();

            return pbp;
        }

        #endregion

        private static void ReadPricingInfoFromReader(SqlDataReader pricingReader, ref ProxyBookingPrice bookingPricing)
        {

            bookingPricing = new ProxyBookingPrice();

            List<ProxyPrincingItem> pricingItems = new List<ProxyPrincingItem>();
            int count = ReadPrincing(pricingReader, ref bookingPricing);
            int itemsCnt = ReadPrincingDetail(pricingReader, ref pricingItems);

            if (count == 1)
            {
                bookingPricing.PrincingItems = pricingItems.ToArray();
            }
            else
            {
                //throw exception
            }
        }

        #region Helper read Method
        private static ProxyBookingPrice ReadPricingInfoFromReaderByProxyBookingID(SqlDataReader pricingReader)
        {

            ProxyBookingPrice bookingPricing = new ProxyBookingPrice();

            List<ProxyPrincingItem> pricingItems = new List<ProxyPrincingItem>();
            int count = ReadPrincing(pricingReader, ref bookingPricing);
            int itemsCnt = ReadPrincingDetail(pricingReader, ref pricingItems);

            if (count == 1)
            {
                bookingPricing.PrincingItems = pricingItems.ToArray();
            }
            else
            {
                ReturnResult badData = new ReturnResult()
                {
                    Code = DataAccessProxyConstantRepo.DataAccessProxyBadDataExceptionCode,
                    Message = DataAccessProxyConstantRepo.DataAccessProxyBadDataExceptionMessage,
                    Type = ResultType.DATAACCESSProxy
                };
                throw new FaultException<ReturnResult>(badData,badData.Message);
            }

            return bookingPricing;
        }


        private static int ReadPrincing(SqlDataReader sqlReader, ref ProxyBookingPrice pricing)
        {
            int counter = 0;
            while (sqlReader.Read())
            {
                counter++;

                //[ID]
                //,[BookingID]
                //,[Total]
                //,[TimeStamp]
                //,[TagID]
                //,[state]
                //,[CreatedOn]
                //,[CreatedBy]
                //,[ModifiedOn]
                //,[ModifiedBy]

                pricing.ID = Convert.ToInt32(sqlReader[0].ToString());
                pricing.ProxyBookingID = Convert.ToInt32(sqlReader[1].ToString());

                pricing.Total = Convert.ToDecimal(sqlReader[2].ToString());
                pricing.Timestamp = sqlReader[3].Equals(DBNull.Value) ? null : sqlReader[3].ToString();
                pricing.TagID = sqlReader[4].Equals(DBNull.Value) ? null : sqlReader[4].ToString();

                pricing.State = Convert.ToInt16(sqlReader[5].ToString());
                pricing.CreatedOn = Convert.ToDateTime(sqlReader[6].ToString());
                pricing.CreatedBy = Guid.Parse(sqlReader[7].ToString());
                pricing.ModifiedOn = sqlReader[8].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[8].ToString());
                pricing.ModifiedBy = sqlReader[9].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[9].ToString());
            }

            return counter;
        }

        private static int ReadPrincingDetail(SqlDataReader sqlReader, ref List<ProxyPrincingItem> pricingDetails)
        {
            int counter = 0;
            if (sqlReader.NextResult())
            {
                while (sqlReader.Read())
                {
                    counter++;

                    ProxyPrincingItem ppi = new ProxyPrincingItem();
                    //  [ID]
                    //,[PrincingID]
                    //,[Description]
                    //,[Group]
                    //,[Category]
                    //,[Type]
                    //,[UnitPrice]
                    //,[Quantity]
                    //,[Total]
                    //,[State]
                    //,[CreatedOn]
                    //,[CreatedBy]
                    //,[ModifiedOn]
                    //,[ModifiedBy]

                    ppi.ID = Convert.ToInt32(sqlReader[0].ToString());
                    ppi.BookingPriceID = Convert.ToInt32(sqlReader[1].ToString());

                    ppi.Description = sqlReader[2].Equals(DBNull.Value) ? null : sqlReader[2].ToString();
                    ppi.Group = sqlReader[3].Equals(DBNull.Value) ? null : sqlReader[3].ToString();
                    ppi.Category = sqlReader[4].Equals(DBNull.Value) ? null : sqlReader[4].ToString();
                    ppi.Type = sqlReader[5].Equals(DBNull.Value) ? null : sqlReader[5].ToString();

                    ppi.UnitPrice = sqlReader[6].Equals(DBNull.Value) ? new Nullable<decimal>() : Convert.ToDecimal(sqlReader[6].ToString());
                    ppi.Quantity = sqlReader[7].Equals(DBNull.Value) ? new Nullable<decimal>() : Convert.ToDecimal(sqlReader[7].ToString());
                    ppi.Total = Convert.ToDecimal(sqlReader[8].ToString());

                    ppi.State = Convert.ToInt16(sqlReader[9].ToString());
                    ppi.CreatedOn = Convert.ToDateTime(sqlReader[10].ToString());
                    ppi.CreatedBy = Guid.Parse(sqlReader[11].ToString());
                    ppi.ModifiedOn = sqlReader[12].Equals(DBNull.Value) ? new Nullable<DateTime>() : Convert.ToDateTime(sqlReader[12].ToString());
                    ppi.ModifiedBy = sqlReader[13].Equals(DBNull.Value) ? new Nullable<Guid>() : Guid.Parse(sqlReader[13].ToString());

                    pricingDetails.Add(ppi);
                }
            }

            return counter;
        }

        #endregion

        public static ProxyBookingPrice SavePricingItems(ProxyBookingPrice bookingPrice)
        {

            //refactor codes later
            List<ProxyBookingPrice> prices = new List<ProxyBookingPrice>();
            prices.Add(bookingPrice);
            IEnumerable<SqlDataRecord> princingRecords = CreatePrincingRecord(prices);
            IEnumerable<SqlDataRecord> princingDetails = CreatePrincingDetailRecords(bookingPrice);
            return SavePricingItems(bookingPrice.ProxyBookingID, princingRecords, princingDetails);
        }

        #region Helper Methods
        private static ProxyBookingPrice SavePricingItems(int bookingID, IEnumerable<SqlDataRecord> bookingPrice, IEnumerable<SqlDataRecord> priceDetails)
        {
            ProxyBookingPrice pbp = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter bookingIDParam = new SqlParameter("@BookingID", bookingID);
            parameters.Add(bookingIDParam);

            //input
            SqlParameter priceParam = new SqlParameter("@PrincingInfo", bookingPrice);
            priceParam.SqlDbType = SqlDbType.Structured;
            priceParam.TypeName = "dbo.BookingPrice";
            parameters.Add(priceParam);

            //input
            SqlParameter priceDetailParam = new SqlParameter("@PrincingDetailInfo", priceDetails);
            priceDetailParam.SqlDbType = SqlDbType.Structured;
            priceDetailParam.TypeName = "dbo.BookingPriceItem";
            parameters.Add(priceDetailParam);

            pbp = DataAccessProxyConstantRepo.ExecuteSPReturnReader<ProxyBookingPrice>(CreateProxyPrincingSP, parameters.ToArray(), (sqldatareader) => ReadPricingInfoFromReaderByProxyBookingID(sqldatareader));
            parameters.Clear();

            return pbp;
        }


        //parent records
        public static IEnumerable<SqlDataRecord> CreatePrincingRecord(IEnumerable<ProxyBookingPrice> princings)
        {
            //price.BookingID,
            //price.[Total],
            //price.[TagID],
            //price.[TimeStamp],
            //price.[state],
            //price.[CreatedOn],
            //price.[CreatedBy],
            //price.[ModifiedOn],
            //price.[ModifiedBy]
            SqlMetaData[] metaData = new SqlMetaData[9]
            {
                new SqlMetaData("BookingID",SqlDbType.Int),
                new SqlMetaData("Total",SqlDbType.Decimal,10,3),
                new SqlMetaData("TagID",SqlDbType.NVarChar,50),
                new SqlMetaData("TimeStamp",SqlDbType.NVarChar,50),
                new SqlMetaData("state",SqlDbType.TinyInt),
                new SqlMetaData("CreatedOn",SqlDbType.DateTime),
                new SqlMetaData("CreatedBy",SqlDbType.UniqueIdentifier),
                new SqlMetaData("ModifiedOn",SqlDbType.DateTime),
                new SqlMetaData("ModifiedBy",SqlDbType.UniqueIdentifier),
            };



            return princings.Select(m => SetPrincingValues(metaData, m));
        }
        public static SqlDataRecord SetPrincingValues(SqlMetaData[] metaData, ProxyBookingPrice princing)
        {
            SqlDataRecord price = new SqlDataRecord(metaData);
            price.SetSqlInt32(0, princing.ProxyBookingID);
            price.SetDecimal(1, princing.Total);

            if (string.IsNullOrEmpty(princing.Timestamp))
            {
                price.SetDBNull(2);
            }
            else
            {
                price.SetString(2, princing.Timestamp);
            }

            if (string.IsNullOrEmpty(princing.TagID))
            {
                price.SetDBNull(3);
            }
            else
            {
                price.SetString(3, princing.TagID);
            }

            price.SetByte(4, (byte)princing.State);

            price.SetDateTime(5, princing.CreatedOn.Value);
            price.SetSqlGuid(6, princing.CreatedBy.Value);

            if (princing.ModifiedOn == null)
            {
                price.SetDBNull(7);
            }
            else
            {
                price.SetDateTime(7, princing.ModifiedOn.Value);
            }

            if (princing.ModifiedBy == null)
            {
                price.SetDBNull(8);
            }
            else
            {
                price.SetSqlGuid(8, princing.ModifiedBy.Value);
            }

            return price;
        }

        //child records

        public static IEnumerable<SqlDataRecord> CreatePrincingDetailRecords(ProxyBookingPrice princings)
        {
            //[PrincingID] [int] NOT NULL,
            //[Description] [nvarchar](50) NULL,
            //[Group] [nvarchar](20) NOT NULL,
            //[Category] [nvarchar](20) NULL,
            //[Type] [nvarchar](20) NULL,
            //[UnitPrice] [decimal](18, 0) NULL,
            //[Quantity] [decimal](18, 0) NULL,
            //[Total] [decimal](18, 0) NULL,
            //[State] [tinyint] NOT NULL,
            //[CreatedOn] [datetime] NOT NULL,
            //[CreatedBy] [uniqueidentifier] NOT NULL,
            //[ModifiedOn] [datetime] NULL,
            //[ModifiedBy] [nchar](10) NULL
            SqlMetaData[] metaData = new SqlMetaData[13]
            {
                new SqlMetaData("PrincingID",SqlDbType.Int),
                new SqlMetaData("Description",SqlDbType.NVarChar,500),
                new SqlMetaData("Group",SqlDbType.NVarChar,20),

                new SqlMetaData("Category",SqlDbType.NVarChar,20),
                new SqlMetaData("Type",SqlDbType.NVarChar,20),
                new SqlMetaData("UnitPrice",SqlDbType.Decimal,10,3),
                new SqlMetaData("Quantity",SqlDbType.Decimal,10,3),
                new SqlMetaData("Total",SqlDbType.Decimal,10,3),

                new SqlMetaData("State",SqlDbType.TinyInt),
                new SqlMetaData("CreatedOn",SqlDbType.DateTime),
                new SqlMetaData("CreatedBy",SqlDbType.UniqueIdentifier),
                new SqlMetaData("ModifiedOn",SqlDbType.DateTime),
                new SqlMetaData("ModifiedBy",SqlDbType.UniqueIdentifier),
            };


            return princings.PrincingItems.Select(m => SetPrincingDetailValues(metaData, m));
        }
        private static SqlDataRecord SetPrincingDetailValues(SqlMetaData[] metaData, ProxyPrincingItem princing)
        {
            SqlDataRecord priceItem = new SqlDataRecord(metaData);
            priceItem.SetSqlInt32(0, princing.BookingPriceID);
            if (string.IsNullOrEmpty(princing.Description))
            {
                priceItem.SetDBNull(1);
            }
            else
            {
                priceItem.SetString(1, princing.Description);
            }
            priceItem.SetString(2, princing.Group);

            if (string.IsNullOrEmpty(princing.Category))
            {
                priceItem.SetDBNull(3);
            }
            else
            {
                priceItem.SetString(3, princing.Category);
            }

            if (string.IsNullOrEmpty(princing.Type))
            {
                priceItem.SetDBNull(4);
            }
            else
            {
                priceItem.SetString(4, princing.Type);
            }

            if (princing.UnitPrice == null)
            {
                priceItem.SetDBNull(5);
            }
            else
            {
                priceItem.SetDecimal(5, princing.UnitPrice.Value);
            }

            if (princing.Quantity == null)
            {
                priceItem.SetDBNull(6);
            }
            else
            {
                priceItem.SetDecimal(6, princing.Quantity.Value);
            }

            priceItem.SetDecimal(7, princing.Total);

            priceItem.SetByte(8, (byte)princing.State);

            priceItem.SetDateTime(9, princing.CreatedOn.Value);
            priceItem.SetSqlGuid(10, princing.CreatedBy.Value);

            if (princing.ModifiedOn == null)
            {
                priceItem.SetDBNull(11);
            }
            else
            {
                priceItem.SetDateTime(11, princing.ModifiedOn.Value);
            }

            if (princing.ModifiedBy == null)
            {
                priceItem.SetDBNull(12);
            }
            else
            {
                priceItem.SetSqlGuid(12, princing.ModifiedBy.Value);
            }

            return priceItem;
        }


        #endregion
    }
}
