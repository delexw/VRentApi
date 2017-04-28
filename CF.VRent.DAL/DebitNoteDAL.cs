using CF.VRent.Common;
using CF.VRent.Common.UserContracts;
using CF.VRent.DataAccessProxy.Entities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace CF.VRent.DAL
{
    public class DebitNoteUtility
    {
        public const string OrderDateFormat = "yyyyMMdd";
        public static void DeterminePaymentStatus(DebitNote[] notes, DateTime queryTime)
        {
            if (notes != null && notes.Length > 0)
            {
                foreach (var note in notes)
                {
                    if (!note.PaymentDate.HasValue && note.PaymentStatus == PaymentState.Pending)
                    {
                        if ((queryTime - note.BillingDate).Days > 30)
                        {
                            note.PaymentStatus = PaymentState.OverDue;
                        }
                    }
                }
            }
        }


        private const string WhereConditionPrefix = "Where 1=1";
        private const string BookingWhereConditionPrefix = " AND 1=1";
        private const string DateTimePattern = "yyyy-MM-dd HH:mm:ss.sss";

        public const string DebitNoteIDField = "DebitNoteID";
        public const string KemasBookingNumberField = "KemasBookingNumber";
        public const string UserIDField = "UserID";
        public const string DateBeginField = "OrderDate";
        public const string DateEndField = "OrderDate";

        public static string EvaluateBookingWhereConditions(DebitNoteDetailSearchConditions ddsc)
        {
            StringBuilder sb = new StringBuilder(BookingWhereConditionPrefix);

            if (!string.IsNullOrEmpty(ddsc.KemasBookingNumber))
            {
                string temp = string.Format(" AND {0} = {1}", KemasBookingNumberField, EscapeFieldToSqlPart(ddsc.KemasBookingNumber.PadLeft(20, '0')));
                sb.Append(temp);
            }

            if (ddsc.UserID.HasValue)
            {
                string temp = string.Format(" AND {0} = {1}", UserIDField, EscapeFieldToSqlPart(ddsc.UserID.Value.ToString()));
                sb.Append(temp);
            }

            return sb.ToString();
        }

        private static string EscapeFieldToSqlPart(string value)
        {
            return "'" + value.Replace("'", "''") + "'";
        }
    }

    public class DebitNoteDAL
    {
        public const string RetrieveDebitNotesPeriods = "Sp_DebitNoteHistory_Retrieve";

        public const string Sp_DebitNotePeriod_RetrieveByState = "Sp_DebitNotePeriod_RetrieveByState";

        public const string RetrieveDebitNoteCompletedPeriods = "Sp_DebitNote_RetrieveCompletedPeriods";

        public const string ClearUpDebitNoteTempData = "Sp_DebitNoteTempData_ClearUp";

        public const string GenerateDebitNotesByMonth = "Sp_DebitNotes_GenerateByMonth_Dynamic";

        public const string RetrieveDebitNotes = "Sp_DebitNotes_Retrieve";

        public const string RetrieveDebitNoteByID = "Sp_DebitNotes_RetrievebyID";

        public const string UpdateDebitNoteByID = "Sp_DebitNotes_UpdateByID";

        public const string Sp_DebitNoteDetail_RetrieveByConditions_Dynamic = "Sp_DebitNoteDetail_RetrieveByConditions_Dynamic";

        public const string RetrieveBookingIDs = "Sp_VrentBookings_RetrieveID";


        #region Debit-Note Detail

        public static DebitNoteDetailSearchConditions RetrieveDebitNoteDetailsInRange(DebitNoteDetailSearchConditions conditions, ProxyUserSetting userInfo)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            //@whereConditionsPara int,
            //@itemsPerPage int,
            //@pageNumber int,
            //@TotalPage int output
            SqlParameter debitNoteIDPara = new SqlParameter("@debitNoteID", conditions.DebitNoteID);
            parameters.Add(debitNoteIDPara);

            SqlParameter dateBeginPara = new SqlParameter("@dateBegin", conditions.DateBegin.Value);
            parameters.Add(dateBeginPara);

            SqlParameter dateEndPara = new SqlParameter("@dateEnd", conditions.DateEnd.Value);
            parameters.Add(dateEndPara);

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

            if (!string.IsNullOrEmpty(conditions.UserName))
            {
                SqlParameter userNamePara = new SqlParameter("@userName", conditions.UserName);
                parameters.Add(userNamePara);
            }
            else
            {
                SqlParameter userNamePara = new SqlParameter("@userName", DBNull.Value);
                parameters.Add(userNamePara);
            }

            
            DebitNoteDetail[] items = DataAccessProxyConstantRepo.ExecuteSPReturnReader<DebitNoteDetail[]>(Sp_DebitNoteDetail_RetrieveByConditions_Dynamic, parameters.ToArray(), (sqldatareader) => ReadMultipleDetailFromDataReader(sqldatareader));
            parameters.Clear();

            //construct response
            DebitNoteDetailSearchConditions output = new DebitNoteDetailSearchConditions();
            output.DebitNoteID = conditions.DebitNoteID; //must have value

            if (conditions.DateBegin.HasValue)
            {
                output.DateBegin = conditions.DateBegin.Value; //must have value 
            }

            if (conditions.DateEnd.HasValue)
            {
                output.DateEnd = conditions.DateEnd.Value; //must have value 
            }
            output.KemasBookingNumber = conditions.KemasBookingNumber;
            output.UserName = conditions.UserName;

            if (conditions.UserID.HasValue)
            {
                output.UserID = conditions.UserID.Value; 
            }
            output.ItemsPerPage = items.Length;
            output.Items = items;
            output.PageNumber = 1;
            output.TotalPage = 1;

            return output;
        }

        private static DebitNoteDetail ReadSingleDetailFromDataReader(SqlDataReader sqlReader)
        {
            DebitNoteDetail item = null;

            while (sqlReader.Read())
            {
                item = new DebitNoteDetail();
                item.ID = Convert.ToInt32(sqlReader["ID"].ToString());
                item.DebitNoteID = Convert.ToInt32(sqlReader["DebitNoteID"].ToString());
                item.ClientID = Guid.Parse(sqlReader["ClientID"].ToString());
                item.UserID = Guid.Parse(sqlReader["UserID"].ToString());

                item.UserName = string.Format("{0} {1}", sqlReader["UserFirstName"], sqlReader["UserLastName"]);
                item.BookingID = Convert.ToInt32(sqlReader["BookingID"].ToString());

                item.KemasBookingID = Guid.Parse(sqlReader["KemasBookingID"].ToString());
                item.KemasBookingNumber = sqlReader["KemasBookingNumber"].ToString();

                if (sqlReader["OrderID"].Equals(DBNull.Value))
                {
                    item.OrderID = null;
                }
                else
                {
                    item.OrderID = Convert.ToInt32(sqlReader["OrderID"].ToString());
                }

                if (!sqlReader["Category"].Equals(DBNull.Value))
                {
                    item.ItemCategory = sqlReader["Category"].ToString();
                }

                item.OrderDate = DateTime.ParseExact(sqlReader["OrderDate"].ToString(), DebitNoteUtility.OrderDateFormat, null);

                if (sqlReader["TotalAmount"].Equals(DBNull.Value))
                {
                    item.TotalAmount = null;
                }
                else
                {
                    item.TotalAmount = decimal.Parse(sqlReader["TotalAmount"].ToString());
                }

                item.PaymentStatus = (PaymentState)Enum.Parse(typeof(PaymentState), sqlReader["PaymentStatus"].ToString());
            }

            return item;
        }

        private static DebitNoteDetail[] ReadMultipleDetailFromDataReader(SqlDataReader sqlReader)
        {
            List<DebitNoteDetail> items = new List<DebitNoteDetail>();

            while (sqlReader.Read())
            {
                DebitNoteDetail item = new DebitNoteDetail();

                item.ID = Convert.ToInt32(sqlReader["ID"].ToString());
                item.DebitNoteID = Convert.ToInt32(sqlReader["DebitNoteID"].ToString());
                item.ClientID = Guid.Parse(sqlReader["ClientID"].ToString());
                item.UserID = Guid.Parse(sqlReader["UserID"].ToString());

                item.UserName = string.Format("{0} {1}", sqlReader["UserFirstName"], sqlReader["UserLastName"]);
                item.BookingID = Convert.ToInt32(sqlReader["BookingID"].ToString());

                item.KemasBookingID = Guid.Parse(sqlReader["KemasBookingID"].ToString());
                item.KemasBookingNumber = sqlReader["KemasBookingNumber"].ToString();

                if (sqlReader["OrderID"].Equals(DBNull.Value))
                {
                    item.OrderID = null;
                }
                else
                {
                    item.OrderID = Convert.ToInt32(sqlReader["OrderID"].ToString());
                }

                if (!sqlReader["Category"].Equals(DBNull.Value))
                {
                    item.ItemCategory = sqlReader["Category"].ToString();
                }

                item.OrderDate = DateTime.ParseExact(sqlReader["OrderDate"].ToString(), DebitNoteUtility.OrderDateFormat, null);

                if (sqlReader["TotalAmount"].Equals(DBNull.Value))
                {
                    item.TotalAmount = null;
                }
                else
                {
                    item.TotalAmount = decimal.Parse(sqlReader["TotalAmount"].ToString());
                }

                item.PaymentStatus = (PaymentState)Enum.Parse(typeof(PaymentState), sqlReader["PaymentStatus"].ToString());

                items.Add(item);
            }

            return items.ToArray();
        }
        #endregion

        #region Debit-Note Periods region

        public static DebitNotePeriod[] RetrieveCompletedPeriods(ProxyUserSetting operatorInfo)
        {
            DebitNotePeriod[] periods = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            periods = DataAccessProxyConstantRepo.ExecuteSPReturnReader<DebitNotePeriod[]>(RetrieveDebitNoteCompletedPeriods, parameters.ToArray(), (sqldatareader) => ReadMultiplePeriodFromDataReader(sqldatareader));
            parameters.Clear();

            return periods;
 
        }
        public static DebitNotePeriod[] RetrievePeriodsByState(SyncedRecordState state, int debitMonth ,ProxyUserSetting operatorInfo)
        {
            DebitNotePeriod[] periods = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter stateParam = new SqlParameter("@state", (int)state);
            parameters.Add(stateParam);

            SqlParameter createdOnParam = new SqlParameter("@createdOn", DateTime.Now);
            parameters.Add(createdOnParam);

            SqlParameter createdByParam = new SqlParameter("@createdBy", operatorInfo.ID);
            parameters.Add(createdByParam);

            //add new parameter from external layer to decide in which month debit note should meet bookings
            SqlParameter debitMonthByParam = new SqlParameter("@debitMonth", debitMonth);
            parameters.Add(debitMonthByParam);

            periods = DataAccessProxyConstantRepo.ExecuteSPReturnReader<DebitNotePeriod[]>(Sp_DebitNotePeriod_RetrieveByState, parameters.ToArray(), (sqldatareader) => ReadMultiplePeriodFromDataReader(sqldatareader));
            parameters.Clear();

            return periods;
        }

        public static DebitNotePeriod[] RetrieveDebitNotePeriods(ProxyUserSetting operatorInfo)
        {
            DebitNotePeriod[] periods = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            periods = DataAccessProxyConstantRepo.ExecuteSPReturnReader<DebitNotePeriod[]>(RetrieveDebitNotesPeriods, parameters.ToArray(), (sqldatareader) => ReadMultiplePeriodFromDataReader(sqldatareader));
            parameters.Clear();

            return periods;
        }

        public static void ClearUpTempDataByPeriod(DebitNotePeriod dnp, ProxyUserSetting operatorInfo)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter periodParam = new SqlParameter("@periodID", dnp.ID);
            parameters.Add(periodParam);

            DataAccessProxyConstantRepo.ExecuteSPNonQuery(ClearUpDebitNoteTempData, parameters.ToArray());
            parameters.Clear();
        }

        private static DebitNotePeriod ReadSinglePeriodFromDataReader(SqlDataReader sqlReader)
        {
            DebitNotePeriod period = null;

            while (sqlReader.Read())
            {
                period = new DebitNotePeriod();
                  //  [ID]
                  //,[Period]
                  //,[PeriodBegin]
                  //,[PeriodEnd]
                  //,[BillingDate]
                  //,[DueDate]
                  //,[Type]
                  //,[State]
                  //,[CreatedOn]
                  //,[CreatedBy]
                  //,[ModifiedOn]
                  //,[ModifiedBy]
                period.ID = Convert.ToInt32(sqlReader[0].ToString());
                period.Period = sqlReader[1].ToString();
                period.PeriodStartDate = DateTime.Parse(sqlReader[2].ToString());
                period.PeriodEndDate = DateTime.Parse(sqlReader[3].ToString());
                period.BillingDate = DateTime.Parse(sqlReader[4].ToString()); 
                period.DueDate = DateTime.Parse(sqlReader[5].ToString());
                period.State = (SyncedRecordState)Enum.Parse(typeof(SyncedRecordState), sqlReader[6].ToString());
                period.CreatedOn = Convert.ToDateTime(sqlReader[7].ToString());
                period.CreatedBy = Guid.Parse(sqlReader[8].ToString());

                if (!sqlReader[9].Equals(DBNull.Value))
                {
                    period.ModifiedOn = Convert.ToDateTime(sqlReader[9].ToString());
                }

                if (!sqlReader[10].Equals(DBNull.Value))
                {
                    period.ModifiedBy = Guid.Parse(sqlReader[10].ToString());
                }
            }

            return period;
        }

        private static DebitNotePeriod[] ReadMultiplePeriodFromDataReader(SqlDataReader sqlReader)
        {
            List<DebitNotePeriod> periods = new List<DebitNotePeriod>();

            while (sqlReader.Read())
            {
                DebitNotePeriod period = new DebitNotePeriod();
                        //[ID]
                //      ,[Period]
                //      ,[PeriodBegin]
                //      ,[PeriodEnd]
                //      ,[BillingDate]
                //      ,[DueDate]
                //      ,[GenerationType]
                //      ,[State]
                //      ,[CreatedOn]
                //      ,[CreatedBy]
                //      ,[ModifiedOn]
                //      ,[ModifiedBy]

                period.ID = Convert.ToInt32(sqlReader[0].ToString());
                period.Period = sqlReader[1].ToString();
                period.PeriodStartDate = DateTime.Parse(sqlReader[2].ToString());
                period.PeriodEndDate = DateTime.Parse(sqlReader[3].ToString());
                period.BillingDate = DateTime.Parse(sqlReader[4].ToString());
                period.DueDate = DateTime.Parse(sqlReader[5].ToString());

                period.State = (SyncedRecordState)Enum.Parse(typeof(SyncedRecordState), sqlReader[6].ToString());
                period.CreatedOn = Convert.ToDateTime(sqlReader[7].ToString());
                period.CreatedBy = Guid.Parse(sqlReader[8].ToString());

                if (!sqlReader[9].Equals(DBNull.Value))
                {
                    period.ModifiedOn = Convert.ToDateTime(sqlReader[9].ToString());
                }

                if (!sqlReader[10].Equals(DBNull.Value))
                {
                    period.ModifiedBy = Guid.Parse(sqlReader[10].ToString());
                }


                periods.Add(period);
            }

            return periods.ToArray();
        }
        #endregion

        #region Generate Debit notes by background service
        public static void GeneateDebitNotes(DebitNotePeriod dnp, ProxyUserSetting operatorInfo)
        {
            //@period nvarchar(50),
            //@beginDate datetime,
            //@enddate datetime,
            //@dueDate datetime,
            //@clients dbo.Client readonly,
            //@createdOn datetime,
            //@createdBy uniqueidentifier

            int affectedCnt = -1;

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter periodParam = new SqlParameter("@periodID", dnp.ID);
            parameters.Add(periodParam);

            SqlParameter createdOnParam = new SqlParameter("@createdOn", dnp.CreatedOn);
            parameters.Add(createdOnParam);

            SqlParameter createdByParam = new SqlParameter("@createdBy", dnp.CreatedBy);
            parameters.Add(createdByParam);

            SqlParameter retParam = new SqlParameter("@return_value", -1);
            retParam.Direction = ParameterDirection.InputOutput;
            parameters.Add(retParam);

            affectedCnt = DataAccessProxyConstantRepo.ExecuteSPNonQuery(GenerateDebitNotesByMonth, parameters.ToArray());
            parameters.Clear();
        }

        #endregion

        #region Retrieve DebitNotes according search conditions with paging
        public static DebitNotesSearchConditions RetrieveDebitNotesWithPaging(DebitNotesSearchConditions searchConditions, ProxyUserSetting operatorInfo)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            //@clientID nvarchar(50),
            //@status tinyint,
            //@periodBegin Datetime2(7),
            //@periodEnd datetime2(7),

            //@queryTime datetime2(7),

            //@itemsPerPage int,
            //@pageNumber int,
            //@totalPages int output,
            //@return_value int output
            SqlParameter clientNamePara = null;

            if (!searchConditions.ClientID.HasValue)
            {
                clientNamePara = new SqlParameter("@clientID", DBNull.Value);
            }
            else
            {
                clientNamePara = new SqlParameter("@clientID", searchConditions.ClientID); 
            }
            parameters.Add(clientNamePara);

            SqlParameter statusPara = null;
            if (searchConditions.Status.HasValue)
            {
                statusPara = new SqlParameter("@status", (int)searchConditions.Status);
            }
            else
            {
                statusPara = new SqlParameter("@status", DBNull.Value);
            }
            parameters.Add(statusPara);

            SqlParameter periodBeginPara = null;
            if (searchConditions.PeriodBegin.HasValue)
            {
                periodBeginPara = new SqlParameter("@periodBegin", searchConditions.PeriodBegin.Value);
            }
            else
            {
                periodBeginPara = new SqlParameter("@periodBegin", DBNull.Value); 
            }
            parameters.Add(periodBeginPara);

            SqlParameter periodEndPara = null;
            if (searchConditions.PeriodEnd.HasValue)
            {
                periodEndPara = new SqlParameter("@periodEnd", searchConditions.PeriodEnd.Value);
            }
            else
            {
                periodEndPara = new SqlParameter("@periodEnd", DBNull.Value);
            }
            parameters.Add(periodEndPara);


            SqlParameter itemsPerPagePara = new SqlParameter("@itemsPerPage", searchConditions.ItemsPerPage);
            parameters.Add(itemsPerPagePara);

            SqlParameter pageNumberPara = new SqlParameter("@pageNumber", searchConditions.PageNumber);
            parameters.Add(pageNumberPara);

            SqlParameter totalPagePara = new SqlParameter("@totalPages", searchConditions.TotalPages);
            totalPagePara.Direction = ParameterDirection.Output;
            parameters.Add(totalPagePara);

            SqlParameter spRet = new SqlParameter("@return_value", -1);
            spRet.Direction = ParameterDirection.Output;
            parameters.Add(spRet);

            DebitNote[] debitNotes = DataAccessProxyConstantRepo.ExecuteSPReturnReader<DebitNote[]>(RetrieveDebitNotes, parameters.ToArray(), (sqldatareader) => ReadMultipleNoteFromDataReader(sqldatareader));

            parameters.Clear();
            DebitNoteUtility.DeterminePaymentStatus(debitNotes, searchConditions.QueryTime);

            //construct resposne
            DebitNotesSearchConditions output = new DebitNotesSearchConditions();
            output.ClientID = searchConditions.ClientID;
            output.ItemsPerPage = searchConditions.ItemsPerPage;
            output.Notes = debitNotes;
            output.PageNumber = searchConditions.PageNumber;
            output.PeriodBegin = searchConditions.PeriodBegin;
            output.PeriodEnd = searchConditions.PeriodEnd;
            output.Status = searchConditions.Status;
            output.QueryTime = searchConditions.QueryTime;
            output.TotalPages = Convert.ToInt32(totalPagePara.Value);

            return output;
        }
        #endregion

        #region Retrieve DebitNote by ID
        public static DebitNote RetrieveDebitNotesByID(int debitNoteID,DateTime queryTime,ProxyUserSetting operatorInfo)
        {
            DebitNote debitNote = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter debitNoteIDPara = new SqlParameter("@debitNoteID", debitNoteID);
            parameters.Add(debitNoteIDPara);

            debitNote = DataAccessProxyConstantRepo.ExecuteSPReturnReader<DebitNote>(RetrieveDebitNoteByID, parameters.ToArray(), (sqldatareader) => ReadSingleNoteFromDataReader(sqldatareader));

            parameters.Clear();

            if (debitNote != null)
            {
                DebitNoteUtility.DeterminePaymentStatus(new DebitNote[1] { debitNote }, queryTime); 
            }
            return  debitNote;
        }
        #endregion

        #region update debit note payment status
        public static DebitNote UpdateDebitNote(DebitNote note,ProxyUserSetting operatorInfo)
        {

            DebitNote debitNote = null;
            List<SqlParameter> parameters = new List<SqlParameter>();

            //@debitNoteID int,
            //@clientID uniqueidentifier,
            SqlParameter debitNoteIDPara = new SqlParameter("@debitNoteID", note.ID);
            parameters.Add(debitNoteIDPara);

            SqlParameter clientIDPara = new SqlParameter("@clientID", note.ClientID);
            parameters.Add(clientIDPara);

            //@paymentDate datetime,
            //@totalAmount decimal(18,3),
            SqlParameter paymentDatePara = null;
            if (note.PaymentDate.HasValue)
            {
                paymentDatePara = new SqlParameter("@paymentDate", note.PaymentDate.Value);
            }
            else
            {
                paymentDatePara = new SqlParameter("@paymentDate", DBNull.Value); 
            }
            parameters.Add(paymentDatePara);

            SqlParameter totalAmountPara = new SqlParameter("@totalAmount", note.TotalAmount);
            parameters.Add(totalAmountPara);

            //@note nvarchar(200),
            //@generationType tinyint,
            SqlParameter notePara = new SqlParameter("@note", note.Note);
            parameters.Add(notePara);

            SqlParameter paymentStatePara = new SqlParameter("@paymentState", (int)note.PaymentStatus);
            parameters.Add(paymentStatePara);
            //@state tinyint,
            //@modifiedOn datetime,
            //@modifiedBy uniqueidentifier
            SqlParameter statePara = new SqlParameter("@state", note.State);
            parameters.Add(statePara);
            SqlParameter modifiedOnPara = new SqlParameter("@modifiedOn", note.ModifiedOn.Value);
            parameters.Add(modifiedOnPara);
            SqlParameter modifiedByPara = new SqlParameter("@modifiedBy", note.ModifiedBy.Value);
            parameters.Add(modifiedByPara);

            debitNote = DataAccessProxyConstantRepo.ExecuteSPReturnReader<DebitNote>(UpdateDebitNoteByID, parameters.ToArray(), (sqldatareader) => ReadSingleNoteFromDataReader(sqldatareader));

            parameters.Clear();

            DebitNoteUtility.DeterminePaymentStatus(new DebitNote[1] { debitNote }, DateTime.Now);
            return debitNote;
        }
        #endregion

        #region Debit Note Helper class
        private static DebitNote ReadSingleNoteFromDataReader(SqlDataReader sqlReader)
        {
            DebitNote note = null;

            while (sqlReader.Read())
            {
                note = new DebitNote();
                //notes.ID,
                //notes.ClientID,
                //notes.PeriodID,
                //dnh.Period,
                //dnh.PeriodBegin,
                note.ID = Convert.ToInt32(sqlReader[0].ToString());
                note.ClientID = Guid.Parse(sqlReader[1].ToString());
                note.PeriodID = Convert.ToInt32(sqlReader[2].ToString());
                note.Period = sqlReader[3].ToString();
                note.PeriodStartDate = DateTime.Parse(sqlReader[4].ToString());

                //dnh.PeriodEnd,
                //dnh.DueDate, 
                //notes.PaymentDate,
                //notes.TotalAmount,
                //notes.PaymentStatus,
                note.PeriodEndDate = DateTime.Parse(sqlReader[5].ToString());
                note.BillingDate = DateTime.Parse(sqlReader[6].ToString());

                note.DueDate = DateTime.Parse(sqlReader[7].ToString());
                if (!sqlReader[8].Equals(DBNull.Value))
                {
                    note.PaymentDate = DateTime.Parse(sqlReader[8].ToString());
                }
                else
                {
                    note.PaymentDate = new Nullable<DateTime>();
                }
                note.TotalAmount = decimal.Parse(sqlReader[9].ToString());
                note.PaymentStatus = (PaymentState)Enum.Parse(typeof(PaymentState), sqlReader[10].ToString());

                //notes.Note,
                //notes.State,
                //notes.CreatedOn,
                //notes.CreatedBy,
                //notes.ModifiedOn,
                //notes.ModifiedBy
                note.Note = sqlReader[11].ToString();
                
                note.State = (CommonState)Enum.Parse(typeof(CommonState), sqlReader[12].ToString());
                note.CreatedOn = Convert.ToDateTime(sqlReader[13].ToString());
                note.CreatedBy = Guid.Parse(sqlReader[14].ToString());

                if (!sqlReader[15].Equals(DBNull.Value))
                {
                    note.ModifiedOn = Convert.ToDateTime(sqlReader[15].ToString());
                }

                if (!sqlReader[16].Equals(DBNull.Value))
                {
                    note.ModifiedBy = Guid.Parse(sqlReader[16].ToString());
                }
            }

            return note;
        }

        private static DebitNote[] ReadMultipleNoteFromDataReader(SqlDataReader sqlReader)
        {
            List<DebitNote> notes = new List<DebitNote>();

            while (sqlReader.Read())
            {
                DebitNote note = new DebitNote();
                //notes.ID,
                //notes.ClientID,
                //notes.PeriodID,
                //dnh.Period,
                //dnh.PeriodBegin,
                note.ID = Convert.ToInt32(sqlReader[0].ToString());
                note.ClientID = Guid.Parse(sqlReader[1].ToString());
                note.PeriodID = Convert.ToInt32(sqlReader[2].ToString());
                note.Period = sqlReader[3].ToString();
                note.PeriodStartDate = DateTime.Parse(sqlReader[4].ToString());

                //dnh.PeriodEnd,
                //dnh.DueDate, 
                //notes.PaymentDate,
                //notes.TotalAmount,
                //notes.PaymentStatus,
                note.PeriodEndDate = DateTime.Parse(sqlReader[5].ToString());

                note.BillingDate = DateTime.Parse(sqlReader[6].ToString());

                note.DueDate = DateTime.Parse(sqlReader[7].ToString());
                if (!sqlReader[8].Equals(DBNull.Value))
                {
                    note.PaymentDate = DateTime.Parse(sqlReader[8].ToString());
                }
                else
                {
                    note.PaymentDate = new Nullable<DateTime>();
                }
                note.TotalAmount = decimal.Parse(sqlReader[9].ToString());
                note.PaymentStatus = (PaymentState)Enum.Parse(typeof(PaymentState), sqlReader[10].ToString());

                //notes.Note,
                //notes.GenerationType,
                //notes.State,
                //notes.CreatedOn,
                //notes.CreatedBy,
                //notes.ModifiedOn,
                //notes.ModifiedBy
                note.Note = sqlReader[11].ToString();
                
                note.State = (CommonState)Enum.Parse(typeof(CommonState), sqlReader[12].ToString());
                note.CreatedOn = Convert.ToDateTime(sqlReader[13].ToString());
                note.CreatedBy = Guid.Parse(sqlReader[14].ToString());

                if (!sqlReader[15].Equals(DBNull.Value))
                {
                    note.ModifiedOn = Convert.ToDateTime(sqlReader[15].ToString());
                }

                if (!sqlReader[16].Equals(DBNull.Value))
                {
                    note.ModifiedBy = Guid.Parse(sqlReader[16].ToString());
                }
                notes.Add(note);
            }

            return notes.ToArray();
        }
        #endregion

        #region retrieve bookingIDs
        public static BookingCompact[] RetrieveID(BookingCompact[] bc, ProxyUserSetting operatorInfo)
        {
            BookingCompact[] output = null;
            IEnumerable<SqlDataRecord> idRecords = null;
            if (bc != null && bc.Length > 0)
            {
                idRecords = CreateIDRecord(bc);
                List<SqlParameter> parameters = new List<SqlParameter>();
                SqlParameter idParam = new SqlParameter("@kemasIDs", idRecords);
                idParam.SqlDbType = SqlDbType.Structured;
                idParam.TypeName = "dbo.KemasID";
                parameters.Add(idParam);

                output = DataAccessProxyConstantRepo.ExecuteSPReturnReader<BookingCompact[]>(RetrieveBookingIDs, parameters.ToArray(), (sqldatareader) => ReadCompactInfoFromDataReader(sqldatareader));
                parameters.Clear();
            }

            return output;
        }

        private static BookingCompact[] ReadCompactInfoFromDataReader(SqlDataReader sqlReader)
        {
            List<BookingCompact> mappedIDs = new List<BookingCompact>();

            while (sqlReader.Read())
            {
                BookingCompact bc = new BookingCompact();

                bc.KemasBookingID = Guid.Parse(sqlReader["KemasBookingID"].ToString());

                if (!sqlReader["BookingID"].Equals(DBNull.Value))
                {
                    bc.BookingID = Convert.ToInt32(sqlReader["BookingID"].ToString());
                } 
                
                if(!sqlReader["OrderID"].Equals(DBNull.Value))
                {
                    bc.OrderID = Convert.ToInt32(sqlReader["OrderID"].ToString());
                }

                mappedIDs.Add(bc);
            }

            return mappedIDs.ToArray();
        }

        //parent records
        public static IEnumerable<SqlDataRecord> CreateIDRecord(BookingCompact[] bc)
        {
            SqlMetaData[] metaData = new SqlMetaData[3]
            {
                new SqlMetaData("KemasBookingID",SqlDbType.UniqueIdentifier),
                new SqlMetaData("BookingID",SqlDbType.Int),
                new SqlMetaData("OrderID",SqlDbType.Int)
            };
            return bc.Select(m => SetKemasIDValues(metaData, m));
        }
        private static SqlDataRecord SetKemasIDValues(SqlMetaData[] metaData, BookingCompact compact)
        {
            SqlDataRecord bc = new SqlDataRecord(metaData);
            bc.SetGuid(0, compact.KemasBookingID);
            bc.SetDBNull(1);
            bc.SetDBNull(2);
            return bc;
        }
        #endregion
    }
}
