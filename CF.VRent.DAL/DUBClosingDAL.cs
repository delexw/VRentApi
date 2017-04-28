using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using CF.VRent.UPSDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CF.VRent.DAL
{
    public class DUBClosingDAL
    {
        public const string RetrieveDUBDetails = "Sp_DUBDetail_Retrieve_Dynamic";

        #region DUB Detail
        public static DUBDetailSearchConditions RetrieveDUBDetailsByConditions(DUBDetailSearchConditions conditions, ProxyUserSetting userInfo)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            //@beginDate datetime,
            //@endDate datetime,
            //@bookingNumber nvarchar(50),
            //@userID uniqueidentifier,
            //@paymentStatus tinyint,

            //@itemsPerPage int,
            //@pageNumber int,
            //@totalPages int output

            if (conditions.DateBegin.HasValue)
            {
                SqlParameter beginDatePara = new SqlParameter("@beginDate", conditions.DateBegin.Value);
                parameters.Add(beginDatePara);
            }
            else
            {
                SqlParameter beginDatePara = new SqlParameter("@beginDate", DBNull.Value);
                parameters.Add(beginDatePara);
            }

            if (conditions.DateEnd.HasValue)
            {
                SqlParameter endDatePara = new SqlParameter("@endDate", conditions.DateEnd.Value);
                parameters.Add(endDatePara);
            }
            else
            {
                SqlParameter beginDatePara = new SqlParameter("@endDate", DBNull.Value);
                parameters.Add(beginDatePara); 
            }

            if (!string.IsNullOrEmpty(conditions.KemasBookingNumber))
            {
                SqlParameter bookingNumberPara = new SqlParameter("@bookingNumber", conditions.KemasBookingNumber);
                parameters.Add(bookingNumberPara);
            }
            else
            {
                SqlParameter bookingNumberPara = new SqlParameter("@bookingNumber", DBNull.Value);
                parameters.Add(bookingNumberPara); 
            }

            if (conditions.UserID.HasValue)
            {
                SqlParameter userIDPara = new SqlParameter("@userID", conditions.UserID.Value);
                parameters.Add(userIDPara);
            }
            else
            {
                SqlParameter userIDPara = new SqlParameter("@userID", DBNull.Value);
                parameters.Add(userIDPara); 
            }

            if (!string.IsNullOrEmpty( conditions.UserName))
            {
                SqlParameter userNamePara = new SqlParameter("@userName", conditions.UserName);
                parameters.Add(userNamePara);
            }
            else
            {
                SqlParameter userNamePara = new SqlParameter("@userName", DBNull.Value);
                parameters.Add(userNamePara);
            }


            if (conditions.UPState.HasValue)
            {
                SqlParameter upStatePara = new SqlParameter("@paymentStatus", (int)conditions.UPState.Value);
                parameters.Add(upStatePara);
            }
            else
            {
                SqlParameter upStatePara = new SqlParameter("@paymentStatus", DBNull.Value);
                parameters.Add(upStatePara);
            }
            
            SqlParameter itemsPerPagePara = new SqlParameter("@itemsPerPage", conditions.ItemsPerPage);
            parameters.Add(itemsPerPagePara);

            SqlParameter pageNumberPara = new SqlParameter("@pageNumber", conditions.PageNumber);
            parameters.Add(pageNumberPara);

            SqlParameter totalPagePara = new SqlParameter("@totalPages", conditions.TotalPages);
            totalPagePara.Direction = ParameterDirection.Output;
            parameters.Add(totalPagePara);

            DUBDetail[] items = DataAccessProxyConstantRepo.ExecuteSPReturnReader<DUBDetail[]>(RetrieveDUBDetails, parameters.ToArray(), (sqldatareader) => ReadMultipleDetailFromDataReader(sqldatareader));
            parameters.Clear();

            //construct response
            DUBDetailSearchConditions output = new DUBDetailSearchConditions();
            output.DateBegin = conditions.DateBegin;
            output.DateEnd = conditions.DateEnd;
            output.KemasBookingNumber = conditions.KemasBookingNumber;
            output.UserID = conditions.UserID;
            output.UserName = conditions.UserName;
            output.UPState = conditions.UPState;
            output.ItemsPerPage = conditions.ItemsPerPage;
            output.Items = items;
            output.PageNumber = conditions.PageNumber;
            output.TotalPages = Convert.ToInt32(totalPagePara.Value); //no total pages

            return output;

        }

        private static DUBDetail ReadSingleDetailFromDataReader(SqlDataReader sqlReader)
        {
            DUBDetail item = null;

            while (sqlReader.Read())
            {
                item = new DUBDetail();
                //  [ID]
                //,[BookingID]
                //,KemasBookingID
                //,KemasBookingNumber
                //,[UserID]
                //,[OrderDate]
                //,BookingState
                item.ID = Convert.ToInt32(sqlReader["ID"].ToString());
                item.BookingID = Convert.ToInt32(sqlReader["BookingID"].ToString());
                item.KemasBookingID = Guid.Parse(sqlReader["KemasBookingID"].ToString());
                item.KemasBookingNumber = sqlReader["KemasBookingNumber"].ToString();

                item.State = sqlReader["State"].ToString();

                item.UserID = Guid.Parse(sqlReader["UserID"].ToString());
                item.UserName = string.Format("{0} {1}", sqlReader["UserFirstName"], sqlReader["UserLastName"]);

                item.OrderDate = DateTime.ParseExact(sqlReader["OrderDate"].ToString(), DebitNoteUtility.OrderDateFormat, null);

                //,[PaymentID]
                //,[UPState]
                //,[OrderID]
                //,[OrderItemID]
                //,[Category]
                //,[TotalAmount]
                if (sqlReader["PaymentID"].Equals(DBNull.Value))
                {
                    item.PaymentID = new Nullable<int>();
                }
                else
                {
                    item.PaymentID = Convert.ToInt32(sqlReader["PaymentID"].ToString());
                }

                if (sqlReader["UPState"].Equals(DBNull.Value))
                {
                    item.UPstate = new Nullable<UPProcessingState>();
                }
                else
                {
                    item.UPstate = (UPProcessingState)Enum.Parse(typeof(UPProcessingState), sqlReader["UPState"].ToString());
                }

                if (sqlReader["OrderID"].Equals(DBNull.Value))
                {
                    item.OrderID = new Nullable<int>();
                }
                else
                {
                    item.OrderID = Convert.ToInt32(sqlReader["OrderID"].ToString());
                }

                if (!sqlReader["Category"].Equals(DBNull.Value))
                {
                    item.Category = sqlReader["Category"].ToString();
                }

                if (sqlReader["Amount"].Equals(DBNull.Value))
                {
                    item.TotalAmount = new Nullable<decimal>();
                }
                else
                {
                    item.TotalAmount = decimal.Parse(sqlReader["Amount"].ToString());
                }
            }

            return item;
        }

        private static DUBDetail[] ReadMultipleDetailFromDataReader(SqlDataReader sqlReader)
        {
            List<DUBDetail> items = new List<DUBDetail>();

            while (sqlReader.Read())
            {
                DUBDetail item = new DUBDetail();
                //  [ID]
                //,[BookingID]
                //,KemasBookingID
                //,KemasBookingNumber
                //,[UserID]
                //,[OrderDate]
                //,BookingState
                item.ID = Convert.ToInt32(sqlReader["ID"].ToString());
                item.BookingID = Convert.ToInt32(sqlReader["BookingID"].ToString());
                item.KemasBookingID = Guid.Parse(sqlReader["KemasBookingID"].ToString());
                item.KemasBookingNumber = sqlReader["KemasBookingNumber"].ToString();

                item.State = sqlReader["State"].ToString();

                item.UserID = Guid.Parse(sqlReader["UserID"].ToString());
                item.UserName = string.Format("{0} {1}", sqlReader["UserFirstName"], sqlReader["UserLastName"]);

                item.OrderDate = DateTime.ParseExact(sqlReader["OrderDate"].ToString(), DebitNoteUtility.OrderDateFormat, null);

                //,[PaymentID]
                //,[UPState]
                //,[OrderID]
                //,[OrderItemID]
                //,[Category]
                //,[TotalAmount]
                if (sqlReader["PaymentID"].Equals(DBNull.Value))
                {
                    item.PaymentID = new Nullable<int>();
                }
                else
                {
                    item.PaymentID = Convert.ToInt32(sqlReader["PaymentID"].ToString());
                }

                if (sqlReader["UPState"].Equals(DBNull.Value))
                {
                    item.UPstate = new Nullable<UPProcessingState>();
                }
                else
                {
                    item.UPstate = (UPProcessingState)Enum.Parse(typeof(UPProcessingState), sqlReader["UPState"].ToString());
                }

                if (sqlReader["OrderID"].Equals(DBNull.Value))
                {
                    item.OrderID = new Nullable<int>();
                }
                else
                {
                    item.OrderID = Convert.ToInt32(sqlReader["OrderID"].ToString());
                }

                if (!sqlReader["Category"].Equals(DBNull.Value))
                {
                    item.Category = sqlReader["Category"].ToString();
                }

                if (sqlReader["Amount"].Equals(DBNull.Value))
                {
                    item.TotalAmount = new Nullable<decimal>();
                }
                else
                {
                    item.TotalAmount = decimal.Parse(sqlReader["Amount"].ToString());
                }

                items.Add(item);
            }

            return items.ToArray();
        }
        #endregion
    }
}
